using System;

namespace LibraryManagement.Models
{
    public class Reader
    {
        public string MaDocGia { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string SDT { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
    }
}
