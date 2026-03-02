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
    public class LibrarianDashboard : UserControl
    {
        private StatCard[] cards = null!;
        private Panel actCard = null!;

        public LibrarianDashboard()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            var cu = UserStore.CurrentUser;
            Controls.Add(new Label { Text = $"Xin chào, {cu?.HoTen ?? "Thủ thư"}!", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(600, 40), BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right });
            Controls.Add(new Label { Text = "Tổng quan hệ thống thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            var books = SampleData.Books;
            var borrows = SampleData.BorrowRecords;
            int totalBooks = books.Sum(b => b.SoLuong);
            int borrowed = borrows.Count(b => b.NgayTraThuc == null && b.TrangThai == "Đang mượn");
            int overdue = borrows.Count(b => b.IsOverdue);

            cards = new StatCard[] {
                new StatCard { Title = "Tổng số sách", Value = totalBooks.ToString(), IconText = "\uE736", AccentColor = ThemeColors.Primary, Size = new Size(220, 110) },
                new StatCard { Title = "Đang cho mượn", Value = borrowed.ToString(), IconText = "\uE736", AccentColor = ThemeColors.Warning, Size = new Size(220, 110) },
                new StatCard { Title = "Sách quá hạn", Value = overdue.ToString(), IconText = "\uE7BA", AccentColor = ThemeColors.Danger, Size = new Size(220, 110) },
                new StatCard { Title = "Tổng độc giả", Value = SampleData.Readers.Count.ToString(), IconText = "\uE716", AccentColor = ThemeColors.Success, Size = new Size(220, 110) }
            };
            foreach (var c in cards) Controls.Add(c);

            // Recent activity
            actCard = new Panel { BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            actCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, actCard.Width - 6, actCard.Height - 6), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            actCard.Controls.Add(new Label { Text = "Phiếu mượn gần đây", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(400, 28), BackColor = Color.Transparent });

            var dgv = new DataGridView { Location = new Point(16, 50), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("TenDocGia", "Độc giả");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgv);

            foreach (var r in borrows.OrderByDescending(r => r.NgayMuon).Take(10))
            {
                int rowIdx = dgv.Rows.Add(r.MaMuon, r.TenDocGia, r.TenSach, r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"), r.TrangThai);
                if (r.IsOverdue)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }
            actCard.Controls.Add(dgv);
            Controls.Add(actCard);

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
            actCard.Location = new Point(margin, 230);
            actCard.Size = new Size(availW, ClientSize.Height - 230 - margin);
            foreach (Control c in actCard.Controls)
            {
                if (c is DataGridView dgv)
                    dgv.Size = new Size(actCard.Width - 32, actCard.Height - 66);
            }
        }
    }
}
