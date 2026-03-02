using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class RolePanel : UserControl
    {
        public RolePanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "PHÂN QUYỀN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Quản lý vai trò và quyền hạn trong hệ thống", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Role cards
            string[][] roles = {
                new[] { "🛡️", "Quản trị viên", "Toàn quyền quản lý hệ thống, tài khoản, cài đặt, sao lưu, báo cáo", "#8B5CF6" },
                new[] { "📚", "Thủ thư", "Quản lý sách, độc giả, mượn trả sách, kiểm kê kho", "#2563EB" },
                new[] { "📖", "Độc giả", "Tra cứu sách, xem sách đã mượn, yêu cầu gia hạn, xem thông báo", "#10B981" },
            };

            int y = 100;
            for (int ri = 0; ri < roles.Length; ri++)
            {
                var role = roles[ri];
                Panel card = new Panel { Location = new Point(32, y), Size = new Size(900, 140), BackColor = Color.Transparent };
                Color accent = ColorTranslator.FromHtml(role[3]);

                card.Paint += (s, e) =>
                {
                    var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(2, 2, card.Width - 6, card.Height - 6);
                    using (var path = ThemeColors.GetRoundedRect(rect, 12))
                    using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
                    // Left accent bar
                    using (var aPath = ThemeColors.GetRoundedRect(new Rectangle(2, 2, 5, card.Height - 6), 2))
                    using (var ab = new SolidBrush(accent)) g.FillPath(ab, aPath);
                };

                card.Controls.Add(new Label { Text = $"{role[0]}  {role[1]}", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = accent, Location = new Point(24, 16), Size = new Size(400, 32), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = role[2], Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(24, 54), Size = new Size(700, 22), BackColor = Color.Transparent });

                // Permissions
                string[] perms = ri switch
                {
                    0 => new[] { "Quản lý tài khoản", "Phân quyền", "Cài đặt hệ thống", "Báo cáo", "Nhật ký", "Sao lưu" },
                    1 => new[] { "Quản lý sách", "Quản lý độc giả", "Mượn - Trả", "Kiểm kê kho" },
                    2 => new[] { "Tra cứu sách", "Xem sách đã mượn", "Yêu cầu gia hạn", "Thông báo" },
                    _ => Array.Empty<string>()
                };

                int px = 24;
                foreach (var p in perms)
                {
                    Label tag = new Label { Text = $"✓ {p}", Font = ThemeColors.SmallFont, ForeColor = accent, BackColor = Color.FromArgb(30, accent), Size = new Size(TextRenderer.MeasureText($"✓ {p}", ThemeColors.SmallFont).Width + 16, 24), Location = new Point(px, 88), TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(4, 0, 4, 0) };
                    card.Controls.Add(tag);
                    px += tag.Width + 8;
                }

                int count = UserStore.Users.Count(u => (ri == 0 && u.Role == UserRole.Admin) || (ri == 1 && u.Role == UserRole.ThuThu) || (ri == 2 && u.Role == UserRole.DocGia));
                card.Controls.Add(new Label { Text = $"{count} người dùng", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(740, 16), Size = new Size(140, 28), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });

                Controls.Add(card);
                y += 160;
            }
        }
    }
}
