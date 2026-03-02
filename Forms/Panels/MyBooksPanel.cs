using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class MyBooksPanel : UserControl
    {
        private DataGridView dgv = null!;

        public MyBooksPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "SÁCH ĐÃ MƯỢN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Danh sách sách bạn đang mượn và lịch sử", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Tabs (buttons)
            var btnCurrent = new RoundedButton { Text = "Đang mượn", Size = new Size(140, 38), Location = new Point(32, 96), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            var btnHistory = new RoundedButton { Text = "Lịch sử", Size = new Size(140, 38), Location = new Point(184, 96), ButtonColor = Color.Gray, Font = ThemeColors.ButtonFont, IsOutline = true };
            Controls.Add(btnCurrent); Controls.Add(btnHistory);

            dgv = new DataGridView { Location = new Point(32, 148), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, ReadOnly = true, AllowUserToAddRows = false };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("NgayTra", "Ngày trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            dgv.Columns["TenSach"]!.FillWeight = 200;

            bool showCurrent = true;

            btnCurrent.Click += (s, e) => { showCurrent = true; LoadData(true); btnCurrent.ButtonColor = ThemeColors.Primary; btnCurrent.IsOutline = false; btnHistory.ButtonColor = Color.Gray; btnHistory.IsOutline = true; btnCurrent.Invalidate(); btnHistory.Invalidate(); };
            btnHistory.Click += (s, e) => { showCurrent = false; LoadData(false); btnHistory.ButtonColor = ThemeColors.Primary; btnHistory.IsOutline = false; btnCurrent.ButtonColor = Color.Gray; btnCurrent.IsOutline = true; btnCurrent.Invalidate(); btnHistory.Invalidate(); };

            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns["TrangThai"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val.Contains("Quá hạn")) { e.CellStyle.ForeColor = ThemeColors.Danger; e.CellStyle.Font = new Font(ThemeColors.BodyFont, FontStyle.Bold); }
                    else if (val.Contains("Đã trả")) e.CellStyle.ForeColor = ThemeColors.Success;
                    else e.CellStyle.ForeColor = ThemeColors.Primary;
                }
            };

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 168);
            dgv.Size = new Size(Width - 64, Height - 168);

            LoadData(true);
        }

        private void LoadData(bool currentOnly)
        {
            dgv.Rows.Clear();
            var records = currentOnly
                ? SampleData.BorrowRecords.Where(b => b.NgayTraThuc == null).ToList()
                : SampleData.BorrowRecords.ToList();

            foreach (var r in records)
            {
                string bookName = SampleData.Books.FirstOrDefault(b => b.MaSach == r.MaSach)?.TenSach ?? r.MaSach;
                string status = r.NgayTraThuc != null ? "✅ Đã trả" : (r.IsOverdue ? "⚠️ Quá hạn" : "📖 Đang mượn");
                string tienPhat = r.TienPhat > 0 ? $"{r.TienPhat:N0} VNĐ" : "—";
                dgv.Rows.Add(bookName, r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"), r.NgayTraThuc?.ToString("dd/MM/yyyy") ?? "—", status, tienPhat);
            }
        }
    }
}
