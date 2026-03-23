using System;

namespace LibraryManagement.Models
{
    public class BookCopy
    {
        public string MaQuyenSach { get; set; } = "";
        public string MaSach { get; set; } = "";
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public int SoLuong { get; set; } = 1;
        public string TrangThai { get; set; } = "Có sẵn";
        public string NhaCungCap { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }
}
