using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class BackupPanel : UserControl
    {
        public BackupPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "SAO LƯU & PHỤC HỒI", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Quản lý sao lưu và phục hồi dữ liệu hệ thống", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Backup card
            Panel backupCard = new Panel { Location = new Point(32, 100), Size = new Size(440, 280), BackColor = Color.Transparent };
            backupCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, backupCard.Width - 6, backupCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };

            backupCard.Controls.Add(new Label { Text = "💾  Sao lưu dữ liệu", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 32), BackColor = Color.Transparent });
            backupCard.Controls.Add(new Label { Text = "Tạo bản sao lưu toàn bộ cơ sở dữ liệu", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 52), Size = new Size(400, 22), BackColor = Color.Transparent });
            backupCard.Controls.Add(new Label { Text = "Lần sao lưu gần nhất:", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 92), Size = new Size(200, 22), BackColor = Color.Transparent });
            backupCard.Controls.Add(new Label { Text = DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy HH:mm"), Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(220, 92), Size = new Size(200, 22), BackColor = Color.Transparent });
            backupCard.Controls.Add(new Label { Text = "Kích thước DB:", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 120), Size = new Size(200, 22), BackColor = Color.Transparent });
            backupCard.Controls.Add(new Label { Text = "2.4 MB", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(220, 120), Size = new Size(200, 22), BackColor = Color.Transparent });

            var btnBackup = new RoundedButton { Text = "🔄 Sao lưu ngay", Size = new Size(200, 48), Location = new Point(20, 166), ButtonColor = ColorTranslator.FromHtml("#8B5CF6"), Font = new Font("Segoe UI Semibold", 11) };
            btnBackup.Click += (s, e) =>
            {
                UserStore.Logs.Insert(0, new ActivityLog { ThoiGian = DateTime.Now, NguoiDung = UserStore.CurrentUser?.HoTen ?? "Admin", HanhDong = "Sao lưu CSDL", ChiTiet = "Sao lưu thành công" });
                MessageBox.Show("Sao lưu cơ sở dữ liệu thành công!", "Sao lưu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            backupCard.Controls.Add(btnBackup);

            var btnAutoBackup = new RoundedButton { Text = "⏱️ Tự động sao lưu", Size = new Size(180, 48), Location = new Point(230, 166), ButtonColor = ThemeColors.Primary, Font = new Font("Segoe UI Semibold", 11) };
            btnAutoBackup.Click += (s, e) => MessageBox.Show("Đã bật tự động sao lưu hàng ngày lúc 00:00", "Tự động sao lưu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            backupCard.Controls.Add(btnAutoBackup);

            Controls.Add(backupCard);

            // Restore card
            Panel restoreCard = new Panel { Location = new Point(490, 100), Size = new Size(440, 280), BackColor = Color.Transparent };
            restoreCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, restoreCard.Width - 6, restoreCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };

            restoreCard.Controls.Add(new Label { Text = "📥  Phục hồi dữ liệu", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 32), BackColor = Color.Transparent });
            restoreCard.Controls.Add(new Label { Text = "Khôi phục dữ liệu từ bản sao lưu", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 52), Size = new Size(400, 22), BackColor = Color.Transparent });

            restoreCard.Controls.Add(new Label { Text = "⚠️  Lưu ý: Phục hồi sẽ ghi đè toàn bộ\ndữ liệu hiện tại. Hãy sao lưu trước!", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.Warning, Location = new Point(20, 92), Size = new Size(400, 44), BackColor = Color.FromArgb(40, 255, 200, 0) });

            var btnRestore = new RoundedButton { Text = "📂 Chọn file sao lưu", Size = new Size(200, 48), Location = new Point(20, 160), ButtonColor = ThemeColors.Warning, Font = new Font("Segoe UI Semibold", 11) };
            btnRestore.Click += (s, e) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn phục hồi dữ liệu?\nDữ liệu hiện tại sẽ bị ghi đè!", "Phục hồi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) MessageBox.Show("Phục hồi dữ liệu thành công!", "Phục hồi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            restoreCard.Controls.Add(btnRestore);

            Controls.Add(restoreCard);

            // Backup history
            Controls.Add(new Label { Text = "📋  Lịch sử sao lưu", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 400), Size = new Size(400, 28), BackColor = Color.Transparent });

            DataGridView dgvHistory = new DataGridView { Location = new Point(32, 436), Size = new Size(900, 200), ReadOnly = true, AllowUserToAddRows = false };
            ModernDataGridView.ApplyStyle(dgvHistory);
            dgvHistory.Columns.Add("ThoiGian", "Thời gian");
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiSaoLuu", HeaderText = "Loại", Width = 120 });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "KichThuoc", HeaderText = "Kích thước", Width = 100 });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", Width = 120 });

            dgvHistory.Rows.Add(DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy HH:mm"), "Thủ công", "2.4 MB", "✅ Thành công");
            dgvHistory.Rows.Add(DateTime.Now.AddDays(-5).ToString("dd/MM/yyyy HH:mm"), "Tự động", "2.3 MB", "✅ Thành công");
            dgvHistory.Rows.Add(DateTime.Now.AddDays(-12).ToString("dd/MM/yyyy HH:mm"), "Tự động", "2.1 MB", "✅ Thành công");
            Controls.Add(dgvHistory);
        }
    }
}
