using System;

namespace LibraryManagement.Models
{
    public class BookLot
    {
        public string MaLo { get; set; } = "";
        public string MaSach { get; set; } = "";
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public int SoLuongNhap { get; set; }
        public int SoLuongCon { get; set; }
        public string TinhTrang { get; set; } = "Mới";
        public string NhaCungCap { get; set; } = "";
        public string GhiChu { get; set; } = "";

        public int SoLuongDaXuat => SoLuongNhap - SoLuongCon;
    }
}
