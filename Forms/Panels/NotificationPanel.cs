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
    public class NotificationPanel : UserControl
    {
        private ComboBox cboFilter = null!;

        public NotificationPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            string maDocGia = UserStore.CurrentUser?.MaDocGia ?? "";
            var notifications = UserStore.Notifications
                .Where(n => n.MaDocGia == maDocGia)
                .OrderByDescending(n => n.ThoiGian)
                .ToList();

            int unread = notifications.Count(n => !n.DaDoc);

            Controls.Add(new Label
            {
                Text = "THÔNG BÁO",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = $"Bạn có {unread} thông báo chưa đọc",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 62),
                Size = new Size(520, 22),
                BackColor = Color.Transparent
            });

            cboFilter = new ComboBox
            {
                Location = new Point(520, 96),
                Size = new Size(212, 36),
                Font = ThemeColors.SmallFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFilter.Items.AddRange(new object[] { "Tất cả", "Chưa đọc", "Đã đọc", "Quá hạn", "Mượn/Trả", "Nhắc hạn" });
            cboFilter.SelectedIndex = 0;
            cboFilter.SelectedIndexChanged += (_, _) => RefreshUI();
            Controls.Add(cboFilter);

            var btnMarkAll = new RoundedButton
            {
                Text = "Đánh dấu đã đọc tất cả",
                Size = new Size(240, 38),
                Location = new Point(32, 96),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.SmallFont
            };
            btnMarkAll.Click += (_, _) =>
            {
                foreach (var n in notifications) n.DaDoc = true;
                RefreshUI();
            };
            Controls.Add(btnMarkAll);

            int y = 150;
            foreach (var notif in ApplyFilter(notifications))
            {
                Controls.Add(CreateNotificationCard(notif, y));
                y += 122;
            }
        }

        private Panel CreateNotificationCard(Notification notif, int y)
        {
            bool isUnread = !notif.DaDoc;

            Panel card = new Panel
            {
                Location = new Point(32, y),
                Size = new Size(740, 110),
                BackColor = Color.Transparent
            };

            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

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

            // Nút tròn để tích/đánh dấu đã đọc
            var readBtn = new CircleReadButton
            {
                Location = new Point(14, 18),
                Size = new Size(22, 22),
                IsRead = !isUnread
            };
            readBtn.Click += (_, _) =>
            {
                // Toggle trạng thái đã đọc/chưa đọc
                notif.DaDoc = !notif.DaDoc;
                RefreshUI();
            };
            card.Controls.Add(readBtn);

            // Click vào nội dung cũng đánh dấu "đã đọc" để dễ thao tác
            void MarkReadByCardClick(object? _, EventArgs __)
            {
                if (notif.DaDoc) return;
                notif.DaDoc = true;
                RefreshUI();
            }
            card.Click += MarkReadByCardClick;

            string prefix = isUnread ? "[Mới] " : "";

            var lblTitle = new Label
            {
                Text = $"{prefix}{notif.TieuDe}",
                Font = ThemeColors.SubTitleFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(46, 14),
                Size = new Size(650, 24),
                BackColor = Color.Transparent
            };
            var lblBody = new Label
            {
                Text = notif.NoiDung,
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(46, 42),
                Size = new Size(680, 22),
                BackColor = Color.Transparent
            };
            var lblMeta = new Label
            {
                Text = $"{notif.LoaiThongBao} | {notif.ThoiGian:dd/MM/yyyy HH:mm}",
                Font = ThemeColors.SmallFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(46, 70),
                Size = new Size(420, 18),
                BackColor = Color.Transparent
            };
            lblTitle.Click += MarkReadByCardClick;
            lblBody.Click += MarkReadByCardClick;
            lblMeta.Click += MarkReadByCardClick;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblBody);
            card.Controls.Add(lblMeta);

            return card;
        }

        private void RefreshUI()
        {
            Controls.Clear();
            InitializeUI();
        }

        private IEnumerable<Notification> ApplyFilter(IEnumerable<Notification> notifications)
        {
            string filter = cboFilter?.SelectedItem?.ToString() ?? "Tất cả";
            return filter switch
            {
                "Chưa đọc" => notifications.Where(n => !n.DaDoc),
                "Đã đọc" => notifications.Where(n => n.DaDoc),
                "Quá hạn" => notifications.Where(n => n.LoaiThongBao == "QuaHan"),
                "Mượn/Trả" => notifications.Where(n => n.LoaiThongBao == "MuonTra"),
                "Nhắc hạn" => notifications.Where(n => n.LoaiThongBao == "NhacHan"),
                _ => notifications
            };
        }

        private sealed class CircleReadButton : Control
        {
            private bool _isRead;
            public bool IsRead
            {
                get => _isRead;
                set { _isRead = value; Invalidate(); }
            }

            public CircleReadButton()
            {
                DoubleBuffered = true;
                Cursor = Cursors.Hand;
                TabStop = false;
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                using var path = new GraphicsPath();
                path.AddEllipse(0, 0, Width - 1, Height - 1);
                Region = new Region(path);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var fill = IsRead ? Color.FromArgb(226, 232, 240) : ThemeColors.Primary;
                var border = IsRead ? Color.FromArgb(148, 163, 184) : ThemeColors.Primary;

                using var brush = new SolidBrush(fill);
                using var pen = new Pen(border, 2);
                g.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
                g.DrawEllipse(pen, 1, 1, Width - 3, Height - 3);

                if (IsRead)
                {
                    using var tickPen = new Pen(Color.White, 2.2f)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    };
                    Point p1 = new Point((int)(Width * 0.25), (int)(Height * 0.55));
                    Point p2 = new Point((int)(Width * 0.45), (int)(Height * 0.72));
                    Point p3 = new Point((int)(Width * 0.78), (int)(Height * 0.3));
                    g.DrawLines(tickPen, new[] { p1, p2, p3 });
                }
            }
        }
    }
}

