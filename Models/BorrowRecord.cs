using System;

namespace LibraryManagement.Models
{
    public class BorrowRecord
    {
        public string MaMuon { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string TenDocGia { get; set; } = "";
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public DateTime NgayMuon { get; set; } = DateTime.Now;
        public DateTime NgayHenTra { get; set; } = DateTime.Now.AddDays(14);
        public DateTime? NgayTraThuc { get; set; }
        public string TrangThai { get; set; } = "Đang mượn";
        public decimal TienPhat { get; set; } = 0;

        public bool IsOverdue =>
            TrangThai == "Đang mượn" && DateTime.Now > NgayHenTra;
    }
}
