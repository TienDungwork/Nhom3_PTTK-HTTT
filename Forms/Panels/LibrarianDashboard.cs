using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class LibrarianDashboard : UserControl
    {
        public LibrarianDashboard()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Title
            Controls.Add(new Label { Text = "TRANG CHỦ", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tổng quan hoạt động thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            // Stat cards
            var books = SampleData.Books;
            var borrows = SampleData.BorrowRecords;
            int totalBooks = books.Sum(b => b.SoLuong);
            int borrowed = borrows.Count(b => b.NgayTraThuc == null);
            int overdue = borrows.Count(b => b.IsOverdue);

            var card1 = new StatCard { Title = "Tổng số sách", Value = totalBooks.ToString(), IconText = "📚", AccentColor = ThemeColors.Primary, Location = new Point(32, 100), Size = new Size(220, 110) };
            var card2 = new StatCard { Title = "Đang cho mượn", Value = borrowed.ToString(), IconText = "📖", AccentColor = ThemeColors.Warning, Location = new Point(268, 100), Size = new Size(220, 110) };
            var card3 = new StatCard { Title = "Sách quá hạn", Value = overdue.ToString(), IconText = "⚠️", AccentColor = ThemeColors.Danger, Location = new Point(504, 100), Size = new Size(220, 110) };
            var card4 = new StatCard { Title = "Tổng độc giả", Value = SampleData.Readers.Count.ToString(), IconText = "👥", AccentColor = ThemeColors.Success, Location = new Point(740, 100), Size = new Size(220, 110) };
            Controls.Add(card1); Controls.Add(card2); Controls.Add(card3); Controls.Add(card4);

            // Recent activity
            Panel activityCard = new Panel { Location = new Point(32, 230), Size = new Size(700, 340), BackColor = Color.Transparent };
            activityCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, activityCard.Width - 6, activityCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };

            activityCard.Controls.Add(new Label { Text = "🕐  Hoạt động gần đây", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });

            int ay = 52;
            foreach (var log in UserStore.Logs.OrderByDescending(l => l.ThoiGian).Take(6))
            {
                string timeAgo = FormatTimeAgo(log.ThoiGian);
                activityCard.Controls.Add(new Label { Text = $"• {log.HanhDong}: {log.ChiTiet}", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, ay), Size = new Size(500, 22), BackColor = Color.Transparent });
                activityCard.Controls.Add(new Label { Text = timeAgo, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(530, ay + 2), Size = new Size(150, 20), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });
                ay += 44;
            }
            Controls.Add(activityCard);

            // Quick actions
            Panel quickCard = new Panel { Location = new Point(750, 230), Size = new Size(220, 340), BackColor = Color.Transparent };
            quickCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, quickCard.Width - 6, quickCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            quickCard.Controls.Add(new Label { Text = "⚡  Thao tác nhanh", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(16, 16), Size = new Size(190, 28), BackColor = Color.Transparent });

            var btnAddBook = new RoundedButton { Text = "Thêm sách mới", Size = new Size(188, 42), Location = new Point(16, 56), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            var btnBorrow = new RoundedButton { Text = "Cho mượn sách", Size = new Size(188, 42), Location = new Point(16, 110), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            var btnReturn = new RoundedButton { Text = "Nhận trả sách", Size = new Size(188, 42), Location = new Point(16, 164), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            quickCard.Controls.Add(btnAddBook); quickCard.Controls.Add(btnBorrow); quickCard.Controls.Add(btnReturn);
            Controls.Add(quickCard);
        }

        private string FormatTimeAgo(DateTime time)
        {
            var diff = DateTime.Now - time;
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} phút trước";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} giờ trước";
            return $"{(int)diff.TotalDays} ngày trước";
        }
    }
}
