using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class AdminDashboard : UserControl
    {
        public AdminDashboard()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "BẢNG ĐIỀU KHIỂN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tổng quan hệ thống quản lý thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            int totalUsers = UserStore.Users.Count;
            int admins = UserStore.Users.Count(u => u.Role == UserRole.Admin);
            int librarians = UserStore.Users.Count(u => u.Role == UserRole.ThuThu);
            int readers = UserStore.Users.Count(u => u.Role == UserRole.DocGia);

            var card1 = new StatCard { Title = "Tổng tài khoản", Value = totalUsers.ToString(), IconText = "👥", AccentColor = ColorTranslator.FromHtml("#8B5CF6"), Location = new Point(32, 100), Size = new Size(220, 110) };
            var card2 = new StatCard { Title = "Quản trị viên", Value = admins.ToString(), IconText = "🛡️", AccentColor = ThemeColors.Danger, Location = new Point(268, 100), Size = new Size(220, 110) };
            var card3 = new StatCard { Title = "Thủ thư", Value = librarians.ToString(), IconText = "📚", AccentColor = ThemeColors.Primary, Location = new Point(504, 100), Size = new Size(220, 110) };
            var card4 = new StatCard { Title = "Độc giả", Value = readers.ToString(), IconText = "📖", AccentColor = ThemeColors.Success, Location = new Point(740, 100), Size = new Size(220, 110) };
            Controls.Add(card1); Controls.Add(card2); Controls.Add(card3); Controls.Add(card4);

            // Role distribution chart
            Panel chartCard = new Panel { Location = new Point(32, 230), Size = new Size(460, 300), BackColor = Color.Transparent };
            chartCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, chartCard.Width - 6, chartCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString("Phân quyền hiện tại", ThemeColors.SubTitleFont, b, 20, 16);

                // Donut chart
                int cx = 160, cy = 170, r = 80;
                float total = totalUsers;
                float startAngle = -90;
                Color[] colors = { ThemeColors.Danger, ThemeColors.Primary, ThemeColors.Success };
                int[] values = { admins, librarians, readers };
                string[] labels = { "QTV", "Thủ thư", "Độc giả" };

                for (int i = 0; i < values.Length; i++)
                {
                    float sweep = (values[i] / total) * 360f;
                    using (var b = new SolidBrush(colors[i]))
                        g.FillPie(b, cx - r, cy - r, r * 2, r * 2, startAngle, sweep);
                    startAngle += sweep;
                }
                using (var wb = new SolidBrush(Color.White))
                    g.FillEllipse(wb, cx - 45, cy - 45, 90, 90);
                using (var b = new SolidBrush(ThemeColors.TextPrimary))
                    g.DrawString(totalUsers.ToString(), new Font("Segoe UI", 20, FontStyle.Bold), b, cx - 20, cy - 18);

                // Legend
                for (int i = 0; i < labels.Length; i++)
                {
                    int ly = 60 + i * 30;
                    using (var lb = new SolidBrush(colors[i])) g.FillRectangle(lb, 320, ly, 14, 14);
                    using (var tb = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString($"{labels[i]}: {values[i]}", ThemeColors.BodyFont, tb, 342, ly - 2);
                }
            };
            Controls.Add(chartCard);

            // Recent activity
            Panel actCard = new Panel { Location = new Point(510, 230), Size = new Size(460, 300), BackColor = Color.Transparent };
            actCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, actCard.Width - 6, actCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            actCard.Controls.Add(new Label { Text = "🕐  Hoạt động hệ thống", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });

            int ay = 52;
            foreach (var log in UserStore.Logs.OrderByDescending(l => l.ThoiGian).Take(5))
            {
                var diff = DateTime.Now - log.ThoiGian;
                string timeAgo = diff.TotalHours < 1 ? $"{(int)diff.TotalMinutes}p trước" : diff.TotalHours < 24 ? $"{(int)diff.TotalHours}h trước" : $"{(int)diff.TotalDays}d trước";
                actCard.Controls.Add(new Label { Text = $"• [{log.NguoiDung}] {log.HanhDong}", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, ay), Size = new Size(340, 22), BackColor = Color.Transparent });
                actCard.Controls.Add(new Label { Text = timeAgo, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(370, ay + 2), Size = new Size(70, 20), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                ay += 44;
            }
            Controls.Add(actCard);
        }
    }
}
