namespace LibraryManagement.Models
{
    public class Book
    {
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string TacGia { get; set; } = "";
        public string ChuDe { get; set; } = "";
        public int NamXuatBan { get; set; } = 2024;
        public string NhaXuatBan { get; set; } = "";
        public string URI { get; set; } = "";
        public string ISBN { get; set; } = "";
        public string BoSuuTap { get; set; } = "";
        public string AnhBia { get; set; } = ""; // path or empty for default
        public string TheLoai { get; set; } = "";
        public int SoLuong { get; set; }
        public int SoLuongDangMuon { get; set; }
        public int SoLuongMatHong { get; set; }
        public string ViTriKho { get; set; } = "";
        public string NhaCungCap { get; set; } = "";
        public string TrangThai { get; set; } = "Có sẵn";

        public int SoLuongHienCo => SoLuong - SoLuongDangMuon - SoLuongMatHong;
    }
}
