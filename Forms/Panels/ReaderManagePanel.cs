using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Forms.Dialogs;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ReaderManagePanel : UserControl
    {
        private TextBox txtSearch = null!;
        private ComboBox cboTrangThaiMuon = null!;
        private DataGridView dgv = null!;

        public ReaderManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label
            {
                Text = "QUẢN LÝ ĐỘC GIẢ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(420, 40),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = "Quản lý hồ sơ độc giả và gửi thông báo nghiệp vụ",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(560, 22),
                BackColor = Color.Transparent
            });

            txtSearch = new TextBox
            {
                Location = new Point(32, 96),
                Size = new Size(320, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã, tên, email, SĐT..."
            };
            txtSearch.TextChanged += (_, _) => LoadGrid();
            Controls.Add(txtSearch);

            cboTrangThaiMuon = new ComboBox
            {
                Location = new Point(32, 130),
                Size = new Size(220, 30),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTrangThaiMuon.Items.AddRange(new object[]
            {
                "Tất cả độc giả",
                "Đang mượn sách",
                "Đã trả hết",
                "Có phiếu quá hạn"
            });
            cboTrangThaiMuon.SelectedIndex = 0;
            cboTrangThaiMuon.SelectedIndexChanged += (_, _) => LoadGrid();
            Controls.Add(cboTrangThaiMuon);

            var btnAdd = new RoundedButton { Text = "Thêm", Size = new Size(88, 34), Location = new Point(368, 94), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            var btnEdit = new RoundedButton { Text = "Sửa", Size = new Size(88, 34), Location = new Point(462, 94), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnEdit.Click += BtnEdit_Click;
            Controls.Add(btnEdit);

            var btnDelete = new RoundedButton { Text = "Xóa", Size = new Size(88, 34), Location = new Point(556, 94), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnDelete.Click += BtnDelete_Click;
            Controls.Add(btnDelete);

            var btnNotify = new RoundedButton { Text = "Gửi thông báo", Size = new Size(130, 34), Location = new Point(650, 94), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnNotify.Click += BtnNotify_Click;
            Controls.Add(btnNotify);

            dgv = new DataGridView
            {
                Location = new Point(32, 170),
                Size = new Size(940, 486),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgv.Columns.Add("MaDocGia", "Mã độc giả");
            dgv.Columns.Add("HoTen", "Họ tên");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("SDT", "Số điện thoại");
            dgv.Columns.Add("DiaChi", "Địa chỉ");
            dgv.Columns.Add("NgayDangKy", "Ngày đăng ký");
            dgv.Columns.Add("DangMuon", "Đang mượn");
            dgv.Columns.Add("DaTra", "Đã trả");
            dgv.Columns.Add("CanhBao", "Cảnh báo");
            ModernDataGridView.ApplyStyle(dgv);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Controls.Add(dgv);

            LoadGrid();
        }

        private void LoadGrid()
        {
            dgv.Rows.Clear();
            string kw = txtSearch.Text.Trim();
            var data = SampleData.Readers
                .OrderBy(r => r.MaDocGia)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(kw))
            {
                data = data.Where(r =>
                    r.MaDocGia.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    r.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    r.Email.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    r.SDT.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in data)
            {
                int dangMuon = SampleData.BorrowRecords.Count(b => b.MaDocGia == r.MaDocGia && b.TrangThai == "Đang mượn");
                int daTra = SampleData.BorrowRecords.Count(b => b.MaDocGia == r.MaDocGia && b.TrangThai == "Đã trả");
                int quaHan = SampleData.BorrowRecords.Count(b => b.MaDocGia == r.MaDocGia && b.IsOverdue);
                string canhBao = quaHan > 0 ? $"Quá hạn {quaHan} phiếu" : "Bình thường";

                if (!FilterByBorrowStatus(dangMuon, quaHan))
                    continue;

                int row = dgv.Rows.Add(r.MaDocGia, r.HoTen, r.Email, r.SDT, r.DiaChi, r.NgayDangKy.ToString("dd/MM/yyyy"), dangMuon, daTra, canhBao);
                if (quaHan > 0)
                {
                    dgv.Rows[row].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgv.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }
        }

        private bool FilterByBorrowStatus(int dangMuon, int quaHan)
        {
            string filter = cboTrangThaiMuon.SelectedItem?.ToString() ?? "Tất cả độc giả";
            return filter switch
            {
                "Đang mượn sách" => dangMuon > 0,
                "Đã trả hết" => dangMuon == 0,
                "Có phiếu quá hạn" => quaHan > 0,
                _ => true
            };
        }

        private Reader? GetSelectedReader()
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn độc giả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string maDocGia = dgv.SelectedRows[0].Cells["MaDocGia"].Value?.ToString() ?? "";
            return SampleData.Readers.FirstOrDefault(r => r.MaDocGia == maDocGia);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new ReaderDialog();
            if (dlg.ShowDialog(FindForm()) != DialogResult.OK || dlg.ResultReader == null) return;

            var reader = dlg.ResultReader;
            if (SampleData.Readers.Any(r => r.MaDocGia == reader.MaDocGia))
            {
                MessageBox.Show("Mã độc giả đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SampleData.Readers.Add(reader);
            LoadGrid();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var reader = GetSelectedReader();
            if (reader == null) return;

            using var dlg = new ReaderDialog(reader);
            if (dlg.ShowDialog(FindForm()) != DialogResult.OK || dlg.ResultReader == null) return;

            var updated = dlg.ResultReader;
            reader.HoTen = updated.HoTen;
            reader.Email = updated.Email;
            reader.SDT = updated.SDT;
            reader.DiaChi = updated.DiaChi;
            LoadGrid();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var reader = GetSelectedReader();
            if (reader == null) return;

            bool hasBorrow = SampleData.BorrowRecords.Any(r => r.MaDocGia == reader.MaDocGia);
            bool hasPending = SampleData.BorrowRequests.Any(r => r.MaDocGia == reader.MaDocGia && r.TrangThai == "Chờ duyệt");
            if (hasBorrow || hasPending)
            {
                MessageBox.Show("Không thể xóa độc giả đã phát sinh giao dịch mượn/trả hoặc đang có yêu cầu chờ duyệt.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa độc giả {reader.HoTen}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            SampleData.Readers.Remove(reader);
            LoadGrid();
        }

        private void BtnNotify_Click(object? sender, EventArgs e)
        {
            var reader = GetSelectedReader();
            if (reader == null) return;

            string msg = PromptInput("Nội dung thông báo gửi độc giả:", "Gửi thông báo");
            if (string.IsNullOrWhiteSpace(msg))
            {
                MessageBox.Show("Nội dung thông báo không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cu = UserStore.CurrentUser;
            string nguoiGui = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            LibraryDataService.SendNotificationToReader(reader.MaDocGia, "Thông báo từ thủ thư", msg, nguoiGui);
            MessageBox.Show("Đã gửi thông báo thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string PromptInput(string labelText, string title)
        {
            using var dlg = new Form
            {
                Text = title,
                Size = new Size(520, 220),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var lbl = new Label { Text = labelText, Location = new Point(16, 16), Size = new Size(470, 20), Font = ThemeColors.BodyFont };
            var txt = new TextBox { Location = new Point(16, 44), Size = new Size(470, 72), Multiline = true, Font = ThemeColors.BodyFont };
            var btnOk = new Button { Text = "OK", Location = new Point(324, 128), Size = new Size(76, 30), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Hủy", Location = new Point(410, 128), Size = new Size(76, 30), DialogResult = DialogResult.Cancel };
            dlg.Controls.Add(lbl);
            dlg.Controls.Add(txt);
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(btnCancel);
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            return dlg.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : "";
        }
    }
}
