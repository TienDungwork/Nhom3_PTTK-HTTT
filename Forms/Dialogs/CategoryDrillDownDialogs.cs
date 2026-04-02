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
                Text = "Nhấp đúp một đầu sách để xem danh sách quyển sách",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(18, 46),
                Size = new Size(560, 24),
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
            dgvTitles.CellDoubleClick += DgvTitles_CellDoubleClick;
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

        private void DgvTitles_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maSach = dgvTitles.Rows[e.RowIndex].Cells["MaSach"].Value?.ToString() ?? "";
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
                Text = "Danh sách quyển sách thuộc đầu sách đã chọn",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(18, 46),
                Size = new Size(560, 24),
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
    }
}
