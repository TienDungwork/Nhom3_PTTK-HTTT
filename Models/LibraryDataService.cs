using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryManagement.Models
{
    public static class LibraryDataService
    {
        public static IReadOnlyList<BookCategory> GetCategories(bool includeInactive = true)
        {
            return includeInactive
                ? SampleData.BookCategories.OrderBy(c => c.TenDanhMuc).ToList()
                : SampleData.BookCategories.Where(c => c.DangSuDung).OrderBy(c => c.TenDanhMuc).ToList();
        }

        public static string GetCategoryName(string maDanhMuc, string fallback = "")
        {
            var category = SampleData.BookCategories.FirstOrDefault(c => c.MaDanhMuc == maDanhMuc);
            return category?.TenDanhMuc ?? fallback;
        }

        public static IReadOnlyList<Book> SearchBooks(
            string keyword = "",
            string tacGia = "",
            string nhanDe = "",
            string chuDe = "",
            string maSach = "",
            string danhMuc = "")
        {
            var results = SampleData.Books.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                results = results.Where(b =>
                    b.MaSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    b.TenSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    b.TacGia.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    b.ChuDe.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    GetCategoryName(b.MaDanhMuc, b.TheLoai).Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(tacGia))
                results = results.Where(b => b.TacGia.Contains(tacGia.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(nhanDe))
                results = results.Where(b => b.TenSach.Contains(nhanDe.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(chuDe))
                results = results.Where(b => b.ChuDe.Contains(chuDe.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(maSach))
                results = results.Where(b => b.MaSach.Contains(maSach.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(danhMuc) && danhMuc != "Tất cả")
                results = results.Where(b => string.Equals(GetCategoryName(b.MaDanhMuc, b.TheLoai), danhMuc, StringComparison.OrdinalIgnoreCase));

            var list = results.OrderBy(b => b.TenSach).ToList();
            foreach (var book in list)
            {
                SyncBookCategory(book);
                SyncBookStatus(book);
            }
            return list;
        }

        public static IReadOnlyList<BookCopy> GetCopiesForBook(string maSach, bool includeUnavailable = true)
        {
            var query = SampleData.BookCopies.Where(c => c.MaSach == maSach);
            if (!includeUnavailable)
                query = query.Where(c => string.Equals(c.TrangThai, "Có sẵn", StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(c => c.NgayNhap).ThenBy(c => c.MaQuyenSach).ToList();
        }

        public static IReadOnlyList<BookCopy> SearchCopies(string keyword = "", string maSach = "")
        {
            var query = SampleData.BookCopies.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(maSach))
                query = query.Where(c => c.MaSach == maSach);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(c =>
                    c.MaQuyenSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    c.MaSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    GetBookName(c.MaSach).Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    GetBookCategoryName(c.MaSach).Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    c.NhaCungCap.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    c.TrangThai.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderByDescending(c => c.NgayNhap).ThenBy(c => c.MaQuyenSach).ToList();
        }

        public static string GetBookName(string maSach)
        {
            return SampleData.Books.FirstOrDefault(b => b.MaSach == maSach)?.TenSach ?? "";
        }

        public static string GetBookCategoryName(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return "";
            return GetCategoryName(book.MaDanhMuc, book.TheLoai);
        }

        public static void NormalizeData()
        {
            foreach (var book in SampleData.Books)
                SyncBookCategory(book);

            EnsureCopiesForAllBooks();

            foreach (var book in SampleData.Books)
                RecalculateBookInventoryFromCopies(book.MaSach);
        }

        public static void SyncBookCategory(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.MaDanhMuc) && !string.IsNullOrWhiteSpace(book.TheLoai))
            {
                var category = SampleData.BookCategories.FirstOrDefault(c => string.Equals(c.TenDanhMuc, book.TheLoai, StringComparison.OrdinalIgnoreCase));
                if (category != null)
                    book.MaDanhMuc = category.MaDanhMuc;
            }

            if (!string.IsNullOrWhiteSpace(book.MaDanhMuc))
                book.TheLoai = GetCategoryName(book.MaDanhMuc, book.TheLoai);
        }

        public static void SyncBookStatus(Book book)
        {
            if (book.SoLuong < 0) book.SoLuong = 0;
            if (book.SoLuongDangMuon < 0) book.SoLuongDangMuon = 0;
            if (book.SoLuongMatHong < 0) book.SoLuongMatHong = 0;

            if (book.SoLuongDangMuon > book.SoLuong)
                book.SoLuongDangMuon = book.SoLuong;

            if (book.SoLuongDangMuon + book.SoLuongMatHong > book.SoLuong)
                book.SoLuongMatHong = Math.Max(0, book.SoLuong - book.SoLuongDangMuon);

            book.TrangThai = book.SoLuongHienCo > 0 ? "Có sẵn" : "Hết sách";
        }

        public static void RecalculateBookInventoryFromCopies(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return;

            var copies = SampleData.BookCopies.Where(c => c.MaSach == maSach).ToList();
            int dangMuon = copies.Count(c => c.TrangThai == "Đang mượn");
            int matHong = copies.Count(c => c.TrangThai == "Mất" || c.TrangThai == "Hỏng");
            book.SoLuongDangMuon = dangMuon;
            book.SoLuongMatHong = matHong;
            SyncBookStatus(book);
        }

        public static (bool Success, string Message) SaveBook(Book book, bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(book.MaSach) || string.IsNullOrWhiteSpace(book.TenSach))
                return (false, "Mã sách và tên sách là bắt buộc.");

            if (string.IsNullOrWhiteSpace(book.MaDanhMuc))
                return (false, "Vui lòng chọn danh mục sách.");

            if (!IsValidIsbn(book.ISBN))
                return (false, "ISBN không hợp lệ. Chỉ chấp nhận ISBN-10 hoặc ISBN-13.");

            int currentYear = DateTime.Now.Year;
            if (book.NamXuatBan < 1450 || book.NamXuatBan > currentYear + 1)
                return (false, $"Năm xuất bản phải trong khoảng 1450 - {currentYear + 1}.");

            SyncBookCategory(book);

            var existing = SampleData.Books.FirstOrDefault(b => b.MaSach == book.MaSach);
            if (!isEditMode)
            {
                if (existing != null)
                    return (false, "Mã sách đã tồn tại.");

                if (book.SoLuong < 0)
                    return (false, "Số lượng đầu sách không hợp lệ.");

                book.SoLuongDangMuon = 0;
                book.SoLuongMatHong = 0;
                SyncBookStatus(book);
                SampleData.Books.Add(book);

                for (int i = 0; i < book.SoLuong; i++)
                {
                    SampleData.BookCopies.Add(new BookCopy
                    {
                        MaQuyenSach = GenerateCopyCode(),
                        MaSach = book.MaSach,
                        NgayNhap = DateTime.Today,
                        TrangThai = "Có sẵn",
                        NhaCungCap = book.NhaCungCap
                    });
                }
                RecalculateBookInventoryFromCopies(book.MaSach);
                return (true, "Thêm đầu sách thành công.");
            }

            if (existing == null)
                return (false, "Không tìm thấy đầu sách cần cập nhật.");

            existing.TenSach = book.TenSach;
            existing.TacGia = book.TacGia;
            existing.MaDanhMuc = book.MaDanhMuc;
            existing.TheLoai = GetCategoryName(book.MaDanhMuc, book.TheLoai);
            existing.ChuDe = book.ChuDe;
            existing.NamXuatBan = book.NamXuatBan;
            existing.NhaXuatBan = book.NhaXuatBan;
            existing.URI = book.URI;
            existing.ISBN = book.ISBN;
            existing.ViTriKho = book.ViTriKho;
            existing.NhaCungCap = book.NhaCungCap;
            if (book.SoLuong < 0)
                return (false, "Số lượng đầu sách không hợp lệ.");

            var copies = SampleData.BookCopies.Where(c => c.MaSach == existing.MaSach).OrderBy(c => c.NgayNhap).ThenBy(c => c.MaQuyenSach).ToList();
            int currentCopyCount = copies.Count;
            if (book.SoLuong < currentCopyCount)
            {
                int needRemove = currentCopyCount - book.SoLuong;
                var removable = copies.Where(c => c.TrangThai == "Có sẵn").Take(needRemove).ToList();
                if (removable.Count < needRemove)
                    return (false, "Không thể giảm số lượng vì chưa đủ quyển sách ở trạng thái Có sẵn.");

                foreach (var copy in removable)
                    SampleData.BookCopies.Remove(copy);
            }
            else if (book.SoLuong > currentCopyCount)
            {
                int needAdd = book.SoLuong - currentCopyCount;
                for (int i = 0; i < needAdd; i++)
                {
                    SampleData.BookCopies.Add(new BookCopy
                    {
                        MaQuyenSach = GenerateCopyCode(),
                        MaSach = existing.MaSach,
                        NgayNhap = DateTime.Today,
                        TrangThai = "Có sẵn",
                        NhaCungCap = existing.NhaCungCap
                    });
                }
            }

            existing.SoLuong = book.SoLuong;
            RecalculateBookInventoryFromCopies(existing.MaSach);

            return (true, "Cập nhật đầu sách thành công.");
        }

        public static (bool Success, string Message) DeleteBook(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.");

            if (SampleData.BorrowRecords.Any(r => r.MaSach == maSach && r.TrangThai == "Đang mượn"))
                return (false, "Không thể xóa đầu sách đang có bản sách được mượn.");

            if (SampleData.BorrowRecords.Any(r => r.MaSach == maSach))
                return (false, "Không thể xóa đầu sách đã phát sinh giao dịch mượn/trả.");

            SampleData.BookCopies.RemoveAll(c => c.MaSach == maSach);
            SampleData.Books.Remove(book);
            return (true, $"Đã xóa đầu sách \"{book.TenSach}\".");
        }

        public static (bool Success, string Message) SaveCategory(BookCategory category, bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(category.MaDanhMuc) || string.IsNullOrWhiteSpace(category.TenDanhMuc))
                return (false, "Mã danh mục và tên danh mục là bắt buộc.");

            var duplicateCode = SampleData.BookCategories.FirstOrDefault(c => c.MaDanhMuc == category.MaDanhMuc);
            var duplicateName = SampleData.BookCategories.FirstOrDefault(c =>
                c.MaDanhMuc != category.MaDanhMuc &&
                string.Equals(c.TenDanhMuc, category.TenDanhMuc, StringComparison.OrdinalIgnoreCase));

            if (!isEditMode && duplicateCode != null)
                return (false, "Mã danh mục đã tồn tại.");
            if (duplicateName != null)
                return (false, "Tên danh mục đã tồn tại.");

            if (!isEditMode)
            {
                SampleData.BookCategories.Add(category);
                return (true, "Thêm danh mục sách thành công.");
            }

            if (duplicateCode == null)
                return (false, "Không tìm thấy danh mục cần cập nhật.");

            duplicateCode.TenDanhMuc = category.TenDanhMuc;
            duplicateCode.MoTa = category.MoTa;
            duplicateCode.ViTriKe = category.ViTriKe;
            duplicateCode.DangSuDung = category.DangSuDung;

            foreach (var book in SampleData.Books.Where(b => b.MaDanhMuc == duplicateCode.MaDanhMuc))
                book.TheLoai = duplicateCode.TenDanhMuc;

            return (true, "Cập nhật danh mục sách thành công.");
        }

        public static (bool Success, string Message) DeleteCategory(string maDanhMuc)
        {
            var category = SampleData.BookCategories.FirstOrDefault(c => c.MaDanhMuc == maDanhMuc);
            if (category == null)
                return (false, "Không tìm thấy danh mục sách.");

            if (SampleData.Books.Any(b => b.MaDanhMuc == maDanhMuc))
                return (false, "Không thể xóa danh mục đang có đầu sách.");

            SampleData.BookCategories.Remove(category);
            return (true, $"Đã xóa danh mục \"{category.TenDanhMuc}\".");
        }

        public static (bool Success, string Message) SaveCopy(BookCopy copy, bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(copy.MaQuyenSach) || string.IsNullOrWhiteSpace(copy.MaSach))
                return (false, "Mã quyển sách và mã đầu sách là bắt buộc.");

            if (SampleData.Books.FirstOrDefault(b => b.MaSach == copy.MaSach) == null)
                return (false, "Mã đầu sách không tồn tại.");

            var existing = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == copy.MaQuyenSach);
            if (!isEditMode)
            {
                if (existing != null)
                    return (false, "Mã quyển sách đã tồn tại.");

                SampleData.BookCopies.Add(copy);
                var book = SampleData.Books.FirstOrDefault(b => b.MaSach == copy.MaSach);
                if (book != null) book.SoLuong++;
                RecalculateBookInventoryFromCopies(copy.MaSach);
                return (true, "Thêm quyển sách thành công.");
            }

            if (existing == null)
                return (false, "Không tìm thấy quyển sách.");

            if (existing.TrangThai == "Đang mượn" && copy.TrangThai != "Đang mượn")
            {
                bool hasOpenBorrow = SampleData.BorrowRecords.Any(r =>
                    r.MaQuyenSach == existing.MaQuyenSach && r.TrangThai == "Đang mượn");
                if (hasOpenBorrow)
                    return (false, "Không thể đổi trạng thái quyển sách đang có phiếu mượn.");
            }

            existing.NgayNhap = copy.NgayNhap;
            existing.TrangThai = copy.TrangThai;
            existing.NhaCungCap = copy.NhaCungCap;
            existing.GhiChu = copy.GhiChu;
            RecalculateBookInventoryFromCopies(existing.MaSach);

            return (true, "Cập nhật quyển sách thành công.");
        }

        public static (bool Success, string Message) DeleteCopy(string maQuyenSach)
        {
            var copy = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == maQuyenSach);
            if (copy == null)
                return (false, "Không tìm thấy quyển sách.");

            if (copy.TrangThai == "Đang mượn")
                return (false, "Không thể xóa quyển sách đang được mượn.");

            if (SampleData.BorrowRecords.Any(r => r.MaQuyenSach == maQuyenSach))
                return (false, "Không thể xóa quyển sách đã phát sinh giao dịch.");

            SampleData.BookCopies.Remove(copy);
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == copy.MaSach);
            if (book != null && book.SoLuong > 0) book.SoLuong--;
            RecalculateBookInventoryFromCopies(copy.MaSach);
            return (true, $"Đã xóa quyển sách {copy.MaQuyenSach}.");
        }

        public static (bool Success, string Message, string CopyCode) BorrowBook(
            string maSach,
            string maDocGia,
            string tenDocGia,
            DateTime ngayMuon,
            int soNgayMuon,
            string maQuyenSach = "",
            string maYeuCau = "")
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.", "");

            var copy = PickCopyForBorrow(maSach, maQuyenSach);
            if (copy == null)
                return (false, "Không còn quyển sách khả dụng.", "");

            copy.TrangThai = "Đang mượn";

            SampleData.BorrowRecords.Add(new BorrowRecord
            {
                MaMuon = "M" + (SampleData.BorrowRecords.Count + 1).ToString("D3"),
                MaYeuCau = maYeuCau,
                MaDocGia = maDocGia,
                TenDocGia = tenDocGia,
                MaSach = maSach,
                TenSach = book.TenSach,
                MaQuyenSach = copy.MaQuyenSach,
                NgayMuon = ngayMuon,
                NgayHenTra = ngayMuon.AddDays(soNgayMuon),
                TrangThai = "Đang mượn",
                DaThuPhat = false
            });

            RecalculateBookInventoryFromCopies(maSach);
            return (true, $"Mượn sách \"{book.TenSach}\" thành công.", copy.MaQuyenSach);
        }

        public static (bool Success, string Message) CreateBorrowRequest(
            string maSach,
            string maDocGia,
            string tenDocGia,
            DateTime ngayMuonDuKien,
            int soNgayMuon,
            string maQuyenSach = "")
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                return (false, "Không xác định được độc giả.");

            var reader = SampleData.Readers.FirstOrDefault(r => r.MaDocGia == maDocGia);
            if (reader == null)
                return (false, "Độc giả chưa được quản lý trong hệ thống.");

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.");

            if (soNgayMuon <= 0)
                return (false, "Số ngày mượn không hợp lệ.");

            var copy = PickCopyForBorrow(maSach, maQuyenSach);
            if (copy == null)
                return (false, "Không còn quyển sách khả dụng cho yêu cầu này.");

            bool hasPending = SampleData.BorrowRequests.Any(r =>
                r.MaDocGia == maDocGia &&
                r.MaSach == maSach &&
                r.TrangThai == "Chờ duyệt");
            if (hasPending)
                return (false, "Bạn đã có yêu cầu mượn đang chờ duyệt cho đầu sách này.");

            SampleData.BorrowRequests.Add(new BorrowRequest
            {
                MaYeuCau = GenerateBorrowRequestCode(),
                MaDocGia = maDocGia,
                TenDocGia = string.IsNullOrWhiteSpace(tenDocGia) ? reader.HoTen : tenDocGia,
                MaSach = maSach,
                TenSach = book.TenSach,
                MaQuyenSachYeuCau = maQuyenSach,
                NgayMuonDuKien = ngayMuonDuKien,
                SoNgayMuon = soNgayMuon,
                NgayTaoYeuCau = DateTime.Now,
                TrangThai = "Chờ duyệt"
            });

            return (true, "Đã lập phiếu yêu cầu mượn. Vui lòng chờ thủ thư duyệt.");
        }

        public static (bool Success, string Message) ApproveBorrowRequest(string maYeuCau, string nguoiDuyet)
        {
            var request = SampleData.BorrowRequests.FirstOrDefault(r => r.MaYeuCau == maYeuCau);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu mượn.");

            if (request.TrangThai != "Chờ duyệt")
                return (false, "Yêu cầu này đã được xử lý trước đó.");

            var borrowResult = BorrowBook(
                request.MaSach,
                request.MaDocGia,
                request.TenDocGia,
                request.NgayMuonDuKien,
                request.SoNgayMuon,
                request.MaQuyenSachYeuCau,
                request.MaYeuCau);

            if (!borrowResult.Success)
                return (false, borrowResult.Message);

            request.TrangThai = "Đã duyệt";
            request.NguoiDuyet = nguoiDuyet;
            request.MaMuon = SampleData.BorrowRecords.Last().MaMuon;

            SendNotificationToReader(
                request.MaDocGia,
                "Phiếu mượn đã được duyệt",
                $"Yêu cầu {request.MaYeuCau} cho sách \"{request.TenSach}\" đã được duyệt.",
                nguoiDuyet);

            return (true, "Đã duyệt phiếu mượn và tạo phiếu mượn thành công.");
        }

        public static (bool Success, string Message) RejectBorrowRequest(string maYeuCau, string nguoiDuyet, string lyDo = "")
        {
            var request = SampleData.BorrowRequests.FirstOrDefault(r => r.MaYeuCau == maYeuCau);
            if (request == null)
                return (false, "Không tìm thấy yêu cầu mượn.");

            if (request.TrangThai != "Chờ duyệt")
                return (false, "Yêu cầu này đã được xử lý trước đó.");

            request.TrangThai = "Từ chối";
            request.NguoiDuyet = nguoiDuyet;
            request.LyDoTuChoi = lyDo.Trim();

            string noiDung = $"Yêu cầu {request.MaYeuCau} cho sách \"{request.TenSach}\" đã bị từ chối.";
            if (!string.IsNullOrWhiteSpace(request.LyDoTuChoi))
                noiDung += $" Lý do: {request.LyDoTuChoi}.";

            SendNotificationToReader(request.MaDocGia, "Phiếu mượn bị từ chối", noiDung, nguoiDuyet);
            return (true, "Đã từ chối phiếu mượn.");
        }

        public static (bool Success, string Message, BorrowRecord? Record) ReturnBook(string maSach, string maDocGia, DateTime ngayTra)
        {
            var record = SampleData.BorrowRecords
                .Where(r => r.MaSach == maSach && r.MaDocGia == maDocGia && r.TrangThai == "Đang mượn")
                .OrderByDescending(r => r.NgayMuon)
                .FirstOrDefault();

            if (record == null)
                return (false, "Không tìm thấy phiếu mượn phù hợp.", null);

            decimal tienPhatHienTai = CalculateLateFee(record, ngayTra);
            if (tienPhatHienTai > 0 && !record.DaThuPhat)
                return (false, "Phiếu quá hạn chưa được thu phạt, không thể xác nhận trả sách.", null);

            CompleteReturn(record, ngayTra);
            return (true, $"Trả sách \"{record.TenSach}\" thành công.", record);
        }

        public static (bool Success, string Message, BorrowRecord? Record) ReturnBookByBorrowCode(string maMuon, DateTime ngayTra)
        {
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon && r.TrangThai == "Đang mượn");
            if (record == null)
                return (false, "Không tìm thấy phiếu mượn đang hiệu lực.", null);

            decimal tienPhatHienTai = CalculateLateFee(record, ngayTra);
            if (tienPhatHienTai > 0 && !record.DaThuPhat)
                return (false, "Phiếu quá hạn chưa được thu phạt, không thể xác nhận trả sách.", null);

            CompleteReturn(record, ngayTra);
            SendNotificationToReader(
                record.MaDocGia,
                "Xác nhận đã trả sách",
                $"Thư viện đã xác nhận bạn trả sách \"{record.TenSach}\" (phiếu {record.MaMuon}).",
                "Thủ thư");

            return (true, $"Trả sách \"{record.TenSach}\" thành công.", record);
        }

        public static void CompleteReturn(BorrowRecord record, DateTime ngayTra)
        {
            record.NgayTraThuc = ngayTra;
            record.TrangThai = "Đã trả";
            record.TienPhat = CalculateLateFee(record, ngayTra);

            if (!string.IsNullOrWhiteSpace(record.MaQuyenSach))
            {
                var copy = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == record.MaQuyenSach);
                if (copy != null && copy.TrangThai == "Đang mượn")
                    copy.TrangThai = "Có sẵn";
            }

            RecalculateBookInventoryFromCopies(record.MaSach);
        }

        public static (bool Success, string Message) CollectFine(string maMuon)
        {
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
            if (record == null)
                return (false, "Không tìm thấy phiếu mượn.");

            decimal fine = CalculateLateFee(record);
            if (fine <= 0)
            {
                record.TienPhat = 0;
                record.DaThuPhat = true;
                return (true, "Phiếu này không phát sinh tiền phạt.");
            }

            record.TienPhat = fine;
            record.DaThuPhat = true;
            return (true, $"Đã xác nhận thu phạt {fine:N0} VNĐ.");
        }

        public static decimal CalculateLateFee(BorrowRecord record, DateTime? ngayTra = null)
        {
            var compareDate = ngayTra ?? DateTime.Now;
            if (record.TrangThai == "Đã trả" && record.NgayTraThuc.HasValue)
                compareDate = record.NgayTraThuc.Value;

            if (compareDate.Date <= record.NgayHenTra.Date)
                return 0;

            var overdueDays = (compareDate.Date - record.NgayHenTra.Date).Days;
            return overdueDays * 5000m;
        }

        public static int CountBooksByCategory(string maDanhMuc)
        {
            return SampleData.Books.Count(b => b.MaDanhMuc == maDanhMuc);
        }

        public static Notification SendNotificationToReader(string maDocGia, string tieuDe, string noiDung, string nguoiGui = "Thủ thư")
        {
            var notification = new Notification
            {
                MaThongBao = GenerateNotificationCode(),
                MaDocGia = maDocGia,
                NguoiGui = string.IsNullOrWhiteSpace(nguoiGui) ? "Thủ thư" : nguoiGui,
                ThoiGian = DateTime.Now,
                TieuDe = tieuDe.Trim(),
                NoiDung = noiDung.Trim(),
                DaDoc = false
            };
            UserStore.Notifications.Add(notification);
            return notification;
        }

        private static bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return false;

            string normalized = isbn.Replace("-", "").Replace(" ", "");
            if (Regex.IsMatch(normalized, "^[0-9]{13}$"))
                return true;

            if (!Regex.IsMatch(normalized, "^[0-9]{9}[0-9Xx]$"))
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (10 - i) * (normalized[i] - '0');

            int lastValue = normalized[9] is 'X' or 'x' ? 10 : (normalized[9] - '0');
            sum += lastValue;
            return sum % 11 == 0;
        }

        private static void EnsureCopiesForAllBooks()
        {
            foreach (var book in SampleData.Books)
            {
                var copies = SampleData.BookCopies.Where(c => c.MaSach == book.MaSach).ToList();
                if (copies.Count == 0 && book.SoLuong > 0)
                {
                    for (int i = 0; i < book.SoLuong; i++)
                    {
                        SampleData.BookCopies.Add(new BookCopy
                        {
                            MaQuyenSach = GenerateCopyCode(),
                            MaSach = book.MaSach,
                            NgayNhap = DateTime.Today,
                            TrangThai = "Có sẵn",
                            NhaCungCap = book.NhaCungCap
                        });
                    }
                }
                else if (copies.Count > 0 && book.SoLuong <= 0)
                {
                    book.SoLuong = copies.Count;
                }
            }
        }

        private static BookCopy? PickCopyForBorrow(string maSach, string maQuyenSach)
        {
            if (!string.IsNullOrWhiteSpace(maQuyenSach))
            {
                var selected = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == maQuyenSach && c.MaSach == maSach);
                if (selected != null && selected.TrangThai == "Có sẵn")
                    return selected;
                return null;
            }

            return SampleData.BookCopies
                .Where(c => c.MaSach == maSach && c.TrangThai == "Có sẵn")
                .OrderBy(c => c.NgayNhap)
                .ThenBy(c => c.MaQuyenSach)
                .FirstOrDefault();
        }

        public static string GenerateCopyCode()
        {
            int max = SampleData.BookCopies
                .Select(c => c.MaQuyenSach)
                .Where(code => code.StartsWith("Q", StringComparison.OrdinalIgnoreCase))
                .Select(code => int.TryParse(code.Substring(1), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            return "Q" + (max + 1).ToString("D3");
        }

        public static string GenerateBorrowRequestCode()
        {
            int max = SampleData.BorrowRequests
                .Select(r => r.MaYeuCau)
                .Where(code => code.StartsWith("YC", StringComparison.OrdinalIgnoreCase))
                .Select(code => int.TryParse(code.Substring(2), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            return "YC" + (max + 1).ToString("D3");
        }

        public static string GenerateNotificationCode()
        {
            int max = UserStore.Notifications
                .Select(n => n.MaThongBao)
                .Where(code => code.StartsWith("TB", StringComparison.OrdinalIgnoreCase))
                .Select(code => int.TryParse(code.Substring(2), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            return "TB" + (max + 1).ToString("D3");
        }
    }
}
