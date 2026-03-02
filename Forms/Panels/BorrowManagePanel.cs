using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class BorrowManagePanel : UserControl
    {
        private DataGridView dgv = null!;

        public BorrowManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "QUẢN LÝ MƯỢN TRẢ", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Xem và quản lý tất cả phiếu mượn trả sách", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Search bar
            var txtSearch = new TextBox { Location = new Point(32, 96), Size = new Size(300, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            txtSearch.PlaceholderText = "Tìm theo tên độc giả hoặc mã sách...";
            txtSearch.TextChanged += (s, e) => FilterRecords(txtSearch.Text);
            Controls.Add(txtSearch);

            // DGV
            dgv = new DataGridView { Location = new Point(32, 140), Size = new Size(920, 510), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("MaDocGia", "Mã ĐG");
            dgv.Columns.Add("TenDocGia", "Tên độc giả");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("NgayTra", "Ngày trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            ModernDataGridView.ApplyStyle(dgv);
            LoadRecords(SampleData.BorrowRecords);
            Controls.Add(dgv);
        }

        private void LoadRecords(System.Collections.Generic.List<BorrowRecord> records)
        {
            dgv.Rows.Clear();
            foreach (var r in records.OrderByDescending(r => r.NgayMuon))
            {
                string ngayTra = r.NgayTraThuc?.ToString("dd/MM/yyyy") ?? "—";
                string tienPhat = r.TienPhat > 0 ? $"{r.TienPhat:N0} VNĐ" : "—";
                int rowIdx = dgv.Rows.Add(r.MaMuon, r.MaDocGia, r.TenDocGia, r.TenSach, r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"), ngayTra, r.TrangThai, tienPhat);

                if (r.IsOverdue)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
                else if (r.TrangThai == "Đã trả")
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Success;
                }
            }
        }

        private void FilterRecords(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadRecords(SampleData.BorrowRecords);
                return;
            }
            var filtered = SampleData.BorrowRecords
                .Where(r => r.TenDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase) || r.MaSach.Contains(keyword, StringComparison.OrdinalIgnoreCase) || r.MaDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
            LoadRecords(filtered);
        }
    }
}
