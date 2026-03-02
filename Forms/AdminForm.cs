using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Forms.Panels;

namespace LibraryManagement.Forms
{
    public class AdminForm : Form
    {
        private Panel contentPanel = null!;
        private SidebarButton[] menuButtons = null!;

        public AdminForm()
        {
            Text = "Quản trị viên - Thư viện ĐH Thủy Lợi";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ThemeColors.Background;
            MinimumSize = new Size(1024, 600);

            // --- Sidebar ---
            var sidebar = new Panel { Width = ThemeColors.SidebarWidth, Dock = DockStyle.Left, BackColor = ThemeColors.SidebarBackground };

            // Logout button (Dock=Bottom, add first so it stays at bottom)
            var btnLogout = new SidebarButton { IconText = "\uE72B", Text = "Đăng xuất", Dock = DockStyle.Bottom, Size = new Size(ThemeColors.SidebarWidth, 50) };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserStore.CurrentUser = null;
                    Close();
                }
            };
            sidebar.Controls.Add(btnLogout);

            // Logo panel (Dock=Top)
            var logoPanel = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.FromArgb(20, 255, 255, 255) };
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString("\uE83D", new Font("Segoe MDL2 Assets", 24), b, 20, 18);
                using (var b = new SolidBrush(Color.White)) g.DrawString("QUẢN TRỊ", new Font("Segoe UI", 16, FontStyle.Bold), b, 68, 14);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString("Bảng điều khiển", ThemeColors.SmallFont, b, 70, 44);
                using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(pen, 20, 79, ThemeColors.SidebarWidth - 20, 79);
            };
            sidebar.Controls.Add(logoPanel);

            // User info (Dock=Top)
            var cu = UserStore.CurrentUser;
            var userPanel = new Panel { Height = 50, Dock = DockStyle.Top, BackColor = Color.Transparent };
            userPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString(cu?.HoTen ?? "Admin", new Font("Segoe UI Semibold", 10), b, 20, 8);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString(cu?.RoleDisplay ?? "", ThemeColors.SmallFont, b, 20, 28);
            };
            sidebar.Controls.Add(userPanel);

            // Menu buttons container (Dock=Fill, takes remaining space between top panels and logout)
            var menuPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, AutoScroll = true };

            string[][] menuItems = {
                new[] { "\uE80F", "Trang chủ" },
                new[] { "\uE77B", "Quản lý tài khoản" },
                new[] { "\uE83D", "Phân quyền" },
                new[] { "\uE713", "Cấu hình hệ thống" },
                new[] { "\uE895", "Sao lưu & Phục hồi" },
                new[] { "\uE7A8", "Nhật ký hoạt động" },
            };

            menuButtons = new SidebarButton[menuItems.Length];
            int y = 0;
            for (int i = 0; i < menuItems.Length; i++)
            {
                var btn = new SidebarButton { IconText = menuItems[i][0], Text = menuItems[i][1], Location = new Point(0, y), ActiveColor = ColorTranslator.FromHtml("#8B5CF6") };
                int idx = i;
                btn.Click += (s, e) => NavigateTo(idx);
                menuButtons[i] = btn;
                menuPanel.Controls.Add(btn);
                y += 50;
            }
            sidebar.Controls.Add(menuPanel);

            // --- Header ---
            var header = new Panel { Height = ThemeColors.HeaderHeight, Dock = DockStyle.Top, BackColor = ThemeColors.HeaderBackground };
            header.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString("BẢNG ĐIỀU KHIỂN QUẢN TRỊ", new Font("Segoe UI Semibold", 13), b, 24, 18);
                using (var pen = new Pen(ThemeColors.Border)) g.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            // --- Content ---
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ThemeColors.Background };

            // IMPORTANT: Add order matters for WinForms docking!
            // Fill first, then Top, then Left — last added gets docked first
            Controls.Add(contentPanel);
            Controls.Add(header);
            Controls.Add(sidebar);

            NavigateTo(0);
        }

        private void NavigateTo(int index)
        {
            foreach (var btn in menuButtons) btn.IsActive = false;
            menuButtons[index].IsActive = true;

            contentPanel.Controls.Clear();
            UserControl panel = index switch
            {
                0 => new AdminDashboard(),
                1 => new AccountPanel(),
                2 => new RolePanel(),
                3 => new SettingsPanel(),
                4 => new BackupPanel(),
                5 => new ActivityLogPanel(),
                _ => new AdminDashboard()
            };
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
        }
    }
}
