using System;
using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public static class SampleData
    {
        public static List<Book> Books = new List<Book>
        {
            new Book { MaSach = "S001", TenSach = "Lập trình C# cơ bản", TacGia = "Nguyễn Văn A", ChuDe = "Lập trình", NamXuatBan = 2022, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s001", ISBN = "978-604-1-00001-0", BoSuuTap = "CNTT", TheLoai = "Công nghệ", SoLuong = 5, SoLuongDangMuon = 2, ViTriKho = "A1-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S002", TenSach = "Cấu trúc dữ liệu và giải thuật", TacGia = "Trần Thị B", ChuDe = "Khoa học máy tính", NamXuatBan = 2021, NhaXuatBan = "NXB ĐHQG", URI = "https://lib.tlu.edu.vn/s002", ISBN = "978-604-1-00002-7", BoSuuTap = "CNTT", TheLoai = "Công nghệ", SoLuong = 3, SoLuongDangMuon = 1, ViTriKho = "A1-02", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S003", TenSach = "Trí tuệ nhân tạo", TacGia = "Lê Hoàng C", ChuDe = "AI", NamXuatBan = 2023, NhaXuatBan = "NXB Bách khoa", URI = "https://lib.tlu.edu.vn/s003", ISBN = "978-604-1-00003-4", BoSuuTap = "CNTT", TheLoai = "Công nghệ", SoLuong = 2, SoLuongDangMuon = 1, ViTriKho = "A1-03", NhaCungCap = "Tiki" },
            new Book { MaSach = "S004", TenSach = "Văn học Việt Nam hiện đại", TacGia = "Phạm Minh D", ChuDe = "Văn học", NamXuatBan = 2020, NhaXuatBan = "NXB Văn học", URI = "https://lib.tlu.edu.vn/s004", ISBN = "978-604-1-00004-1", BoSuuTap = "Văn học", TheLoai = "Văn học", SoLuong = 4, ViTriKho = "B2-01", NhaCungCap = "Nhà sách Thành Nghĩa" },
            new Book { MaSach = "S005", TenSach = "Lịch sử thế giới cận đại", TacGia = "Võ Thanh E", ChuDe = "Lịch sử", NamXuatBan = 2019, NhaXuatBan = "NXB Chính trị", URI = "https://lib.tlu.edu.vn/s005", ISBN = "978-604-1-00005-8", BoSuuTap = "Lịch sử", TheLoai = "Lịch sử", SoLuong = 6, SoLuongDangMuon = 1, ViTriKho = "B3-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S006", TenSach = "Kinh tế vi mô", TacGia = "Hoàng Thị F", ChuDe = "Kinh tế", NamXuatBan = 2021, NhaXuatBan = "NXB Tài chính", URI = "https://lib.tlu.edu.vn/s006", ISBN = "978-604-1-00006-5", BoSuuTap = "Kinh tế", TheLoai = "Kinh tế", SoLuong = 0, ViTriKho = "C1-01", NhaCungCap = "Tiki", TrangThai = "Hết sách" },
            new Book { MaSach = "S007", TenSach = "Tâm lý học đại cương", TacGia = "Đặng Văn G", ChuDe = "Tâm lý", NamXuatBan = 2022, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s007", ISBN = "978-604-1-00007-2", BoSuuTap = "Xã hội", TheLoai = "Tâm lý", SoLuong = 3, SoLuongDangMuon = 1, ViTriKho = "C2-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S008", TenSach = "Thiết kế cơ sở dữ liệu", TacGia = "Nguyễn Thị H", ChuDe = "Cơ sở dữ liệu", NamXuatBan = 2023, NhaXuatBan = "NXB ĐHQG", URI = "https://lib.tlu.edu.vn/s008", ISBN = "978-604-1-00008-9", BoSuuTap = "CNTT", TheLoai = "Công nghệ", SoLuong = 0, SoLuongMatHong = 1, ViTriKho = "A1-04", NhaCungCap = "Tiki", TrangThai = "Hết sách" },
            new Book { MaSach = "S009", TenSach = "Toán cao cấp", TacGia = "Trần Văn I", ChuDe = "Toán học", NamXuatBan = 2020, NhaXuatBan = "NXB Giáo dục", URI = "https://lib.tlu.edu.vn/s009", ISBN = "978-604-1-00009-6", BoSuuTap = "Khoa học", TheLoai = "Toán học", SoLuong = 7, ViTriKho = "D1-01", NhaCungCap = "Fahasa" },
            new Book { MaSach = "S010", TenSach = "Triết học Mác-Lênin", TacGia = "Lê Thị K", ChuDe = "Triết học", NamXuatBan = 2021, NhaXuatBan = "NXB Chính trị", URI = "https://lib.tlu.edu.vn/s010", ISBN = "978-604-1-00010-2", BoSuuTap = "Chính trị", TheLoai = "Triết học", SoLuong = 5, ViTriKho = "D2-01", NhaCungCap = "Nhà sách Thành Nghĩa" },
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
            new BorrowRecord { MaMuon = "M001", MaDocGia = "DG001", TenDocGia = "Nguyễn Văn Minh", MaSach = "S001", TenSach = "Lập trình C# cơ bản", NgayMuon = DateTime.Now.AddDays(-20), NgayHenTra = DateTime.Now.AddDays(-6), TrangThai = "Đang mượn", TienPhat = 30000 },
            new BorrowRecord { MaMuon = "M002", MaDocGia = "DG002", TenDocGia = "Trần Thị Lan", MaSach = "S003", TenSach = "Trí tuệ nhân tạo", NgayMuon = DateTime.Now.AddDays(-10), NgayHenTra = DateTime.Now.AddDays(4), TrangThai = "Đang mượn" },
            new BorrowRecord { MaMuon = "M003", MaDocGia = "DG003", TenDocGia = "Lê Hoàng Nam", MaSach = "S005", TenSach = "Lịch sử thế giới cận đại", NgayMuon = DateTime.Now.AddDays(-30), NgayHenTra = DateTime.Now.AddDays(-16), TrangThai = "Đang mượn", TienPhat = 80000 },
            new BorrowRecord { MaMuon = "M004", MaDocGia = "DG004", TenDocGia = "Phạm Thị Hoa", MaSach = "S002", TenSach = "Cấu trúc dữ liệu và giải thuật", NgayMuon = DateTime.Now.AddDays(-5), NgayHenTra = DateTime.Now.AddDays(9), TrangThai = "Đang mượn" },
            new BorrowRecord { MaMuon = "M005", MaDocGia = "DG001", TenDocGia = "Nguyễn Văn Minh", MaSach = "S004", TenSach = "Văn học Việt Nam hiện đại", NgayMuon = DateTime.Now.AddDays(-40), NgayHenTra = DateTime.Now.AddDays(-26), NgayTraThuc = DateTime.Now.AddDays(-20), TrangThai = "Đã trả" },
            new BorrowRecord { MaMuon = "M006", MaDocGia = "DG005", TenDocGia = "Võ Thanh Tùng", MaSach = "S007", TenSach = "Tâm lý học đại cương", NgayMuon = DateTime.Now.AddDays(-8), NgayHenTra = DateTime.Now.AddDays(6), TrangThai = "Đang mượn" },
        };

        public static List<string> Categories = new List<string>
        {
            "Tất cả", "Công nghệ", "Văn học", "Lịch sử", "Kinh tế", "Tâm lý", "Toán học", "Triết học", "Khoa học", "Ngoại ngữ"
        };
    }
}
