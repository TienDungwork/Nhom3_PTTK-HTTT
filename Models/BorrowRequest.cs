using System;

namespace LibraryManagement.Models
{
    public class BorrowRequest
    {
        public string MaYeuCau { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string TenDocGia { get; set; } = "";
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string MaQuyenSachYeuCau { get; set; } = "";
        public DateTime NgayMuonDuKien { get; set; } = DateTime.Now;
        public int SoNgayMuon { get; set; } = 14;
        public DateTime NgayTaoYeuCau { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đã duyệt, Từ chối
        public string NguoiDuyet { get; set; } = "";
        public string LyDoTuChoi { get; set; } = "";
        public string MaMuon { get; set; } = "";
    }
}
