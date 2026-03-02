using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Forms.Dialogs;

namespace LibraryManagement.Forms.Panels
{
    public class SearchBookPanel : UserControl
    {
        private TextBox txtTacGia = null!;
        private TextBox txtNhanDe = null!;
        private TextBox txtChuDe = null!;
        private TextBox txtMaSach = null!;
        private DataGridView dgvResults = null!;

        public SearchBookPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            // Title
            Controls.Add(new Label { Text = "TÌM KIẾM SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tra cứu sách theo tác giả, nhan đề, chủ đề hoặc mã sách", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(600, 22), BackColor = Color.Transparent });

            // Search card
            var searchCard = new Panel { Location = new Point(32, 96), Size = new Size(900, 160), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            searchCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, searchCard.Width - 2, searchCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            // Row 1
            searchCard.Controls.Add(new Label { Text = "Tác giả", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 14), Size = new Size(200, 18), BackColor = Color.Transparent });
            txtTacGia = new TextBox { Location = new Point(20, 34), Size = new Size(200, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            searchCard.Controls.Add(txtTacGia);

            searchCard.Controls.Add(new Label { Text = "Nhan đề", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(240, 14), Size = new Size(200, 18), BackColor = Color.Transparent });
            txtNhanDe = new TextBox { Location = new Point(240, 34), Size = new Size(200, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            searchCard.Controls.Add(txtNhanDe);

            searchCard.Controls.Add(new Label { Text = "Chủ đề", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(460, 14), Size = new Size(200, 18), BackColor = Color.Transparent });
            txtChuDe = new TextBox { Location = new Point(460, 34), Size = new Size(200, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            searchCard.Controls.Add(txtChuDe);

            searchCard.Controls.Add(new Label { Text = "Mã sách", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(680, 14), Size = new Size(160, 18), BackColor = Color.Transparent });
            txtMaSach = new TextBox { Location = new Point(680, 34), Size = new Size(160, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            searchCard.Controls.Add(txtMaSach);

            // Search button
            var btnSearch = new RoundedButton { Text = "Tìm kiếm", Size = new Size(160, 44), Location = new Point(20, 86), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnSearch.Click += BtnSearch_Click;
            searchCard.Controls.Add(btnSearch);

            var btnClear = new RoundedButton { Text = "Xóa bộ lọc", Size = new Size(140, 44), Location = new Point(196, 86), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (s, e) => { txtTacGia.Clear(); txtNhanDe.Clear(); txtChuDe.Clear(); txtMaSach.Clear(); dgvResults.Rows.Clear(); };
            searchCard.Controls.Add(btnClear);

            Controls.Add(searchCard);

            // Results DGV
            dgvResults = new DataGridView { Location = new Point(32, 270), Size = new Size(900, 400), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvResults.Columns.Add("MaSach", "Mã sách");
            dgvResults.Columns.Add("TenSach", "Nhan đề");
            dgvResults.Columns.Add("TacGia", "Tác giả");
            dgvResults.Columns.Add("ChuDe", "Chủ đề");
            dgvResults.Columns.Add("NXB", "NXB");
            dgvResults.Columns.Add("SoLuong", "Còn lại");
            dgvResults.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvResults);
            dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;
            Controls.Add(dgvResults);

            Controls.Add(new Label { Text = "Nhấp đúp vào sách để xem chi tiết", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextMuted, Location = new Point(32, 675), Size = new Size(400, 18), BackColor = Color.Transparent, Anchor = AnchorStyles.Bottom | AnchorStyles.Left });
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            dgvResults.Rows.Clear();
            var results = SampleData.Books.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtTacGia.Text))
                results = results.Where(b => b.TacGia.Contains(txtTacGia.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtNhanDe.Text))
                results = results.Where(b => b.TenSach.Contains(txtNhanDe.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtChuDe.Text))
                results = results.Where(b => b.ChuDe.Contains(txtChuDe.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtMaSach.Text))
                results = results.Where(b => b.MaSach.Contains(txtMaSach.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            var list = results.ToList();
            if (list.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var b in list)
            {
                dgvResults.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.ChuDe, b.NhaXuatBan, b.SoLuongHienCo, b.TrangThai);
            }
        }

        private void DgvResults_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maSach = dgvResults.Rows[e.RowIndex].Cells["MaSach"].Value?.ToString() ?? "";
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book != null)
            {
                var dialog = new BookInfoDialog(book);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(FindForm());
            }
        }
    }
}
