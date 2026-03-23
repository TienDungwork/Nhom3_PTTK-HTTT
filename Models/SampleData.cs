using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Models
{
    public static class SampleData
    {
        public static List<BookCategory> BookCategories = new List<BookCategory>
        {
            new BookCategory { MaDanhMuc = "DM001", TenDanhMuc = "Công nghệ", MoTa = "Sách công nghệ thông tin, lập trình và dữ liệu", ViTriKe = "Kệ A", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM002", TenDanhMuc = "Văn học", MoTa = "Tác phẩm văn học Việt Nam và thế giới", ViTriKe = "Kệ B", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM003", TenDanhMuc = "Lịch sử", MoTa = "Tài liệu lịch sử và nghiên cứu xã hội", ViTriKe = "Kệ B", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM004", TenDanhMuc = "Kinh tế", MoTa = "Giáo trình và tài liệu kinh tế", ViTriKe = "Kệ C", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM005", TenDanhMuc = "Tâm lý", MoTa = "Tâm lý học ứng dụng và đại cương", ViTriKe = "Kệ C", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM006", TenDanhMuc = "Toán học", MoTa = "Sách toán cơ bản và nâng cao", ViTriKe = "Kệ D", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM007", TenDanhMuc = "Triết học", MoTa = "Triết học, chính trị và lý luận", ViTriKe = "Kệ D", DangSuDung = true },
            new BookCategory { MaDanhMuc = "DM008", TenDanhMuc = "Ngoại ngữ", MoTa = "Tài liệu học ngoại ngữ", ViTriKe = "Kệ E", DangSuDung = false },
        };

        public static List<Book> Books = new List<Book>
        {
            new Book { MaSach = "S001", TenSach = "Lập trình C# cơ bản", TacGia = "Nguyễn Văn A", MaDanhMuc = "DM001", ChuDe = "Lập trình", NamXuatBan = 2022, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s001", ISBN = "978-604-1-00001-0", BoSuuTap = "CNTT", TheLoai = "Công nghệ", ViTriKho = "A1-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S002", TenSach = "Cấu trúc dữ liệu và giải thuật", TacGia = "Trần Thị B", MaDanhMuc = "DM001", ChuDe = "Khoa học máy tính", NamXuatBan = 2021, NhaXuatBan = "NXB ĐHQG", URI = "https://lib.tlu.edu.vn/s002", ISBN = "978-604-1-00002-7", BoSuuTap = "CNTT", TheLoai = "Công nghệ", ViTriKho = "A1-02", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S003", TenSach = "Trí tuệ nhân tạo", TacGia = "Lê Hoàng C", MaDanhMuc = "DM001", ChuDe = "AI", NamXuatBan = 2023, NhaXuatBan = "NXB Bách khoa", URI = "https://lib.tlu.edu.vn/s003", ISBN = "978-604-1-00003-4", BoSuuTap = "CNTT", TheLoai = "Công nghệ", ViTriKho = "A1-03", NhaCungCap = "Tiki" },
            new Book { MaSach = "S004", TenSach = "Văn học Việt Nam hiện đại", TacGia = "Phạm Minh D", MaDanhMuc = "DM002", ChuDe = "Văn học", NamXuatBan = 2020, NhaXuatBan = "NXB Văn học", URI = "https://lib.tlu.edu.vn/s004", ISBN = "978-604-1-00004-1", BoSuuTap = "Văn học", TheLoai = "Văn học", ViTriKho = "B2-01", NhaCungCap = "Nhà sách Thành Nghĩa" },
            new Book { MaSach = "S005", TenSach = "Lịch sử thế giới cận đại", TacGia = "Võ Thanh E", MaDanhMuc = "DM003", ChuDe = "Lịch sử", NamXuatBan = 2019, NhaXuatBan = "NXB Chính trị", URI = "https://lib.tlu.edu.vn/s005", ISBN = "978-604-1-00005-8", BoSuuTap = "Lịch sử", TheLoai = "Lịch sử", ViTriKho = "B3-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S006", TenSach = "Kinh tế vi mô", TacGia = "Hoàng Thị F", MaDanhMuc = "DM004", ChuDe = "Kinh tế", NamXuatBan = 2021, NhaXuatBan = "NXB Tài chính", URI = "https://lib.tlu.edu.vn/s006", ISBN = "978-604-1-00006-5", BoSuuTap = "Kinh tế", TheLoai = "Kinh tế", ViTriKho = "C1-01", NhaCungCap = "Tiki" },
            new Book { MaSach = "S007", TenSach = "Tâm lý học đại cương", TacGia = "Đặng Văn G", MaDanhMuc = "DM005", ChuDe = "Tâm lý", NamXuatBan = 2022, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s007", ISBN = "978-604-1-00007-2", BoSuuTap = "Xã hội", TheLoai = "Tâm lý", ViTriKho = "C2-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S008", TenSach = "Thiết kế cơ sở dữ liệu", TacGia = "Nguyễn Thị H", MaDanhMuc = "DM001", ChuDe = "Cơ sở dữ liệu", NamXuatBan = 2023, NhaXuatBan = "NXB ĐHQG", URI = "https://lib.tlu.edu.vn/s008", ISBN = "978-604-1-00008-9", BoSuuTap = "CNTT", TheLoai = "Công nghệ", ViTriKho = "A1-04", NhaCungCap = "Tiki" },
            new Book { MaSach = "S009", TenSach = "Toán cao cấp", TacGia = "Trần Văn I", MaDanhMuc = "DM006", ChuDe = "Toán học", NamXuatBan = 2020, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s009", ISBN = "978-604-1-00009-6", BoSuuTap = "Khoa học", TheLoai = "Toán học", ViTriKho = "D1-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S010", TenSach = "Triết học Mác-Lênin", TacGia = "Lê Thị K", MaDanhMuc = "DM007", ChuDe = "Triết học", NamXuatBan = 2021, NhaXuatBan = "NXB Chính trị", URI = "https://lib.tlu.edu.vn/s010", ISBN = "978-604-1-00010-2", BoSuuTap = "Chính trị", TheLoai = "Triết học", ViTriKho = "D2-01", NhaCungCap = "Nhà sách Thành Nghĩa" },
        };

        public static List<BookCopy> BookCopies = new List<BookCopy>
        {
            new BookCopy { MaQuyenSach = "Q001", MaSach = "S001", NgayNhap = new DateTime(2024, 1, 10), SoLuong = 1, TrangThai = "Đang mượn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q002", MaSach = "S001", NgayNhap = new DateTime(2024, 1, 10), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q003", MaSach = "S001", NgayNhap = new DateTime(2024, 6, 18), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q004", MaSach = "S002", NgayNhap = new DateTime(2024, 2, 5), SoLuong = 1, TrangThai = "Đang mượn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q005", MaSach = "S002", NgayNhap = new DateTime(2024, 2, 5), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q006", MaSach = "S002", NgayNhap = new DateTime(2024, 2, 5), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q007", MaSach = "S003", NgayNhap = new DateTime(2024, 3, 2), SoLuong = 1, TrangThai = "Đang mượn", NhaCungCap = "Tiki" },
            new BookCopy { MaQuyenSach = "Q008", MaSach = "S003", NgayNhap = new DateTime(2024, 3, 2), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Tiki" },
            new BookCopy { MaQuyenSach = "Q009", MaSach = "S004", NgayNhap = new DateTime(2023, 11, 11), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Nhà sách Thành Nghĩa" },
            new BookCopy { MaQuyenSach = "Q010", MaSach = "S004", NgayNhap = new DateTime(2023, 11, 11), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Nhà sách Thành Nghĩa" },
            new BookCopy { MaQuyenSach = "Q011", MaSach = "S005", NgayNhap = new DateTime(2023, 9, 21), SoLuong = 1, TrangThai = "Đang mượn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q012", MaSach = "S005", NgayNhap = new DateTime(2023, 9, 21), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q013", MaSach = "S006", NgayNhap = new DateTime(2024, 5, 14), SoLuong = 1, TrangThai = "Hỏng", NhaCungCap = "Tiki" },
            new BookCopy { MaQuyenSach = "Q014", MaSach = "S007", NgayNhap = new DateTime(2024, 4, 8), SoLuong = 1, TrangThai = "Đang mượn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q015", MaSach = "S007", NgayNhap = new DateTime(2024, 4, 8), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q016", MaSach = "S008", NgayNhap = new DateTime(2024, 7, 1), SoLuong = 1, TrangThai = "Mất", NhaCungCap = "Tiki" },
            new BookCopy { MaQuyenSach = "Q017", MaSach = "S009", NgayNhap = new DateTime(2023, 8, 20), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Fahasa" },
            new BookCopy { MaQuyenSach = "Q018", MaSach = "S010", NgayNhap = new DateTime(2024, 1, 28), SoLuong = 1, TrangThai = "Có sẵn", NhaCungCap = "Nhà sách Thành Nghĩa" },
        };

        public static List<Reader> Readers = new List<Reader>
        {
            new Reader { MaDocGia = "DG001", HoTen = "Nguyễn Văn Minh", Email = "minh@email.com", SDT = "0901234567", DiaChi = "Hà Nội", NgayDangKy = new DateTime(2024, 1, 15) },
            new Reader { MaDocGia = "DG002", HoTen = "Trần Thị Lan", Email = "lan@email.com", SDT = "0912345678", DiaChi = "TP.HCM", NgayDangKy = new DateTime(2024, 2, 20) },
            new Reader { MaDocGia = "DG003", HoTen = "Lê Hoàng Nam", Email = "nam@email.com", SDT = "0923456789", DiaChi = "Đà Nẵng", NgayDangKy = new DateTime(2024, 3, 10) },
            new Reader { MaDocGia = "DG004", HoTen = "Phạm Thị Hoa", Email = "hoa@email.com", SDT = "0934567890", DiaChi = "Huế", NgayDangKy = new DateTime(2024, 4, 5) },
            new Reader { MaDocGia = "DG005", HoTen = "Võ Thanh Tùng", Email = "tung@email.com", SDT = "0945678901", DiaChi = "Cần Thơ", NgayDangKy = new DateTime(2024, 5, 18) },
            new Reader { MaDocGia = "DG006", HoTen = "Hoàng Minh Tuấn", Email = "tuan@email.com", SDT = "0956789012", DiaChi = "Hải Phòng", NgayDangKy = new DateTime(2024, 6, 22) },
        };

        public static List<BorrowRecord> BorrowRecords = new List<BorrowRecord>
        {
            new BorrowRecord { MaMuon = "M001", MaDocGia = "DG001", TenDocGia = "Nguyễn Văn Minh", MaSach = "S001", MaQuyenSach = "Q001", TenSach = "Lập trình C# cơ bản", NgayMuon = DateTime.Now.AddDays(-20), NgayHenTra = DateTime.Now.AddDays(-6), TrangThai = "Đang mượn", TienPhat = 30000, DaThuPhat = false },
            new BorrowRecord { MaMuon = "M002", MaDocGia = "DG002", TenDocGia = "Trần Thị Lan", MaSach = "S003", MaQuyenSach = "Q007", TenSach = "Trí tuệ nhân tạo", NgayMuon = DateTime.Now.AddDays(-10), NgayHenTra = DateTime.Now.AddDays(4), TrangThai = "Đang mượn" },
            new BorrowRecord { MaMuon = "M003", MaDocGia = "DG003", TenDocGia = "Lê Hoàng Nam", MaSach = "S005", MaQuyenSach = "Q011", TenSach = "Lịch sử thế giới cận đại", NgayMuon = DateTime.Now.AddDays(-30), NgayHenTra = DateTime.Now.AddDays(-16), TrangThai = "Đang mượn", TienPhat = 80000, DaThuPhat = false },
            new BorrowRecord { MaMuon = "M004", MaDocGia = "DG004", TenDocGia = "Phạm Thị Hoa", MaSach = "S002", MaQuyenSach = "Q004", TenSach = "Cấu trúc dữ liệu và giải thuật", NgayMuon = DateTime.Now.AddDays(-5), NgayHenTra = DateTime.Now.AddDays(9), TrangThai = "Đang mượn" },
            new BorrowRecord { MaMuon = "M005", MaDocGia = "DG001", TenDocGia = "Nguyễn Văn Minh", MaSach = "S004", MaQuyenSach = "Q009", TenSach = "Văn học Việt Nam hiện đại", NgayMuon = DateTime.Now.AddDays(-40), NgayHenTra = DateTime.Now.AddDays(-26), NgayTraThuc = DateTime.Now.AddDays(-20), TrangThai = "Đã trả" },
            new BorrowRecord { MaMuon = "M006", MaDocGia = "DG005", TenDocGia = "Võ Thanh Tùng", MaSach = "S007", MaQuyenSach = "Q014", TenSach = "Tâm lý học đại cương", NgayMuon = DateTime.Now.AddDays(-8), NgayHenTra = DateTime.Now.AddDays(6), TrangThai = "Đang mượn" },
        };

        public static List<string> Categories =>
            new List<string> { "Tất cả" }
                .Concat(BookCategories.OrderBy(c => c.TenDanhMuc).Select(c => c.TenDanhMuc))
                .ToList();

        static SampleData()
        {
            LibraryDataService.NormalizeData();
        }
    }
}
