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
        private TextBox txtMaDanhMuc = null!, txtTenDanhMuc = null!, txtMoTa = null!, txtViTriKe = null!;
        private CheckBox chkDangSuDung = null!;
        private DataGridView dgvCategories = null!;

        public CatalogPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "QUẢN LÝ DANH MỤC SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(520, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Khai báo, cập nhật và kiểm soát danh mục dùng cho toàn bộ đầu sách", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(640, 22), BackColor = Color.Transparent });

            var inputCard = new Panel { Location = new Point(32, 92), Size = new Size(980, 190), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
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

            Controls.Add(inputCard);

            dgvCategories = new DataGridView { Location = new Point(32, 296), Size = new Size(980, 360), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvCategories.Columns.Add("MaDanhMuc", "Mã danh mục");
            dgvCategories.Columns.Add("TenDanhMuc", "Tên danh mục");
            dgvCategories.Columns.Add("MoTa", "Mô tả");
            dgvCategories.Columns.Add("ViTriKe", "Vị trí kệ");
            dgvCategories.Columns.Add("SoDauSach", "Số đầu sách");
            dgvCategories.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvCategories);
            dgvCategories.CellClick += DgvCategories_CellClick;
            Controls.Add(dgvCategories);

            LoadCategories();
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
    }
}
