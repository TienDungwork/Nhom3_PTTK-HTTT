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
    public class BorrowHistoryPanel : UserControl
    {
        private TextBox txtSearch = null!;
        private ComboBox cboTrangThai = null!;
        private ComboBox cboSapXep = null!;
        private DataGridView dgv = null!;

        public BorrowHistoryPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "LỊCH SỬ MƯỢN SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent, Anchor = AnchorStyles.Top | AnchorStyles.Left });
            Controls.Add(new Label { Text = "Toàn bộ lịch sử mượn trả sách của bạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            txtSearch = new TextBox { Location = new Point(32, 96), Size = new Size(260, 30), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Tìm theo tên sách..." };
            txtSearch.TextChanged += (_, _) => LoadHistory();
            Controls.Add(txtSearch);

            cboTrangThai = new ComboBox { Location = new Point(304, 96), Size = new Size(180, 30), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "Tất cả trạng thái", "Đang mượn", "Đã trả", "Quá hạn" });
            cboTrangThai.SelectedIndex = 0;
            cboTrangThai.SelectedIndexChanged += (_, _) => LoadHistory();
            Controls.Add(cboTrangThai);

            cboSapXep = new ComboBox { Location = new Point(496, 96), Size = new Size(220, 30), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboSapXep.Items.AddRange(new object[] { "Ngày mượn (mới nhất)", "Ngày mượn (cũ nhất)", "Ngày trả (mới nhất)", "Ngày trả (cũ nhất)" });
            cboSapXep.SelectedIndex = 0;
            cboSapXep.SelectedIndexChanged += (_, _) => LoadHistory();
            Controls.Add(cboSapXep);

            dgv = new DataGridView { Location = new Point(32, 136), Size = new Size(920, 520), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("MaMuon", "Mã phiếu");
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayMuon", "Ngày mượn");
            dgv.Columns.Add("NgayHenTra", "Hạn trả");
            dgv.Columns.Add("NgayTra", "Ngày trả");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns.Add("TienPhat", "Tiền phạt");
            ModernDataGridView.ApplyStyle(dgv);
            Controls.Add(dgv);
            LoadHistory();
        }

        private void LoadHistory()
        {
            dgv.Rows.Clear();
            var cu = UserStore.CurrentUser;
            string maDocGia = cu?.MaDocGia ?? "";
            var records = SampleData.BorrowRecords
                .Where(r => r.MaDocGia == maDocGia)
                .ToList();

            if (records.Count == 0)
                records = SampleData.BorrowRecords.ToList();

            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrWhiteSpace(search))
                records = records.Where(r => r.TenSach.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            string status = cboTrangThai.SelectedItem?.ToString() ?? "Tất cả trạng thái";
            records = status switch
            {
                "Đang mượn" => records.Where(r => r.TrangThai == "Đang mượn").ToList(),
                "Đã trả" => records.Where(r => r.TrangThai == "Đã trả").ToList(),
                "Quá hạn" => records.Where(r => r.IsOverdue).ToList(),
                _ => records
            };

            string sort = cboSapXep.SelectedItem?.ToString() ?? "Ngày mượn (mới nhất)";
            records = sort switch
            {
                "Ngày mượn (cũ nhất)" => records.OrderBy(r => r.NgayMuon).ToList(),
                "Ngày trả (mới nhất)" => records.OrderByDescending(r => r.NgayTraThuc ?? DateTime.MinValue).ToList(),
                "Ngày trả (cũ nhất)" => records.OrderBy(r => r.NgayTraThuc ?? DateTime.MaxValue).ToList(),
                _ => records.OrderByDescending(r => r.NgayMuon).ToList()
            };

            foreach (var r in records)
            {
                string ngayTra = r.NgayTraThuc?.ToString("dd/MM/yyyy") ?? "—";
                decimal fine = r.TienPhat > 0 ? r.TienPhat : LibraryDataService.CalculateLateFee(r);
                string tienPhat = fine > 0 ? $"{fine:N0} VNĐ" : "—";
                int rowIdx = dgv.Rows.Add(r.MaMuon, r.TenSach, r.NgayMuon.ToString("dd/MM/yyyy"), r.NgayHenTra.ToString("dd/MM/yyyy"), ngayTra, r.TrangThai, tienPhat);

                if (r.IsOverdue)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
                else if (r.TrangThai == "Đã trả")
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.Success;
                }
            }
        }
    }
}
