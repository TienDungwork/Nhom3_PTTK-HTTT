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
    public class BookTitlePanel : UserControl
    {
        private const int HorizontalMargin = 32;
        private TextBox txtMaSach = null!, txtTenSach = null!, txtTacGia = null!, txtChuDe = null!;
        private TextBox txtNXB = null!, txtISBN = null!, txtURI = null!, txtNamXB = null!, txtSoLuong = null!;
        private ComboBox cboDanhMuc = null!;
        private DataGridView dgvBooks = null!;
        private Panel inputCard = null!;

        public BookTitlePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            Controls.Add(new Label { Text = "QUẢN LÝ ĐẦU SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(420, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Quản lý thông tin thư mục của từng đầu sách và gắn với danh mục nghiệp vụ", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(700, 22), BackColor = Color.Transparent });

            inputCard = new Panel { Location = new Point(HorizontalMargin, 92), Size = new Size(1040, 220), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            inputCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, inputCard.Width - 2, inputCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            AddInputField(inputCard, "Mã sách", 16, 12, 120, out txtMaSach);
            AddInputField(inputCard, "Tên sách", 152, 12, 260, out txtTenSach);
            AddInputField(inputCard, "Tác giả", 428, 12, 180, out txtTacGia);
            AddComboField(inputCard, "Danh mục", 624, 12, 170, out cboDanhMuc);
            AddInputField(inputCard, "Năm XB", 810, 12, 80, out txtNamXB);

            AddInputField(inputCard, "Chủ đề", 16, 74, 200, out txtChuDe);
            AddInputField(inputCard, "NXB", 232, 74, 170, out txtNXB);
            AddInputField(inputCard, "ISBN", 418, 74, 170, out txtISBN);
            AddInputField(inputCard, "Số lượng", 604, 74, 100, out txtSoLuong);
            AddInputField(inputCard, "URI", 720, 74, 286, out txtURI);

            var btnThem = new RoundedButton { Text = "Thêm", Size = new Size(100, 40), Location = new Point(16, 148), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnThem.Click += (_, _) => SaveBook(false);
            inputCard.Controls.Add(btnThem);

            var btnSua = new RoundedButton { Text = "Cập nhật", Size = new Size(110, 40), Location = new Point(126, 148), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnSua.Click += (_, _) => SaveBook(true);
            inputCard.Controls.Add(btnSua);

            var btnXoa = new RoundedButton { Text = "Xóa", Size = new Size(100, 40), Location = new Point(246, 148), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnXoa.Click += BtnXoa_Click;
            inputCard.Controls.Add(btnXoa);

            var btnClear = new RoundedButton { Text = "Xóa form", Size = new Size(100, 40), Location = new Point(356, 148), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (_, _) => ClearForm();
            inputCard.Controls.Add(btnClear);

            var btnTim = new RoundedButton { Text = "Tìm kiếm", Size = new Size(110, 40), Location = new Point(486, 148), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnTim.Click += (_, _) => SearchBooksByInputs();
            inputCard.Controls.Add(btnTim);

            Controls.Add(inputCard);

            dgvBooks = new DataGridView { Location = new Point(HorizontalMargin, 324), Size = new Size(1040, 332), Anchor = AnchorStyles.Top | AnchorStyles.Left };
            dgvBooks.Columns.Add("MaSach", "Mã sách");
            dgvBooks.Columns.Add("TenSach", "Tên sách");
            dgvBooks.Columns.Add("TacGia", "Tác giả");
            dgvBooks.Columns.Add("DanhMuc", "Danh mục");
            dgvBooks.Columns.Add("ChuDe", "Chủ đề");
            dgvBooks.Columns.Add("NamXB", "Năm XB");
            dgvBooks.Columns.Add("SoLuong", "Tổng số lượng");
            dgvBooks.Columns.Add("DangMuon", "Đang mượn");
            dgvBooks.Columns.Add("ConLai", "Hiện có");
            ModernDataGridView.ApplyStyle(dgvBooks);
            // Responsive layout: grow/shrink with form width, keep quantity columns visible.
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.ScrollBars = ScrollBars.Both;
            dgvBooks.Columns["MaSach"].MinimumWidth = 80;
            dgvBooks.Columns["TenSach"].MinimumWidth = 180;
            dgvBooks.Columns["TacGia"].MinimumWidth = 130;
            dgvBooks.Columns["DanhMuc"].MinimumWidth = 120;
            dgvBooks.Columns["ChuDe"].MinimumWidth = 120;
            dgvBooks.Columns["NamXB"].MinimumWidth = 80;
            dgvBooks.Columns["SoLuong"].MinimumWidth = 100;
            dgvBooks.Columns["DangMuon"].MinimumWidth = 95;
            dgvBooks.Columns["ConLai"].MinimumWidth = 90;

            dgvBooks.Columns["MaSach"].FillWeight = 8;
            dgvBooks.Columns["TenSach"].FillWeight = 20;
            dgvBooks.Columns["TacGia"].FillWeight = 14;
            dgvBooks.Columns["DanhMuc"].FillWeight = 12;
            dgvBooks.Columns["ChuDe"].FillWeight = 12;
            dgvBooks.Columns["NamXB"].FillWeight = 8;
            dgvBooks.Columns["SoLuong"].FillWeight = 10;
            dgvBooks.Columns["DangMuon"].FillWeight = 8;
            dgvBooks.Columns["ConLai"].FillWeight = 8;
            dgvBooks.CellClick += DgvBooks_CellClick;
            Controls.Add(dgvBooks);

            ReloadCategoryOptions();
            LoadBooks();

            Resize += (_, _) => ApplyResponsiveLayout();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            int availableWidth = Math.Max(900, ClientSize.Width - (HorizontalMargin * 2));
            inputCard.Width = availableWidth;
            dgvBooks.Width = availableWidth;

            int availableHeight = Math.Max(260, ClientSize.Height - dgvBooks.Top - 16);
            dgvBooks.Height = availableHeight;
        }

        private void AddInputField(Panel parent, string label, int x, int y, int width, out TextBox textBox)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            textBox = new TextBox { Location = new Point(x, y + 18), Size = new Size(width, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(textBox);
        }

        private void AddComboField(Panel parent, string label, int x, int y, int width, out ComboBox comboBox)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            comboBox = new ComboBox
            {
                Location = new Point(x, y + 18),
                Size = new Size(width, 28),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            parent.Controls.Add(comboBox);
        }

        private void ReloadCategoryOptions()
        {
            var selected = cboDanhMuc.SelectedValue?.ToString();
            cboDanhMuc.DataSource = LibraryDataService.GetCategories(false).ToList();
            cboDanhMuc.DisplayMember = "TenDanhMuc";
            cboDanhMuc.ValueMember = "MaDanhMuc";

            if (!string.IsNullOrWhiteSpace(selected))
                cboDanhMuc.SelectedValue = selected;
        }

        private void LoadBooks(string keyword = "")
        {
            dgvBooks.Rows.Clear();
            foreach (var b in LibraryDataService.SearchBooks(keyword: keyword))
            {
                LibraryDataService.SyncBookCategory(b);
                LibraryDataService.SyncBookStatus(b);
                dgvBooks.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.TheLoai, b.ChuDe, b.NamXuatBan, b.SoLuong, b.SoLuongDangMuon, b.SoLuongHienCo);
            }
        }

        private void SearchBooksByInputs()
        {
            string maSach = txtMaSach.Text.Trim();
            string tenSach = txtTenSach.Text.Trim();
            string tacGia = txtTacGia.Text.Trim();
            string chuDe = txtChuDe.Text.Trim();
            string danhMuc = cboDanhMuc.SelectedItem is BookCategory category ? category.TenDanhMuc : "";

            var query = SampleData.Books.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(maSach))
                query = query.Where(b => b.MaSach.Contains(maSach, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(tenSach))
                query = query.Where(b => b.TenSach.Contains(tenSach, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(tacGia))
                query = query.Where(b => b.TacGia.Contains(tacGia, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(chuDe))
                query = query.Where(b => b.ChuDe.Contains(chuDe, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(danhMuc))
                query = query.Where(b => string.Equals(LibraryDataService.GetCategoryName(b.MaDanhMuc, b.TheLoai), danhMuc, StringComparison.OrdinalIgnoreCase));

            dgvBooks.Rows.Clear();
            foreach (var b in query.OrderBy(b => b.TenSach))
            {
                LibraryDataService.SyncBookCategory(b);
                LibraryDataService.SyncBookStatus(b);
                dgvBooks.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.TheLoai, b.ChuDe, b.NamXuatBan, b.SoLuong, b.SoLuongDangMuon, b.SoLuongHienCo);
            }
        }

        private void DgvBooks_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maSach = dgvBooks.Rows[e.RowIndex].Cells["MaSach"].Value?.ToString() ?? "";
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return;

            txtMaSach.Text = book.MaSach;
            txtTenSach.Text = book.TenSach;
            txtTacGia.Text = book.TacGia;
            txtChuDe.Text = book.ChuDe;
            txtNamXB.Text = book.NamXuatBan.ToString();
            txtNXB.Text = book.NhaXuatBan;
            txtISBN.Text = book.ISBN;
            txtURI.Text = book.URI;
            txtSoLuong.Text = book.SoLuong.ToString();
            cboDanhMuc.SelectedValue = book.MaDanhMuc;
        }

        private void SaveBook(bool isEditMode)
        {
            int namXb = int.TryParse(txtNamXB.Text, out int nam) ? nam : DateTime.Now.Year;
            int soLuong = int.TryParse(txtSoLuong.Text, out int qty) ? qty : 0;

            var book = new Book
            {
                MaSach = txtMaSach.Text.Trim(),
                TenSach = txtTenSach.Text.Trim(),
                TacGia = txtTacGia.Text.Trim(),
                MaDanhMuc = cboDanhMuc.SelectedValue?.ToString() ?? "",
                ChuDe = txtChuDe.Text.Trim(),
                NamXuatBan = namXb,
                NhaXuatBan = txtNXB.Text.Trim(),
                ISBN = txtISBN.Text.Trim(),
                URI = txtURI.Text.Trim(),
                SoLuong = soLuong
            };

            var result = LibraryDataService.SaveBook(book, isEditMode);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!result.Success) return;

            ReloadCategoryOptions();
            ClearForm();
            LoadBooks();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            string maSach = txtMaSach.Text.Trim();
            if (string.IsNullOrWhiteSpace(maSach))
            {
                MessageBox.Show("Vui lòng chọn đầu sách cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa đầu sách này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var result = LibraryDataService.DeleteBook(maSach);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!result.Success) return;

            ClearForm();
            LoadBooks();
        }

        private void ClearForm()
        {
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtTacGia.Clear();
            txtChuDe.Clear();
            txtNamXB.Clear();
            txtNXB.Clear();
            txtISBN.Clear();
            txtURI.Clear();
            txtSoLuong.Clear();
            cboDanhMuc.SelectedIndex = -1;
        }
    }
}
