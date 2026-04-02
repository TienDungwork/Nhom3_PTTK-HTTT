using System;
using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class InventorySession
    {
        public string MaDotKiemKe { get; set; } = "";
        public string TenDot { get; set; } = "";
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;
        public string NguoiTao { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public List<InventoryCheckItem> Items { get; set; } = new List<InventoryCheckItem>();
    }

    public class InventoryCheckItem
    {
        public string MaQuyenSach { get; set; } = "";
        public string MaSach { get; set; } = "";
        public string TenSach { get; set; } = "";
        public string TrangThaiHeThong { get; set; } = "";
        public string TrangThaiThucTe { get; set; } = "";
        public string GhiChu { get; set; } = "";
        public bool ChenhLech =>
            !string.Equals(TrangThaiHeThong, TrangThaiThucTe, StringComparison.OrdinalIgnoreCase);
    }
}
