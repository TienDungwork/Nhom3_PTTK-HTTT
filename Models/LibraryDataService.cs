using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IReadOnlyList<BookLot> GetLotsForBook(string maSach, bool includeZero = true)
        {
            var query = SampleData.BookLots.Where(l => l.MaSach == maSach);
            if (!includeZero) query = query.Where(l => l.SoLuongCon > 0);
            return query.OrderBy(l => l.NgayNhap).ThenBy(l => l.MaLo).ToList();
        }

        public static IReadOnlyList<BookLot> SearchLots(string keyword = "", string maSach = "")
        {
            var query = SampleData.BookLots.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(maSach))
                query = query.Where(l => l.MaSach == maSach);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(l =>
                    l.MaLo.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    l.MaSach.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    GetBookName(l.MaSach).Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    l.NhaCungCap.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    l.TinhTrang.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderByDescending(l => l.NgayNhap).ThenBy(l => l.MaLo).ToList();
        }

        public static string GetBookName(string maSach)
        {
            return SampleData.Books.FirstOrDefault(b => b.MaSach == maSach)?.TenSach ?? "";
        }

        public static void NormalizeData()
        {
            foreach (var book in SampleData.Books)
            {
                SyncBookCategory(book);
            }

            EnsureLotsForAllBooks();

            foreach (var book in SampleData.Books)
            {
                RecalculateBookInventoryFromLots(book.MaSach);
            }
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

        public static void RecalculateBookInventoryFromLots(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return;

            var lots = SampleData.BookLots.Where(l => l.MaSach == maSach).ToList();
            int total = lots.Sum(l => l.SoLuongNhap);
            int conLai = lots.Sum(l => l.SoLuongCon);
            int dangMuon = SampleData.BorrowRecords.Count(r => r.MaSach == maSach && r.TrangThai == "Đang mượn");
            int matHong = Math.Max(0, total - conLai - dangMuon);

            book.SoLuong = total;
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

            SyncBookCategory(book);

            var existing = SampleData.Books.FirstOrDefault(b => b.MaSach == book.MaSach);
            if (!isEditMode)
            {
                if (existing != null)
                    return (false, "Mã sách đã tồn tại.");

                book.SoLuongDangMuon = 0;
                book.SoLuongMatHong = 0;
                SyncBookStatus(book);
                SampleData.Books.Add(book);

                if (book.SoLuong > 0)
                {
                    SampleData.BookLots.Add(new BookLot
                    {
                        MaLo = GenerateLotCode(),
                        MaSach = book.MaSach,
                        NgayNhap = DateTime.Today,
                        SoLuongNhap = book.SoLuong,
                        SoLuongCon = book.SoLuong,
                        TinhTrang = "Mới",
                        NhaCungCap = book.NhaCungCap
                    });
                    RecalculateBookInventoryFromLots(book.MaSach);
                }

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
            RecalculateBookInventoryFromLots(existing.MaSach);

            return (true, "Cập nhật đầu sách thành công.");
        }

        public static (bool Success, string Message) DeleteBook(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.");

            if (SampleData.BorrowRecords.Any(r => r.MaSach == maSach && r.TrangThai == "Đang mượn"))
                return (false, "Không thể xóa đầu sách đang có bản sách được mượn.");

            if (SampleData.BookLots.Any(l => l.MaSach == maSach && l.SoLuongDaXuat > 0))
                return (false, "Không thể xóa đầu sách đã phát sinh giao dịch theo lô.");

            SampleData.BookLots.RemoveAll(l => l.MaSach == maSach);
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

        public static (bool Success, string Message) SaveLot(BookLot lot, bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(lot.MaLo) || string.IsNullOrWhiteSpace(lot.MaSach))
                return (false, "Mã lô và mã sách là bắt buộc.");
            if (lot.SoLuongNhap < 0 || lot.SoLuongCon < 0)
                return (false, "Số lượng không hợp lệ.");
            if (lot.SoLuongCon > lot.SoLuongNhap)
                return (false, "Số lượng còn không được lớn hơn số lượng nhập.");
            if (SampleData.Books.FirstOrDefault(b => b.MaSach == lot.MaSach) == null)
                return (false, "Mã sách không tồn tại.");

            var existing = SampleData.BookLots.FirstOrDefault(l => l.MaLo == lot.MaLo);
            if (!isEditMode)
            {
                if (existing != null)
                    return (false, "Mã lô đã tồn tại.");
                if (SampleData.Books.FirstOrDefault(b => b.MaSach == lot.MaSach) == null)
                    return (false, "Mã sách không tồn tại.");

                SampleData.BookLots.Add(lot);
                RecalculateBookInventoryFromLots(lot.MaSach);
                return (true, "Thêm lô nhập thành công.");
            }

            if (existing == null)
                return (false, "Không tìm thấy lô nhập.");

            int daXuatCu = existing.SoLuongDaXuat;
            if (lot.SoLuongNhap < daXuatCu)
                return (false, "Số lượng nhập không thể nhỏ hơn số đã xuất của lô.");
            if (lot.SoLuongCon > lot.SoLuongNhap)
                return (false, "Số lượng còn không hợp lệ.");

            existing.NgayNhap = lot.NgayNhap;
            existing.TinhTrang = lot.TinhTrang;
            existing.NhaCungCap = lot.NhaCungCap;
            existing.GhiChu = lot.GhiChu;
            existing.SoLuongNhap = lot.SoLuongNhap;
            existing.SoLuongCon = lot.SoLuongCon;
            RecalculateBookInventoryFromLots(existing.MaSach);

            return (true, "Cập nhật lô nhập thành công.");
        }

        public static (bool Success, string Message) DeleteLot(string maLo)
        {
            var lot = SampleData.BookLots.FirstOrDefault(l => l.MaLo == maLo);
            if (lot == null)
                return (false, "Không tìm thấy lô nhập.");
            if (lot.SoLuongDaXuat > 0)
                return (false, "Không thể xóa lô đã phát sinh mượn/trả.");

            SampleData.BookLots.Remove(lot);
            RecalculateBookInventoryFromLots(lot.MaSach);
            return (true, $"Đã xóa lô {lot.MaLo}.");
        }

        public static (bool Success, string Message, string LotCode) BorrowBook(
            string maSach,
            string maDocGia,
            string tenDocGia,
            DateTime ngayMuon,
            int soNgayMuon,
            string maLo = "")
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.", "");

            var lot = PickLotForBorrow(maSach, maLo);
            if (lot == null)
                return (false, "Không còn bản sách khả dụng theo lô.", "");

            lot.SoLuongCon--;

            SampleData.BorrowRecords.Add(new BorrowRecord
            {
                MaMuon = "M" + (SampleData.BorrowRecords.Count + 1).ToString("D3"),
                MaDocGia = maDocGia,
                TenDocGia = tenDocGia,
                MaSach = maSach,
                TenSach = book.TenSach,
                MaLo = lot.MaLo,
                NgayMuon = ngayMuon,
                NgayHenTra = ngayMuon.AddDays(soNgayMuon),
                TrangThai = "Đang mượn"
            });

            RecalculateBookInventoryFromLots(maSach);
            return (true, $"Mượn sách \"{book.TenSach}\" thành công.", lot.MaLo);
        }

        public static (bool Success, string Message, BorrowRecord? Record) ReturnBook(string maSach, string maDocGia, DateTime ngayTra)
        {
            var record = SampleData.BorrowRecords
                .Where(r => r.MaSach == maSach && r.MaDocGia == maDocGia && r.TrangThai == "Đang mượn")
                .OrderByDescending(r => r.NgayMuon)
                .FirstOrDefault();

            if (record == null)
                return (false, "Không tìm thấy phiếu mượn phù hợp.", null);

            CompleteReturn(record, ngayTra);
            return (true, $"Trả sách \"{record.TenSach}\" thành công.", record);
        }

        public static void CompleteReturn(BorrowRecord record, DateTime ngayTra)
        {
            record.NgayTraThuc = ngayTra;
            record.TrangThai = "Đã trả";
            record.TienPhat = CalculateLateFee(record, ngayTra);

            if (!string.IsNullOrWhiteSpace(record.MaLo))
            {
                var lot = SampleData.BookLots.FirstOrDefault(l => l.MaLo == record.MaLo);
                if (lot != null && lot.SoLuongCon < lot.SoLuongNhap)
                    lot.SoLuongCon++;
            }
            else
            {
                var fallbackLot = SampleData.BookLots
                    .Where(l => l.MaSach == record.MaSach)
                    .OrderBy(l => l.NgayNhap)
                    .FirstOrDefault();
                if (fallbackLot != null && fallbackLot.SoLuongCon < fallbackLot.SoLuongNhap)
                    fallbackLot.SoLuongCon++;
            }

            RecalculateBookInventoryFromLots(record.MaSach);
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

        private static void EnsureLotsForAllBooks()
        {
            foreach (var book in SampleData.Books)
            {
                bool hasLot = SampleData.BookLots.Any(l => l.MaSach == book.MaSach);
                if (hasLot) continue;

                SampleData.BookLots.Add(new BookLot
                {
                    MaLo = GenerateLotCode(),
                    MaSach = book.MaSach,
                    NgayNhap = DateTime.Today,
                    SoLuongNhap = Math.Max(0, book.SoLuong),
                    SoLuongCon = Math.Max(0, book.SoLuong - book.SoLuongDangMuon - book.SoLuongMatHong),
                    TinhTrang = "Mới",
                    NhaCungCap = book.NhaCungCap
                });
            }
        }

        private static BookLot? PickLotForBorrow(string maSach, string maLo)
        {
            if (!string.IsNullOrWhiteSpace(maLo))
            {
                var selected = SampleData.BookLots.FirstOrDefault(l => l.MaLo == maLo && l.MaSach == maSach);
                if (selected != null && selected.SoLuongCon > 0)
                    return selected;
                return null;
            }

            return SampleData.BookLots
                .Where(l => l.MaSach == maSach && l.SoLuongCon > 0)
                .OrderBy(l => l.NgayNhap)
                .ThenBy(l => l.MaLo)
                .FirstOrDefault();
        }

        public static string GenerateLotCode()
        {
            int max = SampleData.BookLots
                .Select(l => l.MaLo)
                .Where(code => code.StartsWith("LO", StringComparison.OrdinalIgnoreCase))
                .Select(code => int.TryParse(code.Substring(2), out int n) ? n : 0)
                .DefaultIfEmpty(0)
                .Max();

            return "LO" + (max + 1).ToString("D3");
        }
    }
}
