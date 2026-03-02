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
    public class ReaderForm : Form
    {
        private Panel contentPanel = null!;
        private SidebarButton[] menuButtons = null!;

        public ReaderForm()
        {
            Text = "Độc giả - Thư viện ĐH Thủy Lợi";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ThemeColors.Background;
            MinimumSize = new Size(1024, 600);

            // --- Sidebar ---
            var sidebar = new Panel { Width = ThemeColors.SidebarWidth, Dock = DockStyle.Left, BackColor = ThemeColors.SidebarBackground };

            // Logo
            var logoPanel = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.FromArgb(20, 255, 255, 255) };
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString("\uE736", new Font("Segoe MDL2 Assets", 24), b, 20, 18);
                using (var b = new SolidBrush(Color.White)) g.DrawString("ĐỘC GIẢ", new Font("Segoe UI", 16, FontStyle.Bold), b, 68, 14);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString("Cổng thông tin", ThemeColors.SmallFont, b, 70, 44);
                using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(pen, 20, 79, ThemeColors.SidebarWidth - 20, 79);
            };
            sidebar.Controls.Add(logoPanel);

            // User info
            var cu = UserStore.CurrentUser;
            var userPanel = new Panel { Height = 50, Dock = DockStyle.Top, BackColor = Color.Transparent };
            userPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString(cu?.HoTen ?? "Độc giả", new Font("Segoe UI Semibold", 10), b, 20, 8);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString(cu?.RoleDisplay ?? "", ThemeColors.SmallFont, b, 20, 28);
            };
            sidebar.Controls.Add(userPanel);

            string[][] menuItems = {
                new[] { "\uE80F", "Trang chủ" },
                new[] { "\uE721", "Tìm kiếm sách" },
                new[] { "\uE8C8", "Mượn / Trả sách" },
                new[] { "\uE8A5", "Lịch sử mượn" },
                new[] { "\uEA8F", "Thông báo" },
            };

            menuButtons = new SidebarButton[menuItems.Length];
            int y = 0;
            for (int i = 0; i < menuItems.Length; i++)
            {
                var btn = new SidebarButton { IconText = menuItems[i][0], Text = menuItems[i][1], Location = new Point(0, y), ActiveColor = ColorTranslator.FromHtml("#10B981") };
                int idx = i;
                btn.Click += (s, e) => NavigateTo(idx);
                menuButtons[i] = btn;
                sidebar.Controls.Add(btn);
                y += 50;
            }

            var btnLogout = new SidebarButton { IconText = "\uE72B", Text = "Đăng xuất", Dock = DockStyle.Bottom, Size = new Size(ThemeColors.SidebarWidth, 50) };
            btnLogout.Click += (s, e) => { if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { UserStore.CurrentUser = null; Close(); } };
            sidebar.Controls.Add(btnLogout);
            Controls.Add(sidebar);

            // --- Header ---
            var header = new Panel { Height = ThemeColors.HeaderHeight, Dock = DockStyle.Top, BackColor = ThemeColors.HeaderBackground };
            header.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString("CỔNG THÔNG TIN ĐỘC GIẢ", new Font("Segoe UI Semibold", 13), b, 24, 18);
                using (var pen = new Pen(ThemeColors.Border)) g.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };
            Controls.Add(header);

            // --- Content ---
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ThemeColors.Background, Padding = new Padding(0) };
            Controls.Add(contentPanel);

            NavigateTo(0);
        }

        private void NavigateTo(int index)
        {
            foreach (var btn in menuButtons) btn.IsActive = false;
            menuButtons[index].IsActive = true;

            contentPanel.Controls.Clear();
            UserControl panel = index switch
            {
                0 => new ReaderDashboard(),
                1 => new SearchBookPanel(),
                2 => new BorrowReturnPanel(),
                3 => new BorrowHistoryPanel(),
                4 => new NotificationPanel(),
                _ => new ReaderDashboard()
            };
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
        }
    }
}
