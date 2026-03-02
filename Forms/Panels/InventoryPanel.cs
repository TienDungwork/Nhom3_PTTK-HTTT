using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class InventoryPanel : UserControl
    {
        private DataGridView dgv = null!;

        public InventoryPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "KIỂM KÊ KHO", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Kiểm tra và cập nhật số lượng sách trong kho", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Summary stats
            var books = SampleData.Books;
            int total = books.Sum(b => b.SoLuong);
            int categories = SampleData.Categories.Count;
            int outOfStock = books.Count(b => b.SoLuong == 0 || b.TrangThai == "Hết sách");

            var card1 = new StatCard { Title = "Tổng đầu sách", Value = books.Count.ToString(), IconText = "\uE736", AccentColor = ThemeColors.Primary, Location = new Point(32, 96), Size = new Size(200, 100) };
            var card2 = new StatCard { Title = "Tổng số bản", Value = total.ToString(), IconText = "\uE736", AccentColor = ThemeColors.Success, Location = new Point(248, 96), Size = new Size(200, 100) };
            var card3 = new StatCard { Title = "Thể loại", Value = categories.ToString(), IconText = "\uE8EC", AccentColor = ThemeColors.Warning, Location = new Point(464, 96), Size = new Size(200, 100) };
            var card4 = new StatCard { Title = "Hết sách", Value = outOfStock.ToString(), IconText = "\uE7BA", AccentColor = ThemeColors.Danger, Location = new Point(680, 96), Size = new Size(200, 100) };
            Controls.Add(card1); Controls.Add(card2); Controls.Add(card3); Controls.Add(card4);

            // Inventory table
            dgv = new DataGridView { Location = new Point(32, 212), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("MaSach", "Mã sách");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("TheLoai", "Thể loại");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLuong", HeaderText = "Số lượng", Width = 100 });
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns["TenSach"]!.FillWeight = 200;

            foreach (var book in books)
            {
                dgv.Rows.Add(book.MaSach, book.TenSach, book.TheLoai, book.SoLuong, book.TrangThai);
            }

            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns["TrangThai"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val == "Hết sách")
                    {
                        e.CellStyle.ForeColor = ThemeColors.Danger;
                        e.CellStyle.Font = new Font(ThemeColors.BodyFont, FontStyle.Bold);
                    }
                    else
                    {
                        e.CellStyle.ForeColor = ThemeColors.Success;
                    }
                }
            };

            Controls.Add(dgv);

            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 230);
            dgv.Size = new Size(Width - 64, Height - 230);
        }
    }
}
