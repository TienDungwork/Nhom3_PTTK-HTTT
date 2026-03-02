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
    public class ClassifyPanel : UserControl
    {
        private ComboBox cboTheLoai = null!;
        private TextBox txtTacGia = null!;
        private DataGridView dgvResults = null!;

        public ClassifyPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "PHÂN LOẠI SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Lọc sách theo thể loại và tác giả", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Filter card
            var filterCard = new Panel { Location = new Point(32, 92), Size = new Size(920, 80), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            filterCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, filterCard.Width - 2, filterCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            filterCard.Controls.Add(new Label { Text = "Thể loại", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(16, 8), Size = new Size(100, 16), BackColor = Color.Transparent });
            cboTheLoai = new ComboBox { Location = new Point(16, 28), Size = new Size(200, 32), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in SampleData.Categories) cboTheLoai.Items.Add(c);
            cboTheLoai.SelectedIndex = 0;
            cboTheLoai.SelectedIndexChanged += (s, e) => ApplyFilter();
            filterCard.Controls.Add(cboTheLoai);

            filterCard.Controls.Add(new Label { Text = "Tác giả", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(236, 8), Size = new Size(100, 16), BackColor = Color.Transparent });
            txtTacGia = new TextBox { Location = new Point(236, 28), Size = new Size(200, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            txtTacGia.TextChanged += (s, e) => ApplyFilter();
            filterCard.Controls.Add(txtTacGia);

            var btnClear = new RoundedButton { Text = "Xóa bộ lọc", Size = new Size(120, 36), Location = new Point(460, 26), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (s, e) => { cboTheLoai.SelectedIndex = 0; txtTacGia.Clear(); };
            filterCard.Controls.Add(btnClear);

            Controls.Add(filterCard);

            // Results
            dgvResults = new DataGridView { Location = new Point(32, 184), Size = new Size(920, 470), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvResults.Columns.Add("MaSach", "Mã sách");
            dgvResults.Columns.Add("TenSach", "Nhan đề");
            dgvResults.Columns.Add("TacGia", "Tác giả");
            dgvResults.Columns.Add("TheLoai", "Thể loại");
            dgvResults.Columns.Add("ChuDe", "Chủ đề");
            dgvResults.Columns.Add("NamXB", "Năm XB");
            dgvResults.Columns.Add("SoLuong", "Số lượng");
            dgvResults.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvResults);
            Controls.Add(dgvResults);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            dgvResults.Rows.Clear();
            var results = SampleData.Books.AsEnumerable();

            string theLoai = cboTheLoai.SelectedItem?.ToString() ?? "Tất cả";
            if (theLoai != "Tất cả")
                results = results.Where(b => b.TheLoai == theLoai);

            if (!string.IsNullOrWhiteSpace(txtTacGia.Text))
                results = results.Where(b => b.TacGia.Contains(txtTacGia.Text.Trim(), StringComparison.OrdinalIgnoreCase));

            foreach (var b in results)
                dgvResults.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.TheLoai, b.ChuDe, b.NamXuatBan, b.SoLuong, b.TrangThai);
        }
    }
}
