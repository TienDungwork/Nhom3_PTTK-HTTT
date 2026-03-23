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
        private TextBox txtMaDocGia = null!;
        private TextBox txtTenDocGia = null!;
        private TextBox txtMaSach = null!;
        private TextBox txtMaLo = null!;
        private NumericUpDown nudMinDays = null!;

        public OverduePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "QUẢN LÝ QUÁ HẠN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tìm kiếm phiếu quá hạn theo nhiều tiêu chí (AND), thu phạt và xác nhận trả sách", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(760, 22), BackColor = Color.Transparent });

            var filterCard = new Panel { Location = new Point(32, 92), Size = new Size(1040, 106), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            filterCard.Padding = new Padding(8);
            filterCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, filterCard.Width - 2, filterCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            AddFilterBox(filterCard, "Mã độc giả", 16, out txtMaDocGia);
            AddFilterBox(filterCard, "Tên độc giả", 216, out txtTenDocGia);
            AddFilterBox(filterCard, "Mã sách", 416, out txtMaSach);
            AddFilterBox(filterCard, "Mã lô", 616, out txtMaLo);

            filterCard.Controls.Add(new Label { Text = "Tối thiểu ngày trễ", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(816, 8), Size = new Size(140, 16), BackColor = Color.Transparent });
            nudMinDays = new NumericUpDown { Location = new Point(816, 28), Size = new Size(120, 28), Font = ThemeColors.BodyFont, Minimum = 0, Maximum = 365, Value = 0 };
            nudMinDays.ValueChanged += (_, _) => ApplyFilter();
            filterCard.Controls.Add(nudMinDays);

            var btnConfirm = new RoundedButton { Text = "Xác nhận thu phạt", Size = new Size(170, 36), Location = new Point(16, 64), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnConfirm.Click += BtnConfirm_Click;
            filterCard.Controls.Add(btnConfirm);

            var btnReturn = new RoundedButton { Text = "Xác nhận trả sách", Size = new Size(170, 36), Location = new Point(194, 64), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnReturn.Click += BtnReturn_Click;
            filterCard.Controls.Add(btnReturn);

            Controls.Add(filterCard);

            dgv = new DataGridView { Location = new Point(32, 210), Size = new Size(1040, 444), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("MaDocGia", "Mã ĐG");
            dgv.Columns.Add("TenDocGia", "Tên độc giả");
            dgv.Columns.Add("MaSach", "Mã sách");
            dgv.Columns.Add("MaLo", "Mã lô");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("SoNgay", "Số ngày trễ");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            dgv.Columns.Add("ThuPhat", "Trạng thái phạt");
            ModernDataGridView.ApplyStyle(dgv);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Controls.Add(dgv);

            ApplyFilter();
        }

        private void AddFilterBox(Panel parent, string label, int x, out TextBox txt)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(x, 8), Size = new Size(180, 16), BackColor = Color.Transparent });
            txt = new TextBox { Location = new Point(x, 28), Size = new Size(180, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            txt.TextChanged += (_, _) => ApplyFilter();
            parent.Controls.Add(txt);
        }

        private void ApplyFilter()
        {
            dgv.Rows.Clear();
            var query = SampleData.BorrowRecords.Where(r => r.IsOverdue);

            if (!string.IsNullOrWhiteSpace(txtMaDocGia.Text))
                query = query.Where(r => r.MaDocGia.Contains(txtMaDocGia.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtTenDocGia.Text))
                query = query.Where(r => r.TenDocGia.Contains(txtTenDocGia.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtMaSach.Text))
                query = query.Where(r => r.MaSach.Contains(txtMaSach.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(txtMaLo.Text))
                query = query.Where(r => r.MaLo.Contains(txtMaLo.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if ((int)nudMinDays.Value > 0)
                query = query.Where(r => r.SoNgayQuaHan >= (int)nudMinDays.Value);

            foreach (var r in query.OrderByDescending(r => r.SoNgayQuaHan))
            {
                decimal fine = LibraryDataService.CalculateLateFee(r);
                int idx = dgv.Rows.Add(
                    r.MaMuon,
                    r.MaDocGia,
                    r.TenDocGia,
                    r.MaSach,
                    string.IsNullOrWhiteSpace(r.MaLo) ? "—" : r.MaLo,
                    r.TenSach,
                    r.NgayHenTra.ToString("dd/MM/yyyy"),
                    r.SoNgayQuaHan + " ngày",
                    $"{fine:N0} VNĐ",
                    r.DaThuPhat ? "Đã thu phạt" : "Chưa thu phạt");

                dgv.Rows[idx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                dgv.Rows[idx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
            }
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu quá hạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maMuon = dgv.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            var result = LibraryDataService.CollectFine(maMuon);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (result.Success) ApplyFilter();
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu quá hạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maMuon = dgv.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
            if (record == null)
            {
                MessageBox.Show("Không tìm thấy phiếu mượn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal fine = LibraryDataService.CalculateLateFee(record);
            if (fine > 0 && !record.DaThuPhat)
            {
                MessageBox.Show("Phiếu chưa thu phạt, không thể xác nhận trả sách.", "Chưa đủ điều kiện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LibraryDataService.CompleteReturn(record, DateTime.Now);
            MessageBox.Show($"Đã xác nhận trả sách \"{record.TenSach}\".", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ApplyFilter();
        }
    }
}
