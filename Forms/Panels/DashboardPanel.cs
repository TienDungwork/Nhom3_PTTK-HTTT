using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class DashboardPanel : UserControl
    {
        public DashboardPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "TRANG CHỦ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Tổng quan hoạt động thư viện",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 62),
                Size = new Size(400, 22),
                BackColor = Color.Transparent
            };
            Controls.Add(lblSubtitle);

            // Stats cards
            int totalBooks = SampleData.Books.Sum(b => b.SoLuong);
            int borrowedBooks = SampleData.BorrowRecords.Count(r => r.TrangThai == "Đang mượn");
            int overdueBooks = SampleData.BorrowRecords.Count(r => r.IsOverdue);
            int totalReaders = SampleData.Readers.Count;

            StatCard card1 = new StatCard
            {
                IconText = "📚", Title = "Tổng số sách", Value = totalBooks.ToString(),
                AccentColor = ThemeColors.Primary, Location = new Point(32, 100), Size = new Size(230, 130)
            };
            StatCard card2 = new StatCard
            {
                IconText = "📖", Title = "Sách đang mượn", Value = borrowedBooks.ToString(),
                AccentColor = ThemeColors.Warning, Location = new Point(282, 100), Size = new Size(230, 130)
            };
            StatCard card3 = new StatCard
            {
                IconText = "⏰", Title = "Sách quá hạn", Value = overdueBooks.ToString(),
                AccentColor = ThemeColors.Danger, Location = new Point(532, 100), Size = new Size(230, 130)
            };
            StatCard card4 = new StatCard
            {
                IconText = "👥", Title = "Tổng số độc giả", Value = totalReaders.ToString(),
                AccentColor = ThemeColors.Success, Location = new Point(782, 100), Size = new Size(230, 130)
            };

            Controls.AddRange(new Control[] { card1, card2, card3, card4 });

            // Chart panel
            Panel chartPanel = new Panel
            {
                Location = new Point(32, 250),
                Size = new Size(540, 320),
                BackColor = Color.Transparent
            };
            chartPanel.Paint += ChartPanel_Paint;
            Controls.Add(chartPanel);

            // Recent activity panel
            Panel activityPanel = new Panel
            {
                Location = new Point(590, 250),
                Size = new Size(420, 320),
                BackColor = Color.Transparent
            };
            activityPanel.Paint += ActivityPanel_Paint;
            Controls.Add(activityPanel);
        }

        private void ChartPanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Card background
            Rectangle cardRect = new Rectangle(2, 2, panel.Width - 6, panel.Height - 6);
            using (GraphicsPath path = ThemeColors.GetRoundedRect(cardRect, 12))
            {
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    g.FillPath(shadow, ThemeColors.GetRoundedRect(new Rectangle(4, 4, panel.Width - 6, panel.Height - 6), 12));
                using (SolidBrush bg = new SolidBrush(Color.White))
                    g.FillPath(bg, path);
            }

            // Title
            using (SolidBrush b = new SolidBrush(ThemeColors.TextPrimary))
                g.DrawString("📊  Thống kê theo thể loại", ThemeColors.SubTitleFont, b, 20, 16);

            // Bar chart
            var categories = SampleData.Books.GroupBy(b => b.TheLoai)
                .Select(grp => new { Name = grp.Key, Count = grp.Sum(b => b.SoLuong) })
                .OrderByDescending(x => x.Count).Take(6).ToList();

            int maxVal = categories.Any() ? categories.Max(c => c.Count) : 1;
            int barWidth = 50;
            int startX = 40;
            int maxBarHeight = 180;
            int baseY = 280;

            Color[] barColors = {
                ThemeColors.Primary, ThemeColors.Success, ThemeColors.Warning,
                ThemeColors.Info, ThemeColors.Danger, Color.FromArgb(139, 92, 246)
            };

            for (int i = 0; i < categories.Count; i++)
            {
                int barHeight = (int)((float)categories[i].Count / maxVal * maxBarHeight);
                Rectangle barRect = new Rectangle(
                    startX + i * (barWidth + 30),
                    baseY - barHeight,
                    barWidth,
                    barHeight);

                using (GraphicsPath barPath = ThemeColors.GetRoundedRect(
                    new Rectangle(barRect.X, barRect.Y, barRect.Width, barRect.Height + 8), 6))
                {
                    g.SetClip(barRect);
                    using (SolidBrush barBrush = new SolidBrush(barColors[i % barColors.Length]))
                        g.FillPath(barBrush, barPath);
                    g.ResetClip();
                }

                // Value on top
                using (SolidBrush tb = new SolidBrush(ThemeColors.TextPrimary))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                    g.DrawString(categories[i].Count.ToString(), ThemeColors.SmallFont, tb,
                        barRect.X + barRect.Width / 2, barRect.Y - 18, sf);

                // Label below
                using (SolidBrush lb = new SolidBrush(ThemeColors.TextSecondary))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    string name = categories[i].Name.Length > 8
                        ? categories[i].Name.Substring(0, 7) + "…"
                        : categories[i].Name;
                    g.DrawString(name, ThemeColors.SmallFont, lb,
                        barRect.X + barRect.Width / 2, baseY + 6, sf);
                }
            }
        }

        private void ActivityPanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Card background
            Rectangle cardRect = new Rectangle(2, 2, panel.Width - 6, panel.Height - 6);
            using (GraphicsPath path = ThemeColors.GetRoundedRect(cardRect, 12))
            {
                using (SolidBrush shadow = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    g.FillPath(shadow, ThemeColors.GetRoundedRect(new Rectangle(4, 4, panel.Width - 6, panel.Height - 6), 12));
                using (SolidBrush bg = new SolidBrush(Color.White))
                    g.FillPath(bg, path);
            }

            // Title
            using (SolidBrush b = new SolidBrush(ThemeColors.TextPrimary))
                g.DrawString("🕐  Hoạt động gần đây", ThemeColors.SubTitleFont, b, 20, 16);

            // Activity items
            var activities = new[]
            {
                ("Nguyễn Văn Minh", "mượn sách \"Lập trình C# cơ bản\"", "2 giờ trước", ThemeColors.Info),
                ("Trần Thị Lan", "trả sách \"Trí tuệ nhân tạo\"", "5 giờ trước", ThemeColors.Success),
                ("Lê Hoàng Nam", "quá hạn trả sách", "1 ngày trước", ThemeColors.Danger),
                ("Phạm Thị Hoa", "đăng ký thẻ mới", "2 ngày trước", ThemeColors.Primary),
                ("Võ Thanh Tùng", "mượn sách \"Tâm lý học\"", "3 ngày trước", ThemeColors.Info),
            };

            int itemY = 52;
            for (int i = 0; i < activities.Length; i++)
            {
                var (name, action, time, color) = activities[i];

                // Dot
                using (SolidBrush dot = new SolidBrush(color))
                    g.FillEllipse(dot, 24, itemY + 8, 10, 10);

                // Name + action
                using (SolidBrush nb = new SolidBrush(ThemeColors.TextPrimary))
                    g.DrawString(name, new Font("Segoe UI Semibold", 9.5f), nb, 42, itemY);
                using (SolidBrush ab = new SolidBrush(ThemeColors.TextSecondary))
                    g.DrawString(action, ThemeColors.SmallFont, ab, 42, itemY + 20);

                // Time
                using (SolidBrush tb = new SolidBrush(ThemeColors.TextMuted))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Far })
                    g.DrawString(time, ThemeColors.SmallFont, tb, panel.Width - 24, itemY + 4, sf);

                // Divider
                if (i < activities.Length - 1)
                {
                    using (Pen pen = new Pen(ThemeColors.BorderLight))
                        g.DrawLine(pen, 42, itemY + 46, panel.Width - 24, itemY + 46);
                }

                itemY += 52;
            }
        }
    }
}
