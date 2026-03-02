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
    public class OverduePanel : UserControl
    {
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;
        private NumericUpDown nudMinDays = null!;

        public OverduePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "QUẢN LÝ QUÁ HẠN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Theo dõi và xử lý sách quá hạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Filter bar
            var filterCard = new Panel { Location = new Point(32, 92), Size = new Size(920, 70), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            filterCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, filterCard.Width - 2, filterCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            filterCard.Controls.Add(new Label { Text = "Tìm kiếm", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(16, 6), Size = new Size(100, 16), BackColor = Color.Transparent });
            txtSearch = new TextBox { Location = new Point(16, 26), Size = new Size(260, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Mã ĐG / Tên ĐG / Mã sách" };
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            filterCard.Controls.Add(txtSearch);

            filterCard.Controls.Add(new Label { Text = "Tối thiểu ngày quá hạn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(296, 6), Size = new Size(160, 16), BackColor = Color.Transparent });
            nudMinDays = new NumericUpDown { Location = new Point(296, 26), Size = new Size(100, 28), Font = ThemeColors.BodyFont, Minimum = 0, Maximum = 365, Value = 0 };
            nudMinDays.ValueChanged += (s, e) => ApplyFilter();
            filterCard.Controls.Add(nudMinDays);

            var btnExport = new RoundedButton { Text = "Xuất danh sách", Size = new Size(140, 36), Location = new Point(420, 22), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnExport.Click += (s, e) => MessageBox.Show("Đã xuất danh sách quá hạn!", "Xuất dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            filterCard.Controls.Add(btnExport);

            var btnConfirm = new RoundedButton { Text = "Xác nhận thu phạt", Size = new Size(160, 36), Location = new Point(575, 22), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnConfirm.Click += BtnConfirm_Click;
            filterCard.Controls.Add(btnConfirm);

            var btnReturn = new RoundedButton { Text = "Xác nhận trả sách", Size = new Size(160, 36), Location = new Point(750, 22), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnReturn.Click += BtnReturn_Click;
            filterCard.Controls.Add(btnReturn);

            Controls.Add(filterCard);

            // DGV
            dgv = new DataGridView { Location = new Point(32, 174), Size = new Size(920, 480), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("MaDocGia", "Mã ĐG");
            dgv.Columns.Add("TenDocGia", "Tên độc giả");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("SoNgay", "Số ngày quá hạn");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            ModernDataGridView.ApplyStyle(dgv);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Controls.Add(dgv);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            dgv.Rows.Clear();
            var overdues = SampleData.BorrowRecords.Where(r => r.IsOverdue);

            if ((int)nudMinDays.Value > 0)
                overdues = overdues.Where(r => r.SoNgayQuaHan >= (int)nudMinDays.Value);

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string kw = txtSearch.Text.Trim();
                overdues = overdues.Where(r =>
                    r.MaDocGia.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    r.TenDocGia.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    r.MaSach.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in overdues.OrderByDescending(r => r.SoNgayQuaHan))
            {
                decimal tienPhat = r.SoNgayQuaHan * 5000m; // 5,000 VNĐ / ngày
                int rowIdx = dgv.Rows.Add(r.MaMuon, r.MaDocGia, r.TenDocGia, r.TenSach,
                    r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"),
                    r.SoNgayQuaHan.ToString() + " ngày", $"{tienPhat:N0} VNĐ");

                dgv.Rows[rowIdx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
            }

            if (dgv.Rows.Count == 0)
            {
                // Show no data message
            }
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maMuon = dgv.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            string tienPhat = dgv.SelectedRows[0].Cells["TienPhat"].Value?.ToString() ?? "";
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
            if (record != null)
            {
                if (MessageBox.Show($"Xác nhận thu tiền phạt {tienPhat} cho phiếu {maMuon}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    record.TienPhat = record.SoNgayQuaHan * 5000m;
                    MessageBox.Show("Đã xác nhận thu tiền phạt!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maMuon = dgv.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
            if (record != null)
            {
                record.NgayTraThuc = DateTime.Now;
                record.TrangThai = "Đã trả";
                record.TienPhat = record.SoNgayQuaHan * 5000m;
                var book = SampleData.Books.FirstOrDefault(b => b.MaSach == record.MaSach);
                if (book != null && book.SoLuongDangMuon > 0) book.SoLuongDangMuon--;

                MessageBox.Show($"Đã xác nhận trả sách \"{record.TenSach}\"!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ApplyFilter();
            }
        }
    }
}
