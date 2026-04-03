using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            string danhMuc = "",
            string trangThai = "")
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

            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "Tất cả")
            {
                list = list
                    .Where(b => string.Equals(b.TrangThai, trangThai.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
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

        /// <summary>Kiểm tra điều kiện mượn: còn slot, không nợ phạt quá hạn, độc giả tồn tại.</summary>
        public static (bool Ok, string Message) ValidateBorrowEligibility(string maDocGia)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                return (false, "Mã độc giả không hợp lệ.");

            if (SampleData.Readers.All(r => r.MaDocGia != maDocGia))
                return (false, "Độc giả chưa có trong hệ thống.");

            int max = int.TryParse(GetSetting("MAX_BORROW", "5"), out int m) ? Math.Max(1, m) : 5;
            int dangMuon = SampleData.BorrowRecords.Count(r => r.MaDocGia == maDocGia && r.TrangThai == "Đang mượn");
            if (dangMuon >= max)
                return (false, $"Độc giả đã mượn tối đa {max} cuốn (theo quy định thư viện).");

            foreach (var r in SampleData.BorrowRecords.Where(x => x.MaDocGia == maDocGia && x.TrangThai == "Đang mượn"))
            {
                if (CalculateLateFee(r) > 0 && !r.DaThuPhat)
                    return (false, "Độc giả còn phiếu quá hạn chưa nộp phạt, không được mượn thêm.");
            }

            return (true, "");
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
            var check = ValidateBorrowEligibility(maDocGia);
            if (!check.Ok)
                return (false, check.Message, "");

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

            var el = ValidateBorrowEligibility(maDocGia);
            if (!el.Ok)
                return (false, el.Message);

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

            var el = ValidateBorrowEligibility(request.MaDocGia);
            if (!el.Ok)
                return (false, el.Message);

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

            CompleteReturn(record, ngayTra, "Tốt");
            return (true, $"Trả sách \"{record.TenSach}\" thành công.", record);
        }

        public static (bool Success, string Message, BorrowRecord? Record) ReturnBookByBorrowCode(string maMuon, DateTime ngayTra, string tinhTrangSachKhiTra = "Tốt")
        {
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon && r.TrangThai == "Đang mượn");
            if (record == null)
                return (false, "Không tìm thấy phiếu mượn đang hiệu lực.", null);

            decimal tienPhatHienTai = CalculateLateFee(record, ngayTra);
            if (tienPhatHienTai > 0 && !record.DaThuPhat)
                return (false, "Phiếu quá hạn chưa được thu phạt, không thể xác nhận trả sách.", null);

            CompleteReturn(record, ngayTra, tinhTrangSachKhiTra);
            string extra = string.Equals(tinhTrangSachKhiTra, "Tốt", StringComparison.OrdinalIgnoreCase)
                ? ""
                : $" Ghi nhận tình trạng sách: {tinhTrangSachKhiTra}.";
            SendNotificationToReader(
                record.MaDocGia,
                "Xác nhận đã trả sách",
                $"Thư viện đã xác nhận bạn trả sách \"{record.TenSach}\" (phiếu {record.MaMuon}).{extra}",
                "Thủ thư");

            return (true, $"Trả sách \"{record.TenSach}\" thành công.", record);
        }

        /// <summary>Thủ thư tiếp nhận trả: cập nhật phiếu hoàn thành, tình trạng quyển (Tốt → kho; Hỏng/Mất → ghi nhận).</summary>
        public static void CompleteReturn(BorrowRecord record, DateTime ngayTra, string tinhTrangSachKhiTra = "Tốt")
        {
            record.NgayTraThuc = ngayTra;
            record.TrangThai = "Đã trả";
            record.TienPhat = CalculateLateFee(record, ngayTra);
            record.TinhTrangSachKhiTra = tinhTrangSachKhiTra ?? "Tốt";

            if (!string.IsNullOrWhiteSpace(record.MaQuyenSach))
            {
                var copy = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == record.MaQuyenSach);
                if (copy != null && copy.TrangThai == "Đang mượn")
                {
                    string tt = record.TinhTrangSachKhiTra.Trim();
                    if (string.Equals(tt, "Mất", StringComparison.OrdinalIgnoreCase))
                        copy.TrangThai = "Mất";
                    else if (string.Equals(tt, "Hỏng", StringComparison.OrdinalIgnoreCase))
                        copy.TrangThai = "Hỏng";
                    else
                        copy.TrangThai = "Có sẵn";
                }
            }

            RecalculateBookInventoryFromCopies(record.MaSach);
        }

        /// <summary>Thủ thư lập phiếu mượn trực tiếp tại quầy (không qua yêu cầu online).</summary>
        public static (bool Success, string Message, string? MaMuon) CreateBorrowSlipDirect(
            string maDocGia,
            string maSach,
            DateTime ngayMuon,
            int soNgayMuon,
            string maQuyenSach = "")
        {
            var reader = SampleData.Readers.FirstOrDefault(r => r.MaDocGia == maDocGia);
            if (reader == null)
                return (false, "Không tìm thấy độc giả.", null);

            if (soNgayMuon <= 0)
                return (false, "Số ngày mượn không hợp lệ.", null);

            var br = BorrowBook(maSach, maDocGia, reader.HoTen, ngayMuon, soNgayMuon, maQuyenSach, "");
            if (!br.Success)
                return (false, br.Message, null);

            string maMuon = SampleData.BorrowRecords.Last().MaMuon;
            return (true, $"Đã lập phiếu mượn {maMuon}.", maMuon);
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
                LoaiThongBao = InferNotificationType(tieuDe, noiDung),
                TieuDe = tieuDe.Trim(),
                NoiDung = noiDung.Trim(),
                DaDoc = false
            };
            UserStore.Notifications.Add(notification);
            return notification;
        }

        public static IReadOnlyList<BorrowRecord> GetOverdueRecords(string maDocGia = "", int minDays = 0)
        {
            var data = SampleData.BorrowRecords
                .Where(r => r.TrangThai == "Đang mượn")
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(maDocGia))
                data = data.Where(r => r.MaDocGia.Contains(maDocGia.Trim(), StringComparison.OrdinalIgnoreCase));

            if (minDays > 0)
                data = data.Where(r => r.SoNgayQuaHan >= minDays);
            else
                data = data.Where(r => r.IsOverdue);

            return data.OrderByDescending(r => r.SoNgayQuaHan).ToList();
        }

        public static (int SentCount, List<string> ReaderCodes) RunDueSoonAndOverdueNotificationJob(int dueSoonDays = 2)
        {
            var sentReaderCodes = new List<string>();
            var now = DateTime.Now.Date;
            foreach (var record in SampleData.BorrowRecords.Where(r => r.TrangThai == "Đang mượn"))
            {
                int daysToDue = (record.NgayHenTra.Date - now).Days;
                bool dueSoon = daysToDue >= 0 && daysToDue <= dueSoonDays;
                bool overdue = now > record.NgayHenTra.Date;
                if (!dueSoon && !overdue)
                    continue;

                string duplicateTitle = overdue ? "Thông báo quá hạn" : "Nhắc nhở sắp đến hạn trả";
                bool duplicated = UserStore.Notifications.Any(n =>
                    n.MaDocGia == record.MaDocGia &&
                    n.TieuDe == duplicateTitle &&
                    n.NoiDung.Contains(record.MaMuon, StringComparison.OrdinalIgnoreCase) &&
                    n.ThoiGian.Date == now);
                if (duplicated)
                    continue;

                string content = overdue
                    ? $"Phiếu {record.MaMuon} cho sách \"{record.TenSach}\" đã quá hạn {record.SoNgayQuaHan} ngày. Vui lòng trả sách và hoàn tất nghĩa vụ phạt (nếu có)."
                    : $"Phiếu {record.MaMuon} cho sách \"{record.TenSach}\" sẽ đến hạn vào {record.NgayHenTra:dd/MM/yyyy}.";
                SendNotificationToReader(record.MaDocGia, duplicateTitle, content, "Hệ thống");
                if (!sentReaderCodes.Contains(record.MaDocGia))
                    sentReaderCodes.Add(record.MaDocGia);
            }

            return (sentReaderCodes.Count, sentReaderCodes);
        }

        public static (bool Success, string Message, InventorySession? Session) StartInventorySession(string tenDot, string nguoiTao, string ghiChu = "")
        {
            string normalizedName = tenDot?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(normalizedName))
                return (false, "Tên đợt kiểm kê không được để trống.", null);

            var session = new InventorySession
            {
                MaDotKiemKe = GenerateInventorySessionCode(),
                TenDot = normalizedName,
                ThoiGianTao = DateTime.Now,
                NguoiTao = string.IsNullOrWhiteSpace(nguoiTao) ? "Thủ thư" : nguoiTao.Trim(),
                GhiChu = ghiChu?.Trim() ?? ""
            };

            session.Items = SampleData.BookCopies
                .OrderBy(c => c.MaQuyenSach)
                .Select(c => new InventoryCheckItem
                {
                    MaQuyenSach = c.MaQuyenSach,
                    MaSach = c.MaSach,
                    TenSach = GetBookName(c.MaSach),
                    TrangThaiHeThong = c.TrangThai,
                    TrangThaiThucTe = c.TrangThai
                })
                .ToList();

            SampleData.InventorySessions.Add(session);
            return (true, $"Đã tạo đợt kiểm kê {session.MaDotKiemKe}.", session);
        }

        public static InventorySession? GetLatestInventorySession()
        {
            return SampleData.InventorySessions
                .OrderByDescending(s => s.ThoiGianTao)
                .FirstOrDefault();
        }

        public static (bool Success, string Message) UpdateInventoryItem(string maDotKiemKe, string maQuyenSach, string trangThaiThucTe, string ghiChu = "")
        {
            var session = SampleData.InventorySessions.FirstOrDefault(s => s.MaDotKiemKe == maDotKiemKe);
            if (session == null)
                return (false, "Không tìm thấy đợt kiểm kê.");

            var item = session.Items.FirstOrDefault(i => i.MaQuyenSach == maQuyenSach);
            if (item == null)
                return (false, "Không tìm thấy quyển sách trong đợt kiểm kê.");

            item.TrangThaiThucTe = string.IsNullOrWhiteSpace(trangThaiThucTe) ? item.TrangThaiHeThong : trangThaiThucTe.Trim();
            item.GhiChu = ghiChu?.Trim() ?? "";
            return (true, "Đã cập nhật kiểm kê cho quyển sách.");
        }

        public static string ExportInventoryReportToCsv(InventorySession session, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaDotKiemKe,TenDot,ThoiGianTao,NguoiTao,MaQuyenSach,MaSach,TenSach,TrangThaiHeThong,TrangThaiThucTe,ChenhLech,GhiChu");
            foreach (var item in session.Items)
            {
                sb.AppendLine(string.Join(",",
                    EscapeCsv(session.MaDotKiemKe),
                    EscapeCsv(session.TenDot),
                    EscapeCsv(session.ThoiGianTao.ToString("yyyy-MM-dd HH:mm:ss")),
                    EscapeCsv(session.NguoiTao),
                    EscapeCsv(item.MaQuyenSach),
                    EscapeCsv(item.MaSach),
                    EscapeCsv(item.TenSach),
                    EscapeCsv(item.TrangThaiHeThong),
                    EscapeCsv(item.TrangThaiThucTe),
                    EscapeCsv(item.ChenhLech ? "Co" : "Khong"),
                    EscapeCsv(item.GhiChu)));
            }
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            return outputPath;
        }

        public static string GetSetting(string key, string fallback = "")
        {
            return UserStore.SystemSettings.FirstOrDefault(s => s.SettingKey == key)?.SettingValue ?? fallback;
        }

        public static void SetSetting(string key, string value)
        {
            var item = UserStore.SystemSettings.FirstOrDefault(s => s.SettingKey == key);
            if (item == null)
                UserStore.SystemSettings.Add(new SystemSetting { SettingKey = key, SettingValue = value });
            else
                item.SettingValue = value;
            UserStore.AddLog(UserStore.CurrentUser?.HoTen ?? "Hệ thống", "Cập nhật cấu hình", $"{key}={value}", "System");
        }

        public static bool GetFeatureToggle(string key, bool fallback = true)
        {
            return UserStore.FeatureToggles.FirstOrDefault(t => t.FeatureKey == key)?.Enabled ?? fallback;
        }

        public static void SetFeatureToggle(string key, bool enabled)
        {
            var item = UserStore.FeatureToggles.FirstOrDefault(t => t.FeatureKey == key);
            if (item == null)
                UserStore.FeatureToggles.Add(new FeatureToggle { FeatureKey = key, Enabled = enabled });
            else
                item.Enabled = enabled;
            UserStore.AddLog(UserStore.CurrentUser?.HoTen ?? "Hệ thống", "Cập nhật toggle", $"{key}={(enabled ? "ON" : "OFF")}", "System");
        }

        public static string GetNotificationTemplate(string key, string fallback = "")
        {
            return UserStore.NotificationTemplates.FirstOrDefault(t => t.TemplateKey == key)?.Content ?? fallback;
        }

        public static void SetNotificationTemplate(string key, bool enabled, string content)
        {
            var item = UserStore.NotificationTemplates.FirstOrDefault(t => t.TemplateKey == key);
            if (item == null)
                UserStore.NotificationTemplates.Add(new NotificationTemplate { TemplateKey = key, Enabled = enabled, Content = content });
            else
            {
                item.Enabled = enabled;
                item.Content = content;
            }
        }

        public static IReadOnlyList<RolePermission> GetRolePermissions(UserRole role)
        {
            return UserStore.RolePermissions.Where(r => r.Role == role.ToString()).OrderBy(r => r.Module).ThenBy(r => r.Action).ToList();
        }

        public static void SetRolePermission(UserRole role, string module, string action, bool allowed)
        {
            var item = UserStore.RolePermissions.FirstOrDefault(r => r.Role == role.ToString() && r.Module == module && r.Action == action);
            if (item == null)
                UserStore.RolePermissions.Add(new RolePermission { Role = role.ToString(), Module = module, Action = action, Allowed = allowed });
            else
                item.Allowed = allowed;
            UserStore.AddLog(UserStore.CurrentUser?.HoTen ?? "Hệ thống", "Phân quyền", $"{role}:{module}:{action}={(allowed ? "Allow" : "Deny")}", "Security");
        }

        public static object GetSystemStatistics()
        {
            var records = SampleData.BorrowRecords;
            var books = SampleData.Books;
            return new
            {
                TongSoSach = SampleData.BookCopies.Count,
                TongDauSach = books.Count,
                TongDanhMuc = SampleData.BookCategories.Count,
                TongNguoiDung = UserStore.Users.Count,
                DangMuon = records.Count(r => r.TrangThai == "Đang mượn"),
                DaTra = records.Count(r => r.TrangThai == "Đã trả"),
                QuaHan = records.Count(r => r.IsOverdue),
                TongTienPhat = records.Sum(r => r.TienPhat > 0 ? r.TienPhat : CalculateLateFee(r)),
                ChuaNopPhat = records.Count(r => CalculateLateFee(r) > 0 && !r.DaThuPhat)
            };
        }

        public static IReadOnlyList<(string Label, int Value)> GetBorrowStatsByTime(string period)
        {
            var records = SampleData.BorrowRecords;
            IEnumerable<IGrouping<string, BorrowRecord>> groups = period switch
            {
                "month" => records.GroupBy(r => $"{r.NgayMuon:yyyy-MM}"),
                "year" => records.GroupBy(r => $"{r.NgayMuon:yyyy}"),
                _ => records.GroupBy(r => $"{r.NgayMuon:yyyy-MM-dd}")
            };
            return groups.OrderBy(g => g.Key).Select(g => (g.Key, g.Count())).ToList();
        }

        public static IReadOnlyList<(string Category, int Count)> GetBookStatsByCategory()
        {
            return SampleData.Books
                .GroupBy(b => GetCategoryName(b.MaDanhMuc, b.TheLoai))
                .Select(g => (g.Key, g.Sum(x => x.SoLuong)))
                .OrderByDescending(g => g.Item2)
                .ToList();
        }

        public static IReadOnlyList<(string Reader, int BorrowCount, int ViolationCount)> GetReaderStats()
        {
            return SampleData.BorrowRecords
                .GroupBy(r => r.MaDocGia + " - " + r.TenDocGia)
                .Select(g => (
                    g.Key,
                    g.Count(),
                    g.Count(x => x.IsOverdue || x.TienPhat > 0)))
                .OrderByDescending(x => x.Item2)
                .ToList();
        }

        public static IReadOnlyList<(string Book, int BorrowCount)> GetBookBorrowRanking(bool descending)
        {
            var ranking = SampleData.BorrowRecords
                .GroupBy(r => r.TenSach)
                .Select(g => (g.Key, g.Count()));
            return (descending
                    ? ranking.OrderByDescending(x => x.Item2)
                    : ranking.OrderBy(x => x.Item2))
                .ToList();
        }

        public static (int DangMuon, int DaTra, bool CoQuaHan, bool DaNopPhatDayDu) GetReaderBorrowOverview(string maDocGia)
        {
            var records = SampleData.BorrowRecords.Where(r => r.MaDocGia == maDocGia).ToList();
            int dangMuon = records.Count(r => r.TrangThai == "Đang mượn");
            int daTra = records.Count(r => r.TrangThai == "Đã trả");
            bool quaHan = records.Any(r => r.IsOverdue);
            bool daNopDayDu = records.Where(r => CalculateLateFee(r) > 0).All(r => r.DaThuPhat);
            return (dangMuon, daTra, quaHan, daNopDayDu);
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

        public static string GenerateInventorySessionCode()
        {
            int max = SampleData.InventorySessions
                .Select(s => s.MaDotKiemKe)
                .Where(code => code.StartsWith("KK", StringComparison.OrdinalIgnoreCase))
                .Select(code => int.TryParse(code.Substring(2), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();
            return "KK" + (max + 1).ToString("D3");
        }

        private static string InferNotificationType(string tieuDe, string noiDung)
        {
            string text = (tieuDe + " " + noiDung).ToLowerInvariant();
            if (text.Contains("quá hạn") || text.Contains("phạt"))
                return "QuaHan";
            if (text.Contains("đến hạn") || text.Contains("nhắc"))
                return "NhacHan";
            if (text.Contains("duyệt") || text.Contains("từ chối"))
                return "MuonTra";
            return "HeThong";
        }

        private static string EscapeCsv(string value)
        {
            string v = value ?? "";
            if (v.Contains('"'))
                v = v.Replace("\"", "\"\"");
            return v.Contains(',') || v.Contains('"') || v.Contains('\n')
                ? $"\"{v}\""
                : v;
        }
    }
}
