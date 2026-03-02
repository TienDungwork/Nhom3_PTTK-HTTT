using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Forms.Panels;
using LibraryManagement.Models;

namespace LibraryManagement.Forms
{
    public class MainForm : Form
    {
        private Panel sidebarPanel = null!;
        private Panel headerPanel = null!;
        private Panel contentPanel = null!;
        private SidebarButton[] menuButtons = null!;

        public MainForm()
        {
            InitializeComponent();
            NavigateTo(0);
        }

        private void InitializeComponent()
        {
            Text = "Hệ thống Quản lý Thư viện";
            Size = new Size(1400, 850);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1200, 700);
            BackColor = ThemeColors.Background;
            DoubleBuffered = true;

            CreateSidebar();
            CreateHeader();
            CreateContent();
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = ThemeColors.SidebarWidth,
                BackColor = ThemeColors.SidebarBackground
            };

            // Logo area
            Panel logoPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.Transparent };
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White))
                    g.DrawString("📚", new Font("Segoe UI", 24), b, 20, 18);
                using (var b = new SolidBrush(Color.White))
                    g.DrawString("THƯ VIỆN", new Font("Segoe UI", 16, FontStyle.Bold), b, 68, 14);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                    g.DrawString("Hệ thống quản lý", ThemeColors.SmallFont, b, 70, 44);
                using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255)))
                    g.DrawLine(pen, 20, 79, ThemeColors.SidebarWidth - 20, 79);
            };
            sidebarPanel.Controls.Add(logoPanel);

            // User info panel at top of sidebar
            var cu = UserStore.CurrentUser;
            Panel userPanel = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.FromArgb(30, 255, 255, 255) };
            userPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                string name = cu?.HoTen ?? "Admin";
                string role = cu?.RoleDisplay ?? "Quản trị viên";
                using (var b = new SolidBrush(Color.White))
                    g.DrawString(name, ThemeColors.SubTitleFont, b, 16, 10);
                using (var b = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                    g.DrawString(role, ThemeColors.SmallFont, b, 16, 36);
            };
            sidebarPanel.Controls.Add(userPanel);

            // Menu items
            string[][] menuItems = {
                new[] { "🏠", "Trang chủ" },
                new[] { "📚", "Quản lý sách" },
                new[] { "👥", "Quản lý độc giả" },
                new[] { "📋", "Mượn - Trả sách" },
                new[] { "📊", "Báo cáo" },
                new[] { "⚙️", "Cài đặt" },
            };

            menuButtons = new SidebarButton[menuItems.Length];
            int y = 160;

            for (int i = 0; i < menuItems.Length; i++)
            {
                int index = i;
                var btn = new SidebarButton
                {
                    IconText = menuItems[i][0],
                    Text = menuItems[i][1],
                    Location = new Point(0, y),
                    Size = new Size(ThemeColors.SidebarWidth, 50)
                };
                btn.Click += (s, e) => NavigateTo(index);
                menuButtons[i] = btn;
                sidebarPanel.Controls.Add(btn);
                y += 50;
            }

            // Logout at bottom
            SidebarButton btnLogout = new SidebarButton
            {
                IconText = "🚪",
                Text = "Đăng xuất",
                Dock = DockStyle.Bottom,
                Size = new Size(ThemeColors.SidebarWidth, 50)
            };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserStore.CurrentUser = null;
                    Close();
                }
            };
            sidebarPanel.Controls.Add(btnLogout);
            Controls.Add(sidebarPanel);
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = ThemeColors.HeaderHeight,
                BackColor = ThemeColors.HeaderBackground,
                Padding = new Padding(24, 0, 24, 0)
            };
            headerPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary))
                    g.DrawString("HỆ THỐNG QUẢN LÝ THƯ VIỆN", new Font("Segoe UI Semibold", 13), b, 24, 18);
                using (var pen = new Pen(ThemeColors.Border))
                    g.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
            };
            Controls.Add(headerPanel);
        }

        private void CreateContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background,
            };
            Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        private void NavigateTo(int index)
        {
            foreach (var btn in menuButtons)
                btn.IsActive = false;
            menuButtons[index].IsActive = true;

            contentPanel.Controls.Clear();

            UserControl panel = index switch
            {
                0 => new DashboardPanel(),
                1 => new BookPanel(),
                2 => new ReaderPanel(),
                3 => new BorrowPanel(),
                4 => new ReportPanel(),
                5 => new SettingsPanel(),
                _ => new DashboardPanel()
            };

            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
        }
    }
}
