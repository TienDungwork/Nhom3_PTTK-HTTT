using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Dialogs
{
    public class BookTitleDrillDownDialog : Form
    {
        private readonly string maDanhMuc;
        private readonly string tenDanhMuc;
        private readonly DataGridView dgvTitles = new DataGridView();

        public BookTitleDrillDownDialog(string maDanhMuc, string tenDanhMuc)
        {
            this.maDanhMuc = maDanhMuc;
            this.tenDanhMuc = tenDanhMuc;

            Text = $"Đầu sách - {tenDanhMuc}";
            Size = new Size(1100, 680);
            MinimumSize = new Size(860, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = ThemeColors.Background;

            InitializeUi();
            LoadTitles();
        }

        private void InitializeUi()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = ThemeColors.Background };
            header.Controls.Add(new Label
            {
                Text = $"DANH MỤC: {tenDanhMuc} ({maDanhMuc})",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(18, 14),
                Size = new Size(900, 34),
                BackColor = Color.Transparent
            });
            header.Controls.Add(new Label
            {
                Text = "Nhấp một đầu sách để mở danh sách quyển sách (hoặc nhấn Enter khi đang chọn dòng)",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(18, 46),
                Size = new Size(720, 24),
                BackColor = Color.Transparent
            });
            Controls.Add(header);

            dgvTitles.Dock = DockStyle.Fill;
            dgvTitles.Columns.Add("MaSach", "Mã đầu sách");
            dgvTitles.Columns.Add("TenSach", "Tên sách");
            dgvTitles.Columns.Add("TacGia", "Tác giả");
            dgvTitles.Columns.Add("ChuDe", "Chủ đề");
            dgvTitles.Columns.Add("NamXB", "Năm XB");
            dgvTitles.Columns.Add("TongSo", "Tổng số lượng");
            dgvTitles.Columns.Add("DangMuon", "Đang mượn");
            dgvTitles.Columns.Add("ConLai", "Hiện có");
            ModernDataGridView.ApplyStyle(dgvTitles);
            dgvTitles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTitles.MultiSelect = false;
            dgvTitles.CellClick += DgvTitles_CellClick;
            dgvTitles.KeyDown += DgvTitles_KeyDown;
            dgvTitles.ScrollBars = ScrollBars.Both;
            dgvTitles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTitles.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            Controls.Add(dgvTitles);
        }

        private void LoadTitles()
        {
            dgvTitles.Rows.Clear();
            var books = SampleData.Books
                .Where(b => b.MaDanhMuc == maDanhMuc)
                .OrderBy(b => b.TenSach)
                .ToList();

            foreach (var b in books)
            {
                LibraryDataService.SyncBookStatus(b);
                dgvTitles.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.ChuDe, b.NamXuatBan, b.SoLuong, b.SoLuongDangMuon, b.SoLuongHienCo);
            }
        }

        private void DgvTitles_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (dgvTitles.CurrentRow == null || dgvTitles.CurrentRow.Index < 0) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            OpenCopiesForRow(dgvTitles.CurrentRow.Index);
        }

        private void DgvTitles_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenCopiesForRow(e.RowIndex);
        }

        private void OpenCopiesForRow(int rowIndex)
        {
            string maSach = dgvTitles.Rows[rowIndex].Cells["MaSach"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(maSach)) return;

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return;

            using var dlg = new BookCopyDrillDownDialog(book.MaSach, book.TenSach);
            dlg.ShowDialog(this);
        }
    }

    public class BookCopyDrillDownDialog : Form
    {
        private readonly string maSach;
        private readonly string tenSach;
        private readonly DataGridView dgvCopies = new DataGridView();

        public BookCopyDrillDownDialog(string maSach, string tenSach)
        {
            this.maSach = maSach;
            this.tenSach = tenSach;

            Text = $"Quyển sách - {tenSach}";
            Size = new Size(980, 620);
            MinimumSize = new Size(760, 480);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = ThemeColors.Background;

            InitializeUi();
            LoadCopies();
        }

        private void InitializeUi()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = ThemeColors.Background };
            header.Controls.Add(new Label
            {
                Text = $"ĐẦU SÁCH: {tenSach} ({maSach})",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(18, 14),
                Size = new Size(780, 34),
                BackColor = Color.Transparent
            });
            header.Controls.Add(new Label
            {
                Text = "Danh sách quyển — nhấp một quyển để xem chi tiết (hoặc Enter khi đang chọn dòng)",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(18, 46),
                Size = new Size(720, 24),
                BackColor = Color.Transparent
            });
            Controls.Add(header);

            dgvCopies.Dock = DockStyle.Fill;
            dgvCopies.Columns.Add("MaQuyenSach", "Mã quyển");
            dgvCopies.Columns.Add("NgayNhap", "Ngày nhập");
            dgvCopies.Columns.Add("TrangThai", "Trạng thái");
            dgvCopies.Columns.Add("NhaCungCap", "Nhà cung cấp");
            dgvCopies.Columns.Add("GhiChu", "Ghi chú");
            ModernDataGridView.ApplyStyle(dgvCopies);
            dgvCopies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCopies.MultiSelect = false;
            dgvCopies.CellClick += DgvCopies_CellClick;
            dgvCopies.KeyDown += DgvCopies_KeyDown;
            dgvCopies.ScrollBars = ScrollBars.Both;
            dgvCopies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCopies.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            Controls.Add(dgvCopies);
        }

        private void LoadCopies()
        {
            dgvCopies.Rows.Clear();
            var copies = SampleData.BookCopies
                .Where(c => c.MaSach == maSach)
                .OrderByDescending(c => c.NgayNhap)
                .ThenBy(c => c.MaQuyenSach)
                .ToList();

            foreach (var c in copies)
            {
                dgvCopies.Rows.Add(c.MaQuyenSach, c.NgayNhap.ToString("dd/MM/yyyy"), c.TrangThai, c.NhaCungCap, c.GhiChu);
            }
        }

        private void DgvCopies_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (dgvCopies.CurrentRow == null || dgvCopies.CurrentRow.Index < 0) return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            OpenCopyDetailForRow(dgvCopies.CurrentRow.Index);
        }

        private void DgvCopies_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenCopyDetailForRow(e.RowIndex);
        }

        private void OpenCopyDetailForRow(int rowIndex)
        {
            string maQuyen = dgvCopies.Rows[rowIndex].Cells["MaQuyenSach"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(maQuyen)) return;

            var copy = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == maQuyen && c.MaSach == maSach);
            if (copy == null) return;

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            using var dlg = new BookCopyDetailDialog(copy, book, tenSach);
            dlg.ShowDialog(this);
        }
    }

    /// <summary>Chi tiết một quyển sách (và thông tin đầu sách liên quan).</summary>
    public class BookCopyDetailDialog : Form
    {
        public BookCopyDetailDialog(BookCopy copy, Book? book, string tenDauSachHienThi)
        {
            Text = $"Chi tiết quyển — {copy.MaQuyenSach}";
            Size = new Size(560, 460);
            MinimumSize = new Size(480, 400);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = ThemeColors.Background;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var scroll = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 12), AutoScroll = true };

            int y = 0;
            void AddRow(string caption, string value)
            {
                scroll.Controls.Add(new Label
                {
                    Text = caption,
                    Font = ThemeColors.SmallFont,
                    ForeColor = ThemeColors.TextSecondary,
                    Location = new Point(0, y),
                    Size = new Size(160, 20),
                    BackColor = Color.Transparent
                });
                scroll.Controls.Add(new Label
                {
                    Text = string.IsNullOrWhiteSpace(value) ? "—" : value,
                    Font = ThemeColors.BodyFont,
                    ForeColor = ThemeColors.TextPrimary,
                    Location = new Point(168, y),
                    Size = new Size(340, 60),
                    BackColor = Color.Transparent
                });
                y += 52;
            }

            AddRow("Tên đầu sách", book?.TenSach ?? tenDauSachHienThi);
            AddRow("Tác giả", book?.TacGia ?? "");
            AddRow("Chủ đề / Năm XB", book != null ? $"{book.ChuDe} — {book.NamXuatBan}" : "");
            AddRow("Mã đầu sách", copy.MaSach);
            AddRow("Mã quyển", copy.MaQuyenSach);
            AddRow("Ngày nhập", copy.NgayNhap.ToString("dd/MM/yyyy"));
            AddRow("Trạng thái quyển", copy.TrangThai);
            AddRow("Nhà cung cấp", copy.NhaCungCap);
            AddRow("Ghi chú", copy.GhiChu);

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(24, 8, 24, 12),
                BackColor = ThemeColors.Background,
                WrapContents = false
            };
            var btnOk = new Button { Text = "Đóng", Size = new Size(120, 36), Font = ThemeColors.ButtonFont, DialogResult = DialogResult.OK };
            footer.Controls.Add(btnOk);

            Controls.Add(scroll);
            Controls.Add(footer);
            AcceptButton = btnOk;
            CancelButton = btnOk;
        }
    }
}
