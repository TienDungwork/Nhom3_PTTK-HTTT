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
    public class LibrarianForm : Form
    {
        private Panel contentPanel = null!;
        private SidebarButton[] menuButtons = null!;

        public LibrarianForm()
        {
            InitializeComponent();
            NavigateTo(0);
        }

        private void InitializeComponent()
        {
            Text = "Thủ thư - Hệ thống Quản lý Thư viện";
            Size = new Size(1400, 850);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1200, 700);
            BackColor = ThemeColors.Background;
            DoubleBuffered = true;

            // === SIDEBAR ===
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = ThemeColors.SidebarWidth, BackColor = ThemeColors.SidebarBackground };

            // Logo
            Panel logoPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.Transparent };
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString("\uE736", new Font("Segoe MDL2 Assets", 24), b, 20, 18);
                using (var b = new SolidBrush(Color.White)) g.DrawString("THƯ VIỆN", new Font("Segoe UI", 16, FontStyle.Bold), b, 68, 14);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString("Giao diện Thủ thư", ThemeColors.SmallFont, b, 70, 44);
                using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(pen, 20, 79, ThemeColors.SidebarWidth - 20, 79);
            };
            sidebar.Controls.Add(logoPanel);

            // User info
            var cu = UserStore.CurrentUser;
            Panel userPanel = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.FromArgb(30, 255, 255, 255) };
            userPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString(cu?.HoTen ?? "Thủ thư", ThemeColors.SubTitleFont, b, 16, 8);
                using (var b = new SolidBrush(Color.FromArgb(180, 255, 255, 255))) g.DrawString("Thủ thư", ThemeColors.SmallFont, b, 16, 32);
            };
            sidebar.Controls.Add(userPanel);

            // Menu
            string[][] menuItems = {
                new[] { "\uE80F", "Trang chủ" },
                new[] { "\uE736", "Quản lý sách" },
                new[] { "\uE716", "Quản lý độc giả" },
                new[] { "\uE8C8", "Mượn - Trả sách" },
                new[] { "\uE7B8", "Kiểm kê kho" },
            };

            menuButtons = new SidebarButton[menuItems.Length];
            int y = 150;
            for (int i = 0; i < menuItems.Length; i++)
            {
                int idx = i;
                var btn = new SidebarButton { IconText = menuItems[i][0], Text = menuItems[i][1], Location = new Point(0, y), Size = new Size(ThemeColors.SidebarWidth, 50) };
                btn.Click += (s, e) => NavigateTo(idx);
                menuButtons[i] = btn;
                sidebar.Controls.Add(btn);
                y += 50;
            }

            // Logout
            var btnLogout = new SidebarButton { IconText = "\uE72B", Text = "Đăng xuất", Dock = DockStyle.Bottom, Size = new Size(ThemeColors.SidebarWidth, 50) };
            btnLogout.Click += (s, e) => { if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { UserStore.CurrentUser = null; Close(); } };
            sidebar.Controls.Add(btnLogout);
            Controls.Add(sidebar);

            // === HEADER ===
            Panel header = new Panel { Dock = DockStyle.Top, Height = ThemeColors.HeaderHeight, BackColor = ThemeColors.HeaderBackground };
            header.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString("QUẢN LÝ THƯ VIỆN — THỦ THƯ", new Font("Segoe UI Semibold", 13), b, 24, 18);
                using (var pen = new Pen(ThemeColors.Border)) g.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };
            Controls.Add(header);

            // === CONTENT ===
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ThemeColors.Background };
            Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        private void NavigateTo(int index)
        {
            foreach (var b in menuButtons) b.IsActive = false;
            menuButtons[index].IsActive = true;
            contentPanel.Controls.Clear();

            UserControl panel = index switch
            {
                0 => new LibrarianDashboard(),
                1 => new BookPanel(),
                2 => new ReaderPanel(),
                3 => new BorrowPanel(),
                4 => new InventoryPanel(),
                _ => new LibrarianDashboard()
            };
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
        }
    }
}
