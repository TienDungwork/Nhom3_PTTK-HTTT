using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ReaderDashboard : UserControl
    {
        private StatCard[] cards = null!;
        private Panel borrowCard = null!;

        public ReaderDashboard()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            var cu = UserStore.CurrentUser;
            Controls.Add(new Label { Text = $"Xin chào, {cu?.HoTen ?? "Độc giả"}!", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(600, 40), BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right });
            Controls.Add(new Label { Text = "Thông tin mượn sách của bạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            var borrows = SampleData.BorrowRecords.Where(b => b.NgayTraThuc == null).ToList();
            var nearest = borrows.OrderBy(b => b.NgayHenTra).FirstOrDefault();
            string nearestDate = nearest != null ? nearest.NgayHenTra.ToString("dd/MM/yyyy") : "Không có";
            int totalHistory = SampleData.BorrowRecords.Count();

            cards = new StatCard[] {
                new StatCard { Title = "Sách đang mượn", Value = borrows.Count.ToString(), IconText = "\uE736", AccentColor = ColorTranslator.FromHtml("#10B981"), Size = new Size(220, 110) },
                new StatCard { Title = "Hạn trả gần nhất", Value = nearestDate, IconText = "\uE787", AccentColor = ThemeColors.Warning, Size = new Size(220, 110) },
                new StatCard { Title = "Lịch sử mượn", Value = totalHistory.ToString() + " lượt", IconText = "\uE7A8", AccentColor = ThemeColors.Primary, Size = new Size(220, 110) }
            };
            foreach (var c in cards) Controls.Add(c);

            // Currently borrowed books
            borrowCard = new Panel { Location = new Point(32, 230), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            borrowCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, borrowCard.Width - 6, borrowCard.Height - 6), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            borrowCard.Controls.Add(new Label { Text = "Sách đang mượn", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });

            var dgv = new DataGridView { Location = new Point(16, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgv);

            foreach (var b in borrows)
            {
                string status = b.IsOverdue ? "Quá hạn" : "Đang mượn";
                int rowIdx = dgv.Rows.Add(b.TenSach, b.NgayMuon.ToString("dd/MM/yyyy"), b.NgayHenTra.ToString("dd/MM/yyyy"), status);
                if (b.IsOverdue)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }

            if (borrows.Count == 0)
                borrowCard.Controls.Add(new Label { Text = "Bạn chưa mượn sách nào.", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 60), Size = new Size(400, 22), BackColor = Color.Transparent });
            else
                borrowCard.Controls.Add(dgv);

            Controls.Add(borrowCard);

            Resize += (s, e) => LayoutCards();
            Load += (s, e) => LayoutCards();
        }

        private void LayoutCards()
        {
            int margin = 32;
            int gap = 16;
            int availW = ClientSize.Width - margin * 2;
            int cardW = (availW - gap * (cards.Length - 1)) / cards.Length;
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(margin + i * (cardW + gap), 100);
                cards[i].Size = new Size(cardW, 110);
            }
            borrowCard.Location = new Point(margin, 230);
            borrowCard.Size = new Size(availW, ClientSize.Height - 230 - margin);
            // DGV inside borrowCard
            foreach (Control c in borrowCard.Controls)
            {
                if (c is DataGridView dgv)
                    dgv.Size = new Size(borrowCard.Width - 32, borrowCard.Height - 66);
            }
        }
    }
}
