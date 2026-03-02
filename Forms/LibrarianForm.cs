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
            Text = "Thủ thư - Thư viện ĐH Thủy Lợi";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = ThemeColors.Background;
            MinimumSize = new Size(1024, 600);

            // --- Sidebar ---
            var sidebar = new Panel { Width = ThemeColors.SidebarWidth, Dock = DockStyle.Left, BackColor = ThemeColors.SidebarBackground };

            var logoPanel = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.FromArgb(20, 255, 255, 255) };
            logoPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString("\uE736", new Font("Segoe MDL2 Assets", 24), b, 20, 18);
                using (var b = new SolidBrush(Color.White)) g.DrawString("THƯ VIỆN", new Font("Segoe UI", 16, FontStyle.Bold), b, 68, 14);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString("Giao diện Thủ thư", ThemeColors.SmallFont, b, 70, 44);
                using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(pen, 20, 79, ThemeColors.SidebarWidth - 20, 79);
            };

            var cu = UserStore.CurrentUser;
            var userPanel = new Panel { Height = 50, Dock = DockStyle.Top, BackColor = Color.Transparent };
            userPanel.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(Color.White)) g.DrawString(cu?.HoTen ?? "Thủ thư", new Font("Segoe UI Semibold", 10), b, 20, 8);
                using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255))) g.DrawString(cu?.RoleDisplay ?? "", ThemeColors.SmallFont, b, 20, 28);
            };

            var menuPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, AutoScroll = true };

            string[][] menuItems = {
                new[] { "\uE80F", "Trang chủ" },
                new[] { "\uE736", "Cập nhật danh mục" },
                new[] { "\uE8C8", "Phân loại sách" },
                new[] { "\uE762", "Quản lý mượn trả" },
                new[] { "\uE7BA", "Quản lý quá hạn" },
                new[] { "\uE7A8", "Báo cáo thống kê" },
            };

            menuButtons = new SidebarButton[menuItems.Length];
            int y = 0;
            for (int i = 0; i < menuItems.Length; i++)
            {
                var btn = new SidebarButton { IconText = menuItems[i][0], Text = menuItems[i][1], Location = new Point(0, y) };
                int idx = i;
                btn.Click += (s, e) => NavigateTo(idx);
                menuButtons[i] = btn;
                menuPanel.Controls.Add(btn);
                y += 50;
            }

            var btnLogout = new SidebarButton { IconText = "\uE72B", Text = "Đăng xuất", Dock = DockStyle.Bottom, Size = new Size(ThemeColors.SidebarWidth, 50) };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UserStore.CurrentUser = null;
                    Close();
                }
            };

            // SIDEBAR INTERNAL DOCK ORDER: Fill first (docked last), then Top, then Bottom last (docked first)
            sidebar.Controls.Add(menuPanel);    // Fill → docked LAST
            sidebar.Controls.Add(userPanel);    // Top  → docked 3rd
            sidebar.Controls.Add(logoPanel);    // Top  → docked 2nd
            sidebar.Controls.Add(btnLogout);    // Bottom → docked 1st

            // --- Header ---
            var header = new Panel { Height = ThemeColors.HeaderHeight, Dock = DockStyle.Top, BackColor = ThemeColors.HeaderBackground };
            header.Paint += (s, e) =>
            {
                var g = e.Graphics; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString("QUẢN LÝ THƯ VIỆN — THỦ THƯ", new Font("Segoe UI Semibold", 13), b, 24, 18);
                using (var pen = new Pen(ThemeColors.Border)) g.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            // --- Content ---
            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = ThemeColors.Background };

            // FORM DOCK ORDER: Fill first (docked last), then Top, then Left last (docked first)
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
                0 => new LibrarianDashboard(),
                1 => new CatalogPanel(),
                2 => new ClassifyPanel(),
                3 => new BorrowManagePanel(),
                4 => new OverduePanel(),
                5 => new ReportPanel(),
                _ => new LibrarianDashboard()
            };
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
        }
    }
}
