namespace LibraryManagement.Models
{
    public class Book
    {
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string TacGia { get; set; } = "";
        public string ISBN { get; set; } = "";
        public string TheLoai { get; set; } = "";
        public int SoLuong { get; set; }
        public string TrangThai { get; set; } = "Có sẵn";
    }
}
