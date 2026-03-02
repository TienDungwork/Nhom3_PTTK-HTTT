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
    public class BorrowHistoryPanel : UserControl
    {
        public BorrowHistoryPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "LỊCH SỬ MƯỢN SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Toàn bộ lịch sử mượn trả sách của bạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            var dgv = new DataGridView { Location = new Point(32, 96), Size = new Size(920, 560), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("NgayTra", "Ngày trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            ModernDataGridView.ApplyStyle(dgv);

            var cu = UserStore.CurrentUser;
            var records = SampleData.BorrowRecords
                .Where(r => r.MaDocGia == (cu?.Username ?? ""))
                .OrderByDescending(r => r.NgayMuon)
                .ToList();

            // If no records for this user, show all for demo
            if (records.Count == 0) records = SampleData.BorrowRecords.OrderByDescending(r => r.NgayMuon).ToList();

            foreach (var r in records)
            {
                string ngayTra = r.NgayTraThuc?.ToString("dd/MM/yyyy") ?? "—";
                string tienPhat = r.TienPhat > 0 ? $"{r.TienPhat:N0} VNĐ" : "—";
                int rowIdx = dgv.Rows.Add(r.MaMuon, r.TenSach, r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"), ngayTra, r.TrangThai, tienPhat);

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

            Controls.Add(dgv);
        }
    }
}
