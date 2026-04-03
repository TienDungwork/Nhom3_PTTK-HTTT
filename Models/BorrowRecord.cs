using System;

namespace LibraryManagement.Models
{
    public class BorrowRecord
    {
        public string MaMuon { get; set; } = "";
        public string MaYeuCau { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string TenDocGia { get; set; } = "";
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string MaQuyenSach { get; set; } = "";
        public int SoLuong { get; set; } = 1;
        public DateTime NgayMuon { get; set; } = DateTime.Now;
        public DateTime NgayHenTra { get; set; } = DateTime.Now.AddDays(14);
        public DateTime? NgayTraThuc { get; set; }
        public string TrangThai { get; set; } = "Đang mượn";
        public decimal TienPhat { get; set; } = 0;
        public bool DaThuPhat { get; set; } = false;

        /// <summary>Tình trạng quyển sách khi thủ thư tiếp nhận trả: Tốt, Hỏng, Mất (rỗng nếu chưa trả).</summary>
        public string TinhTrangSachKhiTra { get; set; } = "";

        public bool IsOverdue =>
            TrangThai == "Đang mượn" && DateTime.Now > NgayHenTra;

        public int SoNgayQuaHan =>
            IsOverdue ? (int)(DateTime.Now - NgayHenTra).TotalDays : 0;

        /// <summary>Trả muộn so với hạn (chỉ có nghĩa khi đã trả).</summary>
        public bool LaTraMuon =>
            TrangThai == "Đã trả" && NgayTraThuc.HasValue && NgayTraThuc.Value.Date > NgayHenTra.Date;
    }
}
