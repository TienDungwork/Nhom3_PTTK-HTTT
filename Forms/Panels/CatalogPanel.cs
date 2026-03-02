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
    public class CatalogPanel : UserControl
    {
        private TextBox txtMaSach = null!, txtTenSach = null!, txtTacGia = null!, txtChuDe = null!;
        private TextBox txtNXB = null!, txtISBN = null!, txtURI = null!, txtNamXB = null!;
        private DataGridView dgvBooks = null!;

        public CatalogPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "CẬP NHẬT DANH MỤC SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(500, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Thêm, sửa, xóa sách trong hệ thống", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Input card
            var inputCard = new Panel { Location = new Point(32, 92), Size = new Size(920, 220), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            inputCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, inputCard.Width - 2, inputCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            // Row 1
            AddInputField(inputCard, "Mã sách", 16, 12, 130, out txtMaSach);
            AddInputField(inputCard, "Tên sách", 162, 12, 250, out txtTenSach);
            AddInputField(inputCard, "Tác giả", 428, 12, 180, out txtTacGia);
            AddInputField(inputCard, "Chủ đề", 624, 12, 140, out txtChuDe);
            AddInputField(inputCard, "Năm XB", 780, 12, 80, out txtNamXB);

            // Row 2
            AddInputField(inputCard, "NXB", 16, 74, 200, out txtNXB);
            AddInputField(inputCard, "ISBN", 232, 74, 200, out txtISBN);
            AddInputField(inputCard, "URI", 448, 74, 260, out txtURI);

            // Buttons
            var btnThem = new RoundedButton { Text = "Thêm", Size = new Size(100, 40), Location = new Point(16, 148), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnThem.Click += BtnThem_Click;
            inputCard.Controls.Add(btnThem);

            var btnSua = new RoundedButton { Text = "Sửa", Size = new Size(100, 40), Location = new Point(126, 148), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnSua.Click += BtnSua_Click;
            inputCard.Controls.Add(btnSua);

            var btnXoa = new RoundedButton { Text = "Xóa", Size = new Size(100, 40), Location = new Point(236, 148), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnXoa.Click += BtnXoa_Click;
            inputCard.Controls.Add(btnXoa);

            var btnClear = new RoundedButton { Text = "Xóa form", Size = new Size(100, 40), Location = new Point(346, 148), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (s, e) => ClearForm();
            inputCard.Controls.Add(btnClear);

            Controls.Add(inputCard);

            // DGV
            dgvBooks = new DataGridView { Location = new Point(32, 324), Size = new Size(920, 340), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvBooks.Columns.Add("MaSach", "Mã sách");
            dgvBooks.Columns.Add("TenSach", "Tên sách");
            dgvBooks.Columns.Add("TacGia", "Tác giả");
            dgvBooks.Columns.Add("ChuDe", "Chủ đề");
            dgvBooks.Columns.Add("NamXB", "Năm XB");
            dgvBooks.Columns.Add("NXB", "NXB");
            dgvBooks.Columns.Add("ISBN", "ISBN");
            dgvBooks.Columns.Add("SoLuong", "Số lượng");
            ModernDataGridView.ApplyStyle(dgvBooks);
            dgvBooks.CellClick += DgvBooks_CellClick;
            LoadBooks();
            Controls.Add(dgvBooks);
        }

        private void AddInputField(Panel parent, string label, int x, int y, int width, out TextBox textBox)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            textBox = new TextBox { Location = new Point(x, y + 18), Size = new Size(width, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(textBox);
        }

        private void LoadBooks()
        {
            dgvBooks.Rows.Clear();
            foreach (var b in SampleData.Books)
                dgvBooks.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.ChuDe, b.NamXuatBan, b.NhaXuatBan, b.ISBN, b.SoLuong);
        }

        private void DgvBooks_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvBooks.Rows[e.RowIndex];
            txtMaSach.Text = row.Cells["MaSach"].Value?.ToString() ?? "";
            txtTenSach.Text = row.Cells["TenSach"].Value?.ToString() ?? "";
            txtTacGia.Text = row.Cells["TacGia"].Value?.ToString() ?? "";
            txtChuDe.Text = row.Cells["ChuDe"].Value?.ToString() ?? "";
            txtNamXB.Text = row.Cells["NamXB"].Value?.ToString() ?? "";
            txtNXB.Text = row.Cells["NXB"].Value?.ToString() ?? "";
            txtISBN.Text = row.Cells["ISBN"].Value?.ToString() ?? "";
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSach.Text) || string.IsNullOrWhiteSpace(txtTenSach.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã sách và Tên sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (SampleData.Books.Any(b => b.MaSach == txtMaSach.Text.Trim()))
            {
                MessageBox.Show("Mã sách đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newBook = new Book
            {
                MaSach = txtMaSach.Text.Trim(),
                TenSach = txtTenSach.Text.Trim(),
                TacGia = txtTacGia.Text.Trim(),
                ChuDe = txtChuDe.Text.Trim(),
                NamXuatBan = int.TryParse(txtNamXB.Text, out int nam) ? nam : 2024,
                NhaXuatBan = txtNXB.Text.Trim(),
                ISBN = txtISBN.Text.Trim(),
                URI = txtURI.Text.Trim(),
                SoLuong = 1
            };
            SampleData.Books.Add(newBook);
            MessageBox.Show("Thêm sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadBooks();
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == txtMaSach.Text.Trim());
            if (book == null)
            {
                MessageBox.Show("Không tìm thấy sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            book.TenSach = txtTenSach.Text.Trim();
            book.TacGia = txtTacGia.Text.Trim();
            book.ChuDe = txtChuDe.Text.Trim();
            book.NamXuatBan = int.TryParse(txtNamXB.Text, out int nam) ? nam : book.NamXuatBan;
            book.NhaXuatBan = txtNXB.Text.Trim();
            book.ISBN = txtISBN.Text.Trim();
            book.URI = txtURI.Text.Trim();
            MessageBox.Show("Cập nhật thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBooks();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == txtMaSach.Text.Trim());
            if (book == null)
            {
                MessageBox.Show("Không tìm thấy sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sách \"{book.TenSach}\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                SampleData.Books.Remove(book);
                MessageBox.Show("Xóa thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadBooks();
            }
        }

        private void ClearForm()
        {
            txtMaSach.Clear(); txtTenSach.Clear(); txtTacGia.Clear(); txtChuDe.Clear();
            txtNamXB.Clear(); txtNXB.Clear(); txtISBN.Clear(); txtURI.Clear();
        }
    }
}
