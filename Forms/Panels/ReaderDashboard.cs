using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ReaderDashboard : UserControl
    {
        public ReaderDashboard()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            var cu = UserStore.CurrentUser;
            Controls.Add(new Label { Text = $"Xin chào, {cu?.HoTen ?? "Độc giả"}!", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(600, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Thông tin mượn sách của bạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            var borrows = SampleData.BorrowRecords.Where(b => b.NgayTraThuc == null).ToList();
            var nearest = borrows.OrderBy(b => b.NgayHenTra).FirstOrDefault();
            string nearestDate = nearest != null ? nearest.NgayHenTra.ToString("dd/MM/yyyy") : "Không có";
            int totalHistory = SampleData.BorrowRecords.Count();

            var card1 = new StatCard { Title = "Sách đang mượn", Value = borrows.Count.ToString(), IconText = "\uE736", AccentColor = ColorTranslator.FromHtml("#10B981"), Location = new Point(32, 100), Size = new Size(240, 110) };
            var card2 = new StatCard { Title = "Hạn trả gần nhất", Value = nearestDate, IconText = "\uE787", AccentColor = ThemeColors.Warning, Location = new Point(290, 100), Size = new Size(240, 110) };
            var card3 = new StatCard { Title = "Lịch sử mượn", Value = totalHistory.ToString() + " lượt", IconText = "\uE7A8", AccentColor = ThemeColors.Primary, Location = new Point(548, 100), Size = new Size(240, 110) };
            Controls.Add(card1); Controls.Add(card2); Controls.Add(card3);

            // Currently borrowed books
            Panel borrowCard = new Panel { Location = new Point(32, 230), Size = new Size(760, 300), BackColor = Color.Transparent };
            borrowCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, borrowCard.Width - 6, borrowCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            borrowCard.Controls.Add(new Label { Text = "Sách đang mượn", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });

            int by = 52;
            foreach (var b in borrows)
            {
                string bookName = SampleData.Books.FirstOrDefault(bk => bk.MaSach == b.MaSach)?.TenSach ?? b.MaSach;
                string status = b.IsOverdue ? "⚠️ Quá hạn" : "✅ Đang mượn";
                Color statusColor = b.IsOverdue ? ThemeColors.Danger : ThemeColors.Success;

                borrowCard.Controls.Add(new Label { Text = $"• {bookName}", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, by), Size = new Size(400, 22), BackColor = Color.Transparent });
                borrowCard.Controls.Add(new Label { Text = $"Hạn trả: {b.NgayHenTra:dd/MM/yyyy}", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(430, by + 2), Size = new Size(160, 20), BackColor = Color.Transparent });
                borrowCard.Controls.Add(new Label { Text = status, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = statusColor, Location = new Point(600, by + 2), Size = new Size(140, 20), BackColor = Color.Transparent });
                by += 40;
            }

            if (borrows.Count() == 0)
                borrowCard.Controls.Add(new Label { Text = "Bạn chưa mượn sách nào.", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, by), Size = new Size(400, 22), BackColor = Color.Transparent });

            Controls.Add(borrowCard);

            // Notifications preview
            Panel notifCard = new Panel { Location = new Point(32, 548), Size = new Size(760, 160), BackColor = Color.Transparent };
            notifCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, notifCard.Width - 6, notifCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            int unread = UserStore.Notifications.Count(n => !n.DaDoc);
            notifCard.Controls.Add(new Label { Text = $"Thông báo ({unread} chưa đọc)", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });
            int ny = 52;
            foreach (var n in UserStore.Notifications.Take(3))
            {
                string prefix = n.DaDoc ? "" : "🔴 ";
                notifCard.Controls.Add(new Label { Text = $"{prefix}{n.TieuDe}", Font = ThemeColors.BodyFont, ForeColor = n.DaDoc ? ThemeColors.TextSecondary : ThemeColors.TextPrimary, Location = new Point(20, ny), Size = new Size(700, 22), BackColor = Color.Transparent });
                ny += 30;
            }
            Controls.Add(notifCard);
        }
    }
}
