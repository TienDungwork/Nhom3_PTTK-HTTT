using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Forms.Dialogs;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class CatalogPanel : UserControl
    {
        private TextBox txtMaDanhMuc = null!, txtTenDanhMuc = null!, txtMoTa = null!, txtViTriKe = null!;
        private CheckBox chkDangSuDung = null!;
        private DataGridView dgvCategories = null!;

        public CatalogPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 118, BackColor = ThemeColors.Background };
            headerPanel.Controls.Add(new Label { Text = "QUẢN LÝ KHO SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 8), Size = new Size(520, 36), BackColor = Color.Transparent });
            headerPanel.Controls.Add(new Label { Text = "Danh mục → đầu sách → quyển: nhấn đúp dòng trên từng lưới để mở bước tiếp. Một lần nhấp chọn dòng để sửa form bên dưới.", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 44), Size = new Size(860, 36), BackColor = Color.Transparent });

            var btnFullTitles = new RoundedButton { Text = "Đầu sách (màn đầy đủ)", Size = new Size(200, 36), Location = new Point(32, 78), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnFullTitles.Click += (_, _) => ShowEmbeddedPanelDialog("Quản lý đầu sách", new BookTitlePanel());
            headerPanel.Controls.Add(btnFullTitles);

            var btnFullCopies = new RoundedButton { Text = "Quyển sách (màn đầy đủ)", Size = new Size(210, 36), Location = new Point(244, 78), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnFullCopies.Click += (_, _) => ShowEmbeddedPanelDialog("Quản lý sách (quyển)", new BookCopyManagePanel());
            headerPanel.Controls.Add(btnFullCopies);

            var inputCard = new Panel { BackColor = Color.White };
            inputCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, inputCard.Width - 2, inputCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            AddInputField(inputCard, "Mã danh mục", 16, 12, 150, out txtMaDanhMuc);
            AddInputField(inputCard, "Tên danh mục", 182, 12, 220, out txtTenDanhMuc);
            AddInputField(inputCard, "Vị trí kệ", 418, 12, 120, out txtViTriKe);
            AddInputField(inputCard, "Mô tả", 554, 12, 390, out txtMoTa);

            chkDangSuDung = new CheckBox
            {
                Text = "Đang sử dụng",
                Location = new Point(20, 86),
                Size = new Size(140, 24),
                Font = ThemeColors.BodyFont,
                Checked = true
            };
            inputCard.Controls.Add(chkDangSuDung);

            var btnThem = new RoundedButton { Text = "Thêm", Size = new Size(100, 40), Location = new Point(16, 128), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnThem.Click += (_, _) => SaveCategory(false);
            inputCard.Controls.Add(btnThem);

            var btnSua = new RoundedButton { Text = "Cập nhật", Size = new Size(110, 40), Location = new Point(126, 128), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnSua.Click += (_, _) => SaveCategory(true);
            inputCard.Controls.Add(btnSua);

            var btnXoa = new RoundedButton { Text = "Xóa", Size = new Size(100, 40), Location = new Point(246, 128), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnXoa.Click += BtnXoa_Click;
            inputCard.Controls.Add(btnXoa);

            var btnClear = new RoundedButton { Text = "Xóa form", Size = new Size(100, 40), Location = new Point(366, 128), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (_, _) => ClearForm();
            inputCard.Controls.Add(btnClear);

            var btnTim = new RoundedButton { Text = "Tìm kiếm", Size = new Size(110, 40), Location = new Point(486, 128), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnTim.Click += (_, _) => SearchCategoriesByInputs();
            inputCard.Controls.Add(btnTim);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 228, Padding = new Padding(32, 0, 32, 16), BackColor = ThemeColors.Background };
            inputCard.Dock = DockStyle.Fill;
            bottomPanel.Controls.Add(inputCard);

            var gridHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 8, 32, 8), BackColor = ThemeColors.Background };

            var gridToolbar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = ThemeColors.Background };
            gridToolbar.Controls.Add(new Label
            {
                Text = "Danh mục sách — nhấn đúp một dòng để xem đầu sách",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(0, 8),
                AutoSize = true,
                BackColor = Color.Transparent
            });

            dgvCategories = new DataGridView { Dock = DockStyle.Fill };
            dgvCategories.Columns.Add("MaDanhMuc", "Mã danh mục");
            dgvCategories.Columns.Add("TenDanhMuc", "Tên danh mục");
            dgvCategories.Columns.Add("MoTa", "Mô tả");
            dgvCategories.Columns.Add("ViTriKe", "Vị trí kệ");
            dgvCategories.Columns.Add("SoDauSach", "Số đầu sách");
            dgvCategories.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvCategories);
            dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
            dgvCategories.CellClick += DgvCategories_CellClick;
            dgvCategories.CellDoubleClick += DgvCategories_CellDoubleClick;

            gridHost.Controls.Add(gridToolbar);
            gridHost.Controls.Add(dgvCategories);

            Controls.Add(bottomPanel);
            Controls.Add(headerPanel);
            Controls.Add(gridHost);

            LoadCategories();
        }

        private void OpenCategoryDrillDown(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvCategories.Rows.Count) return;
            string maDanhMuc = dgvCategories.Rows[rowIndex].Cells["MaDanhMuc"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(maDanhMuc)) return;
            var category = SampleData.BookCategories.FirstOrDefault(c => c.MaDanhMuc == maDanhMuc);
            if (category == null) return;

            using var dlg = new BookTitleDrillDownDialog(category.MaDanhMuc, category.TenDanhMuc);
            dlg.ShowDialog(FindForm());
        }

        private void AddInputField(Panel parent, string label, int x, int y, int width, out TextBox textBox)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, y), Size = new Size(width, 16), BackColor = Color.Transparent });
            textBox = new TextBox { Location = new Point(x, y + 18), Size = new Size(width, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(textBox);
        }

        private void LoadCategories(string keyword = "")
        {
            dgvCategories.Rows.Clear();
            var categories = LibraryDataService.GetCategories();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                categories = categories
                    .Where(c =>
                        c.MaDanhMuc.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        c.TenDanhMuc.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        c.MoTa.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            foreach (var category in categories)
            {
                dgvCategories.Rows.Add(
                    category.MaDanhMuc,
                    category.TenDanhMuc,
                    category.MoTa,
                    category.ViTriKe,
                    LibraryDataService.CountBooksByCategory(category.MaDanhMuc),
                    category.DangSuDung ? "Đang sử dụng" : "Ngừng sử dụng");
            }
        }

        private void SearchCategoriesByInputs()
        {
            var maDanhMuc = txtMaDanhMuc.Text.Trim();
            var tenDanhMuc = txtTenDanhMuc.Text.Trim();
            var moTa = txtMoTa.Text.Trim();
            var viTriKe = txtViTriKe.Text.Trim();

            dgvCategories.Rows.Clear();
            var categories = LibraryDataService.GetCategories()
                .Where(c =>
                    (string.IsNullOrWhiteSpace(maDanhMuc) || c.MaDanhMuc.Contains(maDanhMuc, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(tenDanhMuc) || c.TenDanhMuc.Contains(tenDanhMuc, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(moTa) || c.MoTa.Contains(moTa, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(viTriKe) || c.ViTriKe.Contains(viTriKe, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var category in categories)
            {
                dgvCategories.Rows.Add(
                    category.MaDanhMuc,
                    category.TenDanhMuc,
                    category.MoTa,
                    category.ViTriKe,
                    LibraryDataService.CountBooksByCategory(category.MaDanhMuc),
                    category.DangSuDung ? "Đang sử dụng" : "Ngừng sử dụng");
            }
        }

        private void DgvCategories_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maDanhMuc = dgvCategories.Rows[e.RowIndex].Cells["MaDanhMuc"].Value?.ToString() ?? "";
            var category = SampleData.BookCategories.FirstOrDefault(c => c.MaDanhMuc == maDanhMuc);
            if (category == null) return;

            txtMaDanhMuc.Text = category.MaDanhMuc;
            txtTenDanhMuc.Text = category.TenDanhMuc;
            txtMoTa.Text = category.MoTa;
            txtViTriKe.Text = category.ViTriKe;
            chkDangSuDung.Checked = category.DangSuDung;
        }

        private void DgvCategories_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OpenCategoryDrillDown(e.RowIndex);
        }

        private void SaveCategory(bool isEditMode)
        {
            var result = LibraryDataService.SaveCategory(new BookCategory
            {
                MaDanhMuc = txtMaDanhMuc.Text.Trim(),
                TenDanhMuc = txtTenDanhMuc.Text.Trim(),
                MoTa = txtMoTa.Text.Trim(),
                ViTriKe = txtViTriKe.Text.Trim(),
                DangSuDung = chkDangSuDung.Checked
            }, isEditMode);

            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!result.Success) return;

            ClearForm();
            LoadCategories();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            string maDanhMuc = txtMaDanhMuc.Text.Trim();
            if (string.IsNullOrWhiteSpace(maDanhMuc))
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa danh mục này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var result = LibraryDataService.DeleteCategory(maDanhMuc);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (!result.Success) return;

            ClearForm();
            LoadCategories();
        }

        private void ClearForm()
        {
            txtMaDanhMuc.Clear();
            txtTenDanhMuc.Clear();
            txtMoTa.Clear();
            txtViTriKe.Clear();
            chkDangSuDung.Checked = true;
        }

        private void ShowEmbeddedPanelDialog(string title, UserControl content)
        {
            var owner = FindForm();
            using var f = new Form
            {
                Text = title,
                Size = new Size(1100, 720),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = ThemeColors.Background,
                MinimumSize = new Size(900, 560)
            };
            if (owner != null) f.Icon = owner.Icon;
            content.Dock = DockStyle.Fill;
            f.Controls.Add(content);
            f.ShowDialog(owner);
        }
    }
}
