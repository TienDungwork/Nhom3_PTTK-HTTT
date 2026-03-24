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
    public class BorrowReturnPanel : UserControl
    {
        private DataGridView dgvBooks = null!;
        private DateTimePicker dtpNgayMuon = null!;
        private TextBox txtSoNgay = null!;
        private Label lblNgayTra = null!;
        private TextBox txtSearch = null!;
        private ComboBox cboQuyenSach = null!;

        public BorrowReturnPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = ThemeColors.Background };
            topPanel.Controls.Add(new Label { Text = "LẬP PHIẾU MƯỢN / TRẢ SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 16), Size = new Size(520, 36), BackColor = Color.Transparent });
            topPanel.Controls.Add(new Label { Text = "Độc giả tạo phiếu mượn chờ duyệt; thao tác trả sẽ được cập nhật ngay khi đủ điều kiện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 50), Size = new Size(760, 22), BackColor = Color.Transparent });

            topPanel.Controls.Add(new Label { Text = "Tìm kiếm sách", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(560, 20), Size = new Size(120, 18), BackColor = Color.Transparent });
            txtSearch = new TextBox { Location = new Point(560, 40), Size = new Size(320, 30), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            txtSearch.TextChanged += (_, _) => LoadBooks(txtSearch.Text.Trim());
            topPanel.Controls.Add(txtSearch);

            var bottomBar = new Panel { Dock = DockStyle.Bottom, Height = 140, BackColor = Color.White, Padding = new Padding(20, 12, 20, 12) };
            bottomBar.Paint += (s, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawLine(pen, 0, 0, bottomBar.Width, 0);
            };

            bottomBar.Controls.Add(new Label { Text = "Ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 8), Size = new Size(120, 18), BackColor = Color.Transparent });
            dtpNgayMuon = new DateTimePicker { Location = new Point(20, 28), Size = new Size(170, 32), Font = ThemeColors.BodyFont, Format = DateTimePickerFormat.Short };
            dtpNgayMuon.ValueChanged += UpdateNgayTra;
            bottomBar.Controls.Add(dtpNgayMuon);

            bottomBar.Controls.Add(new Label { Text = "Số ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(210, 8), Size = new Size(120, 18), BackColor = Color.Transparent });
            txtSoNgay = new TextBox { Text = "14", Location = new Point(210, 28), Size = new Size(80, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
            txtSoNgay.TextChanged += UpdateNgayTra;
            bottomBar.Controls.Add(txtSoNgay);

            bottomBar.Controls.Add(new Label { Text = "Ngày trả (dự kiến)", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(310, 8), Size = new Size(150, 18), BackColor = Color.Transparent });
            lblNgayTra = new Label
            {
                Text = DateTime.Now.AddDays(14).ToString("dd/MM/yyyy"),
                Font = ThemeColors.SubTitleFont,
                ForeColor = ThemeColors.Primary,
                Location = new Point(310, 28),
                Size = new Size(150, 30),
                BackColor = ThemeColors.InfoLight,
                TextAlign = ContentAlignment.MiddleCenter
            };
            bottomBar.Controls.Add(lblNgayTra);

            bottomBar.Controls.Add(new Label { Text = "Quyển sách", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(480, 8), Size = new Size(120, 18), BackColor = Color.Transparent });
            cboQuyenSach = new ComboBox
            {
                Location = new Point(480, 28),
                Size = new Size(260, 32),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            bottomBar.Controls.Add(cboQuyenSach);

            var btnMuon = new RoundedButton { Text = "Lập phiếu mượn", Size = new Size(180, 48), Location = new Point(20, 76), ButtonColor = ThemeColors.Success, Font = new Font("Segoe UI Semibold", 12) };
            btnMuon.Click += BtnMuon_Click;
            bottomBar.Controls.Add(btnMuon);

            var btnTra = new RoundedButton { Text = "Trả sách", Size = new Size(180, 48), Location = new Point(216, 76), ButtonColor = ThemeColors.Warning, Font = new Font("Segoe UI Semibold", 12) };
            btnTra.Click += BtnTra_Click;
            bottomBar.Controls.Add(btnTra);

            var btnTimKiem = new RoundedButton { Text = "Tìm kiếm", Size = new Size(160, 48), Location = new Point(412, 76), ButtonColor = ThemeColors.Primary, Font = new Font("Segoe UI Semibold", 12) };
            btnTimKiem.Click += BtnTimKiem_Click;
            bottomBar.Controls.Add(btnTimKiem);

            var dgvPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 8, 32, 8), BackColor = ThemeColors.Background };
            dgvBooks = new DataGridView { Dock = DockStyle.Fill };
            dgvBooks.Columns.Add("MaSach", "Mã sách");
            dgvBooks.Columns.Add("TenSach", "Nhan đề");
            dgvBooks.Columns.Add("TacGia", "Tác giả");
            dgvBooks.Columns.Add("TheLoai", "Thể loại");
            dgvBooks.Columns.Add("ConLai", "Còn lại");
            dgvBooks.Columns.Add("QuyenGanNhat", "Mã quyển gần nhất");
            dgvBooks.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvBooks);
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.SelectionChanged += (_, _) => LoadCopiesForSelectedBook();
            LoadBooks();
            dgvPanel.Controls.Add(dgvBooks);

            Controls.Add(dgvPanel);
            Controls.Add(bottomBar);
            Controls.Add(topPanel);
        }

        private void LoadBooks(string keyword = "")
        {
            dgvBooks.Rows.Clear();
            var books = string.IsNullOrWhiteSpace(keyword)
                ? LibraryDataService.SearchBooks()
                : LibraryDataService.SearchBooks(keyword: keyword);

            foreach (var b in books)
            {
                LibraryDataService.SyncBookStatus(b);
                var quyenGanNhat = LibraryDataService.GetCopiesForBook(b.MaSach).OrderByDescending(c => c.NgayNhap).FirstOrDefault()?.MaQuyenSach ?? "—";
                dgvBooks.Rows.Add(b.MaSach, b.TenSach, b.TacGia, LibraryDataService.GetCategoryName(b.MaDanhMuc, b.TheLoai), b.SoLuongHienCo, quyenGanNhat, b.TrangThai);
            }
            LoadCopiesForSelectedBook();
        }

        private void LoadCopiesForSelectedBook()
        {
            cboQuyenSach.Items.Clear();
            if (dgvBooks.SelectedRows.Count == 0) return;

            string maSach = dgvBooks.SelectedRows[0].Cells["MaSach"].Value?.ToString() ?? "";
            var copies = LibraryDataService.GetCopiesForBook(maSach, includeUnavailable: false);
            foreach (var copy in copies)
                cboQuyenSach.Items.Add($"{copy.MaQuyenSach} | {copy.TrangThai} | {copy.NgayNhap:dd/MM/yyyy}");

            if (cboQuyenSach.Items.Count > 0) cboQuyenSach.SelectedIndex = 0;
        }

        private string GetSelectedCopyCode()
        {
            if (cboQuyenSach.SelectedItem == null) return "";
            string raw = cboQuyenSach.SelectedItem.ToString() ?? "";
            int idx = raw.IndexOf(" | ", StringComparison.Ordinal);
            return idx > 0 ? raw.Substring(0, idx).Trim() : raw.Trim();
        }

        private void UpdateNgayTra(object? sender, EventArgs e)
        {
            if (int.TryParse(txtSoNgay.Text, out int days) && days > 0)
                lblNgayTra.Text = dtpNgayMuon.Value.AddDays(days).ToString("dd/MM/yyyy");
        }

        private void BtnTimKiem_Click(object? sender, EventArgs e)
        {
            using var searchForm = new Form
            {
                Text = "Tìm kiếm sách",
                Size = new Size(1000, 700),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = ThemeColors.Background
            };
            var searchPanel = new SearchBookPanel { Dock = DockStyle.Fill };
            searchForm.Controls.Add(searchPanel);
            searchForm.ShowDialog(FindForm());
        }

        private void BtnMuon_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            string maSach = row.Cells["MaSach"].Value?.ToString() ?? "";
            int conLai = int.Parse(row.Cells["ConLai"].Value?.ToString() ?? "0");
            if (conLai <= 0)
            {
                MessageBox.Show("Sách này hiện đã hết!", "Không thể mượn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoNgay.Text, out int soNgay) || soNgay <= 0)
            {
                MessageBox.Show("Số ngày mượn không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var cu = UserStore.CurrentUser;
            string maDocGia = GetCurrentReaderCode(cu);
            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                MessageBox.Show("Tài khoản hiện tại chưa được gắn với hồ sơ độc giả.", "Không thể lập phiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maQuyenSach = GetSelectedCopyCode();
            var result = LibraryDataService.CreateBorrowRequest(maSach, maDocGia, cu?.HoTen ?? "", dtpNgayMuon.Value, soNgay, maQuyenSach);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"{result.Message}\nNgày mượn dự kiến: {dtpNgayMuon.Value:dd/MM/yyyy}\nHạn trả dự kiến: {dtpNgayMuon.Value.AddDays(soNgay):dd/MM/yyyy}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBooks(txtSearch.Text.Trim());
        }

        private void BtnTra_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần trả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            string maSach = row.Cells["MaSach"].Value?.ToString() ?? "";
            var cu = UserStore.CurrentUser;
            string maDocGia = GetCurrentReaderCode(cu);
            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                MessageBox.Show("Tài khoản hiện tại chưa được gắn với hồ sơ độc giả.", "Không thể trả", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = LibraryDataService.ReturnBook(maSach, maDocGia, DateTime.Now);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBooks(txtSearch.Text.Trim());
        }

        private static string GetCurrentReaderCode(AppUser? user)
        {
            if (user == null || user.Role != UserRole.DocGia)
                return "";

            if (!string.IsNullOrWhiteSpace(user.MaDocGia))
                return user.MaDocGia;

            var reader = SampleData.Readers.FirstOrDefault(r =>
                string.Equals(r.Email, user.Email, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.HoTen, user.HoTen, StringComparison.OrdinalIgnoreCase));
            return reader?.MaDocGia ?? "";
        }
    }
}
