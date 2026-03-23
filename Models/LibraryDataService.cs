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

        public static void NormalizeData()
        {
            foreach (var book in SampleData.Books)
            {
                SyncBookCategory(book);
                SyncBookStatus(book);
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

        public static (bool Success, string Message) SaveBook(Book book, bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(book.MaSach) || string.IsNullOrWhiteSpace(book.TenSach))
                return (false, "Mã sách và tên sách là bắt buộc.");

            if (string.IsNullOrWhiteSpace(book.MaDanhMuc))
                return (false, "Vui lòng chọn danh mục sách.");

            SyncBookCategory(book);
            SyncBookStatus(book);

            var existing = SampleData.Books.FirstOrDefault(b => b.MaSach == book.MaSach);
            if (!isEditMode)
            {
                if (existing != null)
                    return (false, "Mã sách đã tồn tại.");

                SampleData.Books.Add(book);
                return (true, "Thêm đầu sách thành công.");
            }

            if (existing == null)
                return (false, "Không tìm thấy đầu sách cần cập nhật.");

            if (book.SoLuong < existing.SoLuongDangMuon + existing.SoLuongMatHong)
                return (false, "Tổng số lượng không được nhỏ hơn số sách đang mượn và mất/hỏng.");

            existing.TenSach = book.TenSach;
            existing.TacGia = book.TacGia;
            existing.MaDanhMuc = book.MaDanhMuc;
            existing.TheLoai = GetCategoryName(book.MaDanhMuc, book.TheLoai);
            existing.ChuDe = book.ChuDe;
            existing.NamXuatBan = book.NamXuatBan;
            existing.NhaXuatBan = book.NhaXuatBan;
            existing.URI = book.URI;
            existing.ISBN = book.ISBN;
            existing.SoLuong = book.SoLuong;
            existing.ViTriKho = book.ViTriKho;
            existing.NhaCungCap = book.NhaCungCap;
            SyncBookStatus(existing);
            return (true, "Cập nhật đầu sách thành công.");
        }

        public static (bool Success, string Message) DeleteBook(string maSach)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.");

            if (book.SoLuongDangMuon > 0)
                return (false, "Không thể xóa đầu sách đang có bản sách được mượn.");

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
            {
                book.TheLoai = duplicateCode.TenDanhMuc;
            }

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

        public static (bool Success, string Message) BorrowBook(string maSach, string maDocGia, string tenDocGia, DateTime ngayMuon, int soNgayMuon)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null)
                return (false, "Không tìm thấy đầu sách.");

            SyncBookStatus(book);
            if (book.SoLuongHienCo <= 0)
                return (false, "Đầu sách này hiện không còn bản khả dụng.");

            book.SoLuongDangMuon++;
            SyncBookStatus(book);

            SampleData.BorrowRecords.Add(new BorrowRecord
            {
                MaMuon = "M" + (SampleData.BorrowRecords.Count + 1).ToString("D3"),
                MaDocGia = maDocGia,
                TenDocGia = tenDocGia,
                MaSach = maSach,
                TenSach = book.TenSach,
                NgayMuon = ngayMuon,
                NgayHenTra = ngayMuon.AddDays(soNgayMuon),
                TrangThai = "Đang mượn"
            });

            return (true, $"Mượn sách \"{book.TenSach}\" thành công.");
        }

        public static (bool Success, string Message, BorrowRecord? Record) ReturnBook(string maSach, string maDocGia, DateTime ngayTra)
        {
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaSach == maSach && r.MaDocGia == maDocGia && r.TrangThai == "Đang mượn");
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

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == record.MaSach);
            if (book != null && book.SoLuongDangMuon > 0)
            {
                book.SoLuongDangMuon--;
                SyncBookStatus(book);
            }
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
    }
}
