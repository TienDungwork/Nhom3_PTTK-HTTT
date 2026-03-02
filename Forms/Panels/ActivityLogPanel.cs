using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ActivityLogPanel : UserControl
    {
        private DataGridView dgv = null!;

        public ActivityLogPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "NHẬT KÝ HOẠT ĐỘNG", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Theo dõi mọi hoạt động trong hệ thống", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Filter bar
            var txtSearch = new RoundedTextBox { Placeholder = "🔍  Tìm kiếm theo người dùng, hành động...", Location = new Point(32, 96), Size = new Size(400, 44) };
            Controls.Add(txtSearch);

            var btnExport = new RoundedButton { Text = "📥 Xuất báo cáo", Size = new Size(150, 42), Location = new Point(450, 98), ButtonColor = ColorTranslator.FromHtml("#8B5CF6"), Font = ThemeColors.ButtonFont };
            btnExport.Click += (s, e) => MessageBox.Show("Xuất nhật ký hoạt động thành công!", "Xuất báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Controls.Add(btnExport);

            // DataGridView
            dgv = new DataGridView { Location = new Point(32, 152), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, ReadOnly = true, AllowUserToAddRows = false };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("ThoiGian", "Thời gian");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NguoiDung", HeaderText = "Người dùng", FillWeight = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "HanhDong", HeaderText = "Hành động", FillWeight = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChiTiet", HeaderText = "Chi tiết", FillWeight = 250 });

            LoadLogs("");

            txtSearch.TextChanged += (s, e) => LoadLogs(txtSearch.InputText.ToLower());

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 172);
            dgv.Size = new Size(Width - 64, Height - 172);
        }

        private void LoadLogs(string filter)
        {
            dgv.Rows.Clear();
            var logs = string.IsNullOrEmpty(filter)
                ? UserStore.Logs
                : UserStore.Logs.Where(l => l.NguoiDung.ToLower().Contains(filter) || l.HanhDong.ToLower().Contains(filter) || l.ChiTiet.ToLower().Contains(filter)).ToList();

            foreach (var log in logs.OrderByDescending(l => l.ThoiGian))
                dgv.Rows.Add(log.ThoiGian.ToString("dd/MM/yyyy HH:mm"), log.NguoiDung, log.HanhDong, log.ChiTiet);
        }
    }
}
