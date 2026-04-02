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
    public class BookCopyManagePanel : UserControl
    {
        private const int HorizontalMargin = 32;
        private const int InputTop = 92;
        private const int InputHeight = 220;
        private const int GridTop = 324;
        private const int BaseContentWidth = 1200;
        private const int BaseContentHeight = 700;
        private TextBox txtMaQuyen = null!, txtDanhMuc = null!, txtNhaCungCap = null!, txtGhiChu = null!;
        private ComboBox cboSach = null!, cboTrangThai = null!;
        private DateTimePicker dtpNgayNhap = null!;
        private DataGridView dgvCopies = null!;
        private Panel card = null!;

        public BookCopyManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            Controls.Add(new Label
            {
                Text = "QUẢN LÝ SÁCH",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(520, 40),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = "Quản lý từng quyển sách theo mã riêng, đầu sách, ngày nhập và trạng thái",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(860, 22),
                BackColor = Color.Transparent
            });

            card = new Panel
            {
                Location = new Point(HorizontalMargin, InputTop),
                Size = new Size(960, 220),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            card.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, card.Width - 2, card.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            AddInput(card, "Mã quyển sách", 16, 12, 150, out txtMaQuyen);
            AddBookCombo(card, "Đầu sách", 182, 12, 270, out cboSach);
            AddInput(card, "Danh mục", 468, 12, 170, out txtDanhMuc);
            txtDanhMuc.ReadOnly = true;
            txtDanhMuc.BackColor = Color.FromArgb(248, 250, 252);
            AddDateInput(card, "Ngày nhập", 654, 12, 130, out dtpNgayNhap);
            dtpNgayNhap.ShowCheckBox = true;
            dtpNgayNhap.Checked = false;
            AddComboInput(card, "Trạng thái", 800, 12, 120, out cboTrangThai, new[] { "Có sẵn", "Đang mượn", "Hỏng", "Mất", "Bảo trì" });
            cboTrangThai.Items.Insert(0, "Tất cả");
            cboTrangThai.SelectedIndex = 0;

            AddInput(card, "Nhà cung cấp", 16, 74, 220, out txtNhaCungCap);
            AddInput(card, "Ghi chú", 252, 74, 754, out txtGhiChu);

            var btnThem = new RoundedButton { Text = "Thêm sách", Size = new Size(110, 40), Location = new Point(16, 148), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnThem.Click += (_, _) => SaveCopy(false);
            card.Controls.Add(btnThem);

            var btnSua = new RoundedButton { Text = "Cập nhật", Size = new Size(110, 40), Location = new Point(136, 148), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnSua.Click += (_, _) => SaveCopy(true);
            card.Controls.Add(btnSua);

            var btnXoa = new RoundedButton { Text = "Xóa sách", Size = new Size(110, 40), Location = new Point(256, 148), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnXoa.Click += BtnXoa_Click;
            card.Controls.Add(btnXoa);

            var btnClear = new RoundedButton { Text = "Xóa form", Size = new Size(100, 40), Location = new Point(376, 148), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (_, _) => ClearForm();
            card.Controls.Add(btnClear);

            var btnTim = new RoundedButton { Text = "Tìm kiếm", Size = new Size(110, 40), Location = new Point(500, 148), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnTim.Click += (_, _) => SearchCopiesByInputs();
            card.Controls.Add(btnTim);

            Controls.Add(card);

            dgvCopies = new DataGridView
            {
                Location = new Point(HorizontalMargin, GridTop),
                Size = new Size(960, 320),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvCopies.Columns.Add("MaQuyenSach", "Mã quyển");
            dgvCopies.Columns.Add("MaSach", "Mã đầu sách");
            dgvCopies.Columns.Add("TenSach", "Tên đầu sách");
            dgvCopies.Columns.Add("DanhMuc", "Danh mục");
            dgvCopies.Columns.Add("NgayNhap", "Ngày nhập");
            dgvCopies.Columns.Add("TrangThai", "Trạng thái");
            dgvCopies.Columns.Add("NCC", "Nhà cung cấp");
            dgvCopies.Columns.Add("GhiChu", "Ghi chú");
            ModernDataGridView.ApplyStyle(dgvCopies);
            dgvCopies.ScrollBars = ScrollBars.Both;
            dgvCopies.CellClick += DgvCopies_CellClick;
            Controls.Add(dgvCopies);

            ReloadBooks();
            ClearForm();
            LoadCopies();
            cboSach.SelectedIndexChanged += (_, _) => UpdateSelectedCategory();
            ApplyPanelLayout();
            ApplyCopyGridLayout();
            Resize += (_, _) =>
            {
                ApplyPanelLayout();
                ApplyCopyGridLayout();
            };
            Load += (_, _) =>
            {
                ApplyPanelLayout();
                ApplyCopyGridLayout();
            };
        }

        /// <summary>Chọn đầu sách và làm mới lưới quyển (dùng khi mở từ màn đầu sách).</summary>
        public void SelectHeadBook(string maSach)
        {
            if (string.IsNullOrWhiteSpace(maSach)) return;
            ReloadBooks();
            try
            {
                if (SampleData.Books.Any(b => b.MaSach == maSach))
                    cboSach.SelectedValue = maSach;
            }
            catch
            {
                /* ignore */
            }

            UpdateSelectedCategory();
            SearchCopiesByInputs();
        }

        private void ApplyPanelLayout()
        {
            int panelWidth = Math.Max(BaseContentWidth, ClientSize.Width - HorizontalMargin * 2);
            int gridHeight = Math.Max(260, ClientSize.Height - (GridTop + 28));
            card.SetBounds(HorizontalMargin, InputTop, panelWidth, InputHeight);
            dgvCopies.SetBounds(HorizontalMargin, GridTop, panelWidth, gridHeight);
            AutoScrollMinSize = new Size(panelWidth + HorizontalMargin, BaseContentHeight);
        }

        private void ApplyCopyGridLayout()
        {
            if (dgvCopies.Columns.Count == 0) return;

            dgvCopies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvCopies.Columns["MaQuyenSach"].Width = 90;
            dgvCopies.Columns["MaSach"].Width = 100;
            dgvCopies.Columns["TenSach"].Width = 220;
            dgvCopies.Columns["DanhMuc"].Width = 130;
            dgvCopies.Columns["NgayNhap"].Width = 110;
            dgvCopies.Columns["TrangThai"].Width = 120;
            dgvCopies.Columns["NCC"].Width = 170;
            dgvCopies.Columns["GhiChu"].Width = 220;
        }

        private void AddInput(Panel parent, string label, int x, int y, int width, out TextBox box)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            box = new TextBox { Location = new Point(x, y + 18), Size = new Size(width, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(box);
        }

        private void AddBookCombo(Panel parent, string label, int x, int y, int width, out ComboBox combo)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            combo = new ComboBox
            {
                Location = new Point(x, y + 18),
                Size = new Size(width, 28),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            parent.Controls.Add(combo);
        }

        private void AddDateInput(Panel parent, string label, int x, int y, int width, out DateTimePicker picker)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            picker = new DateTimePicker
            {
                Location = new Point(x, y + 18),
                Size = new Size(width, 28),
                Font = ThemeColors.BodyFont,
                Format = DateTimePickerFormat.Short
            };
            parent.Controls.Add(picker);
        }

        private void AddComboInput(Panel parent, string label, int x, int y, int width, out ComboBox combo, string[] options)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            combo = new ComboBox
            {
                Location = new Point(x, y + 18),
                Size = new Size(width, 28),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var option in options) combo.Items.Add(option);
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
            parent.Controls.Add(combo);
        }

        private void ReloadBooks()
        {
            string selected = cboSach.SelectedValue?.ToString() ?? "";
            var data = SampleData.Books.OrderBy(b => b.TenSach).ToList();
            cboSach.DataSource = data;
            cboSach.DisplayMember = "TenSach";
            cboSach.ValueMember = "MaSach";
            if (!string.IsNullOrWhiteSpace(selected))
                cboSach.SelectedValue = selected;
            UpdateSelectedCategory();
        }

        private void UpdateSelectedCategory()
        {
            string maSach = cboSach.SelectedValue?.ToString() ?? "";
            txtDanhMuc.Text = string.IsNullOrWhiteSpace(maSach) ? "" : LibraryDataService.GetBookCategoryName(maSach);
        }

        private void LoadCopies(string keyword = "")
        {
            dgvCopies.Rows.Clear();
            var copies = LibraryDataService.SearchCopies(keyword);
            foreach (var copy in copies)
            {
                dgvCopies.Rows.Add(
                    copy.MaQuyenSach,
                    copy.MaSach,
                    LibraryDataService.GetBookName(copy.MaSach),
                    LibraryDataService.GetBookCategoryName(copy.MaSach),
                    copy.NgayNhap.ToString("dd/MM/yyyy"),
                    copy.TrangThai,
                    copy.NhaCungCap,
                    copy.GhiChu);
            }
        }

        private void SearchCopiesByInputs()
        {
            string maQuyen = txtMaQuyen.Text.Trim();
            string maSach = cboSach.SelectedValue?.ToString() ?? "";
            string danhMuc = txtDanhMuc.Text.Trim();
            string nhaCungCap = txtNhaCungCap.Text.Trim();
            string ghiChu = txtGhiChu.Text.Trim();
            string trangThai = cboTrangThai.SelectedItem?.ToString() ?? "Tất cả";
            bool locNgayNhap = dtpNgayNhap.Checked;
            var ngayNhap = dtpNgayNhap.Value.Date;

            var copies = SampleData.BookCopies
                .Where(c =>
                    (string.IsNullOrWhiteSpace(maQuyen) || c.MaQuyenSach.Contains(maQuyen, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(maSach) || c.MaSach == maSach) &&
                    (string.IsNullOrWhiteSpace(danhMuc) || LibraryDataService.GetBookCategoryName(c.MaSach).Contains(danhMuc, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(nhaCungCap) || c.NhaCungCap.Contains(nhaCungCap, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(ghiChu) || c.GhiChu.Contains(ghiChu, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(trangThai) || trangThai == "Tất cả" || string.Equals(c.TrangThai, trangThai, StringComparison.OrdinalIgnoreCase)) &&
                    (!locNgayNhap || c.NgayNhap.Date == ngayNhap))
                .OrderByDescending(c => c.NgayNhap)
                .ThenBy(c => c.MaQuyenSach)
                .ToList();

            dgvCopies.Rows.Clear();
            foreach (var copy in copies)
            {
                dgvCopies.Rows.Add(
                    copy.MaQuyenSach,
                    copy.MaSach,
                    LibraryDataService.GetBookName(copy.MaSach),
                    LibraryDataService.GetBookCategoryName(copy.MaSach),
                    copy.NgayNhap.ToString("dd/MM/yyyy"),
                    copy.TrangThai,
                    copy.NhaCungCap,
                    copy.GhiChu);
            }
        }

        private void DgvCopies_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maQuyen = dgvCopies.Rows[e.RowIndex].Cells["MaQuyenSach"].Value?.ToString() ?? "";
            var copy = SampleData.BookCopies.FirstOrDefault(c => c.MaQuyenSach == maQuyen);
            if (copy == null) return;

            txtMaQuyen.Text = copy.MaQuyenSach;
            cboSach.SelectedValue = copy.MaSach;
            txtDanhMuc.Text = LibraryDataService.GetBookCategoryName(copy.MaSach);
            dtpNgayNhap.Value = copy.NgayNhap;
            cboTrangThai.SelectedItem = copy.TrangThai;
            txtNhaCungCap.Text = copy.NhaCungCap;
            txtGhiChu.Text = copy.GhiChu;
        }

        private void SaveCopy(bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(txtMaQuyen.Text))
            {
                MessageBox.Show("Vui lòng nhập mã quyển sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var copy = new BookCopy
            {
                MaQuyenSach = txtMaQuyen.Text.Trim(),
                MaSach = cboSach.SelectedValue?.ToString() ?? "",
                NgayNhap = dtpNgayNhap.Value.Date,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Có sẵn",
                NhaCungCap = txtNhaCungCap.Text.Trim(),
                GhiChu = txtGhiChu.Text.Trim()
            };

            var result = LibraryDataService.SaveCopy(copy, isEditMode);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (!result.Success) return;

            LoadCopies();
            ClearForm();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            string maQuyen = txtMaQuyen.Text.Trim();
            if (string.IsNullOrWhiteSpace(maQuyen))
            {
                MessageBox.Show("Vui lòng chọn quyển sách cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa quyển {maQuyen}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var result = LibraryDataService.DeleteCopy(maQuyen);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (!result.Success) return;

            LoadCopies();
            ClearForm();
        }

        private void ClearForm()
        {
            txtMaQuyen.Text = LibraryDataService.GenerateCopyCode();
            txtNhaCungCap.Clear();
            txtGhiChu.Clear();
            dtpNgayNhap.Value = DateTime.Today;
            dtpNgayNhap.Checked = false;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
            cboSach.SelectedIndex = -1;
        }
    }
}
