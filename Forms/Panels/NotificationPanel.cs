using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class NotificationPanel : UserControl
    {
        public NotificationPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            int unread = UserStore.Notifications.Count(n => !n.DaDoc);
            Controls.Add(new Label { Text = "THÔNG BÁO", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = $"Bạn có {unread} thông báo chưa đọc", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            var btnMarkAll = new RoundedButton { Text = "Đánh dấu đã đọc tất cả", Size = new Size(220, 38), Location = new Point(32, 96), ButtonColor = ThemeColors.Primary, Font = ThemeColors.SmallFont };
            btnMarkAll.Click += (s, e) =>
            {
                foreach (var n in UserStore.Notifications) n.DaDoc = true;
                Controls.Clear(); InitializeUI();
            };
            Controls.Add(btnMarkAll);

            int y = 150;
            foreach (var notif in UserStore.Notifications.OrderByDescending(n => n.ThoiGian))
            {
                Panel card = new Panel { Location = new Point(32, y), Size = new Size(700, 100), BackColor = Color.Transparent };
                bool isUnread = !notif.DaDoc;
                card.Paint += (s, e) =>
                {
                    var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(2, 2, card.Width - 6, card.Height - 6);
                    using (var path = ThemeColors.GetRoundedRect(rect, 10))
                    using (var bg = new SolidBrush(isUnread ? Color.FromArgb(255, 239, 243, 255) : Color.White))
                        g.FillPath(bg, path);
                    if (isUnread)
                    {
                        using (var accentPath = ThemeColors.GetRoundedRect(new Rectangle(2, 2, 4, card.Height - 6), 2))
                        using (var accentBrush = new SolidBrush(ThemeColors.Primary))
                            g.FillPath(accentBrush, accentPath);
                    }
                };

                string prefix = isUnread ? "🔴 " : "";
                card.Controls.Add(new Label { Text = $"{prefix}{notif.TieuDe}", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 14), Size = new Size(550, 24), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = notif.NoiDung, Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 42), Size = new Size(650, 22), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = notif.ThoiGian.ToString("dd/MM/yyyy HH:mm"), Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 70), Size = new Size(200, 18), BackColor = Color.Transparent });

                Controls.Add(card);
                y += 116;
            }
        }
    }
}
