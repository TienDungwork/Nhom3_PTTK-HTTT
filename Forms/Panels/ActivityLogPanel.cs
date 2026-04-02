using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
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

            var cboType = new ComboBox
            {
                Location = new Point(450, 104),
                Size = new Size(140, 28),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboType.Items.AddRange(new object[] { "Tất cả", "Auth", "Security", "System", "Business" });
            cboType.SelectedIndex = 0;
            Controls.Add(cboType);

            var btnExport = new RoundedButton { Text = "📥 Xuất báo cáo", Size = new Size(150, 42), Location = new Point(602, 98), ButtonColor = ColorTranslator.FromHtml("#8B5CF6"), Font = ThemeColors.ButtonFont };
            btnExport.Click += (s, e) =>
            {
                using var save = new SaveFileDialog { Filter = "CSV file (*.csv)|*.csv", FileName = $"audit_logs_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
                if (save.ShowDialog(FindForm()) != DialogResult.OK) return;
                var lines = UserStore.Logs.Select(l => $"{l.ThoiGian:yyyy-MM-dd HH:mm:ss},{Escape(l.NguoiDung)},{Escape(l.HanhDong)},{Escape(l.ChiTiet)},{l.LoaiSuKien}");
                File.WriteAllLines(save.FileName, (new[] { "ThoiGian,NguoiDung,HanhDong,ChiTiet,LoaiSuKien" }).Concat(lines));
                MessageBox.Show("Xuất nhật ký hoạt động thành công!", "Xuất báo cáo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnExport);

            // DataGridView
            dgv = new DataGridView { Location = new Point(32, 152), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, ReadOnly = true, AllowUserToAddRows = false };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("ThoiGian", "Thời gian");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NguoiDung", HeaderText = "Người dùng", FillWeight = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "HanhDong", HeaderText = "Hành động", FillWeight = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChiTiet", HeaderText = "Chi tiết", FillWeight = 250 });

            LoadLogs("", "Tất cả");

            txtSearch.TextChanged += (s, e) => LoadLogs(txtSearch.InputText.ToLower(), cboType.SelectedItem?.ToString() ?? "Tất cả");
            cboType.SelectedIndexChanged += (s, e) => LoadLogs(txtSearch.InputText.ToLower(), cboType.SelectedItem?.ToString() ?? "Tất cả");

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 172);
            dgv.Size = new Size(Width - 64, Height - 172);
        }

        private void LoadLogs(string filter, string typeFilter)
        {
            dgv.Rows.Clear();
            var logs = UserStore.Logs.AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
                logs = logs.Where(l => l.NguoiDung.ToLower().Contains(filter) || l.HanhDong.ToLower().Contains(filter) || l.ChiTiet.ToLower().Contains(filter));
            if (!string.Equals(typeFilter, "Tất cả", StringComparison.OrdinalIgnoreCase))
                logs = logs.Where(l => string.Equals(l.LoaiSuKien, typeFilter, StringComparison.OrdinalIgnoreCase));

            foreach (var log in logs.OrderByDescending(l => l.ThoiGian))
                dgv.Rows.Add(log.ThoiGian.ToString("dd/MM/yyyy HH:mm"), log.NguoiDung, $"[{log.LoaiSuKien}] {log.HanhDong}", log.ChiTiet);
        }

        private static string Escape(string value)
        {
            if (value.Contains(","))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
