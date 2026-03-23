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
    public class LotManagePanel : UserControl
    {
        private TextBox txtMaLo = null!, txtNhaCungCap = null!, txtSoLuongNhap = null!, txtSoLuongCon = null!, txtGhiChu = null!, txtSearch = null!;
        private ComboBox cboSach = null!, cboTinhTrang = null!;
        private DateTimePicker dtpNgayNhap = null!;
        private DataGridView dgvLots = null!;

        public LotManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label
            {
                Text = "QUẢN LÝ LÔ NHẬP SÁCH",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(520, 40),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = "Theo dõi lô nhập theo ngày nhập, tình trạng mới/cũ và số lượng tồn từng lô",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(760, 22),
                BackColor = Color.Transparent
            });

            var card = new Panel { Location = new Point(32, 92), Size = new Size(1040, 220), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            card.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, card.Width - 2, card.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            AddInput(card, "Mã lô", 16, 12, 140, out txtMaLo);
            AddBookCombo(card, "Đầu sách", 172, 12, 280, out cboSach);
            AddDateInput(card, "Ngày nhập", 468, 12, 150, out dtpNgayNhap);
            AddComboInput(card, "Tình trạng", 634, 12, 120, out cboTinhTrang, new[] { "Mới", "Cũ" });
            AddInput(card, "Nhà cung cấp", 770, 12, 236, out txtNhaCungCap);

            AddInput(card, "Số lượng nhập", 16, 74, 120, out txtSoLuongNhap);
            AddInput(card, "Số lượng còn", 152, 74, 120, out txtSoLuongCon);
            AddInput(card, "Ghi chú", 288, 74, 718, out txtGhiChu);

            var btnThem = new RoundedButton { Text = "Thêm lô", Size = new Size(100, 40), Location = new Point(16, 148), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnThem.Click += (_, _) => SaveLot(false);
            card.Controls.Add(btnThem);

            var btnSua = new RoundedButton { Text = "Cập nhật", Size = new Size(110, 40), Location = new Point(126, 148), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnSua.Click += (_, _) => SaveLot(true);
            card.Controls.Add(btnSua);

            var btnXoa = new RoundedButton { Text = "Xóa lô", Size = new Size(100, 40), Location = new Point(246, 148), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnXoa.Click += BtnXoa_Click;
            card.Controls.Add(btnXoa);

            var btnClear = new RoundedButton { Text = "Xóa form", Size = new Size(100, 40), Location = new Point(356, 148), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnClear.Click += (_, _) => ClearForm();
            card.Controls.Add(btnClear);

            card.Controls.Add(new Label { Text = "Tìm kiếm lô / đầu sách", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(486, 150), Size = new Size(180, 16), BackColor = Color.Transparent });
            txtSearch = new TextBox { Location = new Point(486, 168), Size = new Size(250, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            txtSearch.TextChanged += (_, _) => LoadLots(txtSearch.Text.Trim());
            card.Controls.Add(txtSearch);

            Controls.Add(card);

            dgvLots = new DataGridView { Location = new Point(32, 324), Size = new Size(1040, 332), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvLots.Columns.Add("MaLo", "Mã lô");
            dgvLots.Columns.Add("MaSach", "Mã sách");
            dgvLots.Columns.Add("TenSach", "Tên sách");
            dgvLots.Columns.Add("NgayNhap", "Ngày nhập");
            dgvLots.Columns.Add("TinhTrang", "Tình trạng");
            dgvLots.Columns.Add("SoLuongNhap", "SL nhập");
            dgvLots.Columns.Add("SoLuongCon", "SL còn");
            dgvLots.Columns.Add("SoLuongDaXuat", "SL đã xuất");
            dgvLots.Columns.Add("NCC", "Nhà cung cấp");
            ModernDataGridView.ApplyStyle(dgvLots);
            dgvLots.CellClick += DgvLots_CellClick;
            Controls.Add(dgvLots);

            ReloadBooks();
            ClearForm();
            LoadLots();
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
            if (cboSach.SelectedIndex < 0 && cboSach.Items.Count > 0)
                cboSach.SelectedIndex = 0;
        }

        private void LoadLots(string keyword = "")
        {
            dgvLots.Rows.Clear();
            var lots = LibraryDataService.SearchLots(keyword);
            foreach (var lot in lots)
            {
                dgvLots.Rows.Add(
                    lot.MaLo,
                    lot.MaSach,
                    LibraryDataService.GetBookName(lot.MaSach),
                    lot.NgayNhap.ToString("dd/MM/yyyy"),
                    lot.TinhTrang,
                    lot.SoLuongNhap,
                    lot.SoLuongCon,
                    lot.SoLuongDaXuat,
                    lot.NhaCungCap);
            }
        }

        private void DgvLots_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string maLo = dgvLots.Rows[e.RowIndex].Cells["MaLo"].Value?.ToString() ?? "";
            var lot = SampleData.BookLots.FirstOrDefault(l => l.MaLo == maLo);
            if (lot == null) return;

            txtMaLo.Text = lot.MaLo;
            cboSach.SelectedValue = lot.MaSach;
            dtpNgayNhap.Value = lot.NgayNhap;
            cboTinhTrang.SelectedItem = lot.TinhTrang;
            txtNhaCungCap.Text = lot.NhaCungCap;
            txtSoLuongNhap.Text = lot.SoLuongNhap.ToString();
            txtSoLuongCon.Text = lot.SoLuongCon.ToString();
            txtGhiChu.Text = lot.GhiChu;
        }

        private void SaveLot(bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(txtMaLo.Text))
            {
                MessageBox.Show("Vui lòng nhập mã lô.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuongNhap.Text.Trim(), out int soLuongNhap) ||
                !int.TryParse(txtSoLuongCon.Text.Trim(), out int soLuongCon))
            {
                MessageBox.Show("Số lượng nhập/còn không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lot = new BookLot
            {
                MaLo = txtMaLo.Text.Trim(),
                MaSach = cboSach.SelectedValue?.ToString() ?? "",
                NgayNhap = dtpNgayNhap.Value.Date,
                TinhTrang = cboTinhTrang.SelectedItem?.ToString() ?? "Mới",
                NhaCungCap = txtNhaCungCap.Text.Trim(),
                SoLuongNhap = soLuongNhap,
                SoLuongCon = soLuongCon,
                GhiChu = txtGhiChu.Text.Trim()
            };

            var result = LibraryDataService.SaveLot(lot, isEditMode);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (!result.Success) return;

            LoadLots(txtSearch.Text.Trim());
            ClearForm();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            string maLo = txtMaLo.Text.Trim();
            if (string.IsNullOrWhiteSpace(maLo))
            {
                MessageBox.Show("Vui lòng chọn lô cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa lô {maLo}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var result = LibraryDataService.DeleteLot(maLo);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (!result.Success) return;

            LoadLots(txtSearch.Text.Trim());
            ClearForm();
        }

        private void ClearForm()
        {
            txtMaLo.Text = LibraryDataService.GenerateLotCode();
            txtNhaCungCap.Clear();
            txtSoLuongNhap.Text = "0";
            txtSoLuongCon.Text = "0";
            txtGhiChu.Clear();
            dtpNgayNhap.Value = DateTime.Today;
            if (cboTinhTrang.Items.Count > 0) cboTinhTrang.SelectedIndex = 0;
            if (cboSach.Items.Count > 0) cboSach.SelectedIndex = 0;
        }
    }
}
