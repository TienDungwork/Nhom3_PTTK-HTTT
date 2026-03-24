using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class BorrowManagePanel : UserControl
    {
        private TextBox txtSearchRequest = null!;
        private TextBox txtSearchReturn = null!;
        private DataGridView dgvRequests = null!;
        private DataGridView dgvReturns = null!;

        public BorrowManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label
            {
                Text = "QUẢN LÝ MƯỢN TRẢ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(460, 40),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = "Thủ thư duyệt phiếu mượn và xử lí trả sách theo từng tab",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(620, 22),
                BackColor = Color.Transparent
            });

            var tabs = new TabControl
            {
                Location = new Point(32, 96),
                Size = new Size(940, 560),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var tabRequest = new TabPage("Duyệt mượn sách") { BackColor = ThemeColors.Background };
            var tabReturn = new TabPage("Xử lí trả sách") { BackColor = ThemeColors.Background };

            InitializeRequestTab(tabRequest);
            InitializeReturnTab(tabReturn);

            tabs.TabPages.Add(tabRequest);
            tabs.TabPages.Add(tabReturn);
            Controls.Add(tabs);
        }

        private void InitializeRequestTab(TabPage page)
        {
            txtSearchRequest = new TextBox
            {
                Location = new Point(12, 14),
                Size = new Size(330, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã YC, độc giả, mã sách..."
            };
            txtSearchRequest.TextChanged += (_, _) => LoadRequestGrid();
            page.Controls.Add(txtSearchRequest);

            var btnApprove = new RoundedButton
            {
                Text = "Duyệt phiếu",
                Size = new Size(130, 36),
                Location = new Point(356, 11),
                ButtonColor = ThemeColors.Success,
                Font = ThemeColors.ButtonFont
            };
            btnApprove.Click += BtnApprove_Click;
            page.Controls.Add(btnApprove);

            var btnReject = new RoundedButton
            {
                Text = "Từ chối",
                Size = new Size(110, 36),
                Location = new Point(494, 11),
                ButtonColor = ThemeColors.Danger,
                Font = ThemeColors.ButtonFont
            };
            btnReject.Click += BtnReject_Click;
            page.Controls.Add(btnReject);

            var btnRefresh = new RoundedButton
            {
                Text = "Làm mới",
                Size = new Size(110, 36),
                Location = new Point(612, 11),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnRefresh.Click += (_, _) => LoadRequestGrid();
            page.Controls.Add(btnRefresh);

            dgvRequests = new DataGridView
            {
                Location = new Point(12, 56),
                Size = new Size(900, 440),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvRequests.Columns.Add("MaYeuCau", "Mã YC");
            dgvRequests.Columns.Add("MaDocGia", "Mã ĐG");
            dgvRequests.Columns.Add("TenDocGia", "Độc giả");
            dgvRequests.Columns.Add("MaSach", "Mã sách");
            dgvRequests.Columns.Add("TenSach", "Tên sách");
            dgvRequests.Columns.Add("NgayMuonDuKien", "Ngày mượn");
            dgvRequests.Columns.Add("SoNgay", "Số ngày");
            dgvRequests.Columns.Add("TrangThai", "Trạng thái");
            dgvRequests.Columns.Add("NguoiDuyet", "Người duyệt");
            ModernDataGridView.ApplyStyle(dgvRequests);
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            page.Controls.Add(dgvRequests);

            LoadRequestGrid();
        }

        private void InitializeReturnTab(TabPage page)
        {
            txtSearchReturn = new TextBox
            {
                Location = new Point(12, 14),
                Size = new Size(330, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã phiếu, độc giả, sách..."
            };
            txtSearchReturn.TextChanged += (_, _) => LoadReturnGrid();
            page.Controls.Add(txtSearchReturn);

            var btnCollect = new RoundedButton
            {
                Text = "Thu phạt",
                Size = new Size(110, 36),
                Location = new Point(356, 11),
                ButtonColor = ThemeColors.Warning,
                Font = ThemeColors.ButtonFont
            };
            btnCollect.Click += BtnCollectFine_Click;
            page.Controls.Add(btnCollect);

            var btnReturn = new RoundedButton
            {
                Text = "Xác nhận trả",
                Size = new Size(130, 36),
                Location = new Point(474, 11),
                ButtonColor = ThemeColors.Success,
                Font = ThemeColors.ButtonFont
            };
            btnReturn.Click += BtnReturn_Click;
            page.Controls.Add(btnReturn);

            var btnNotify = new RoundedButton
            {
                Text = "Nhắc độc giả",
                Size = new Size(130, 36),
                Location = new Point(612, 11),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnNotify.Click += BtnNotifyReader_Click;
            page.Controls.Add(btnNotify);

            dgvReturns = new DataGridView
            {
                Location = new Point(12, 56),
                Size = new Size(900, 440),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvReturns.Columns.Add("MaMuon", "Mã phiếu");
            dgvReturns.Columns.Add("MaDocGia", "Mã ĐG");
            dgvReturns.Columns.Add("TenDocGia", "Độc giả");
            dgvReturns.Columns.Add("TenSach", "Tên sách");
            dgvReturns.Columns.Add("NgayMuon", "Ngày mượn");
            dgvReturns.Columns.Add("HanTra", "Hạn trả");
            dgvReturns.Columns.Add("QuaHan", "Quá hạn");
            dgvReturns.Columns.Add("TienPhat", "Tiền phạt");
            dgvReturns.Columns.Add("DaThuPhat", "Đã thu");
            ModernDataGridView.ApplyStyle(dgvReturns);
            dgvReturns.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            page.Controls.Add(dgvReturns);

            LoadReturnGrid();
        }

        private void LoadRequestGrid()
        {
            dgvRequests.Rows.Clear();
            string keyword = txtSearchRequest?.Text.Trim() ?? "";

            var data = SampleData.BorrowRequests
                .OrderByDescending(r => r.NgayTaoYeuCau)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                data = data.Where(r =>
                    r.MaYeuCau.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.MaDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.TenDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.MaSach.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.TenSach.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in data)
            {
                int row = dgvRequests.Rows.Add(
                    r.MaYeuCau,
                    r.MaDocGia,
                    r.TenDocGia,
                    r.MaSach,
                    r.TenSach,
                    r.NgayMuonDuKien.ToString("dd/MM/yyyy"),
                    r.SoNgayMuon,
                    r.TrangThai,
                    string.IsNullOrWhiteSpace(r.NguoiDuyet) ? "—" : r.NguoiDuyet);

                if (r.TrangThai == "Chờ duyệt")
                {
                    dgvRequests.Rows[row].DefaultCellStyle.BackColor = ThemeColors.WarningLight;
                }
                else if (r.TrangThai == "Đã duyệt")
                {
                    dgvRequests.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Success;
                }
                else if (r.TrangThai == "Từ chối")
                {
                    dgvRequests.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }
        }

        private void LoadReturnGrid()
        {
            dgvReturns.Rows.Clear();
            string keyword = txtSearchReturn?.Text.Trim() ?? "";

            var data = SampleData.BorrowRecords
                .Where(r => r.TrangThai == "Đang mượn")
                .OrderBy(r => r.NgayHenTra)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                data = data.Where(r =>
                    r.MaMuon.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.MaDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.TenDocGia.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    r.TenSach.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in data)
            {
                int quaHanNgay = r.SoNgayQuaHan;
                decimal fine = LibraryDataService.CalculateLateFee(r);
                r.TienPhat = fine;
                int row = dgvReturns.Rows.Add(
                    r.MaMuon,
                    r.MaDocGia,
                    r.TenDocGia,
                    r.TenSach,
                    r.NgayMuon.ToString("dd/MM/yyyy"),
                    r.NgayHenTra.ToString("dd/MM/yyyy"),
                    quaHanNgay > 0 ? $"{quaHanNgay} ngày" : "Không",
                    fine > 0 ? $"{fine:N0} VNĐ" : "—",
                    r.DaThuPhat ? "Đã thu" : "Chưa thu");

                if (quaHanNgay > 0)
                {
                    dgvReturns.Rows[row].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgvReturns.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }
        }

        private BorrowRequest? GetSelectedRequest()
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu yêu cầu cần xử lý.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string maYeuCau = dgvRequests.SelectedRows[0].Cells["MaYeuCau"].Value?.ToString() ?? "";
            return SampleData.BorrowRequests.FirstOrDefault(r => r.MaYeuCau == maYeuCau);
        }

        private BorrowRecord? GetSelectedBorrow()
        {
            if (dgvReturns.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn cần xử lý trả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string maMuon = dgvReturns.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            return SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
        }

        private void BtnApprove_Click(object? sender, EventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null) return;

            var cu = UserStore.CurrentUser;
            string nguoiDuyet = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            var result = LibraryDataService.ApproveBorrowRequest(request.MaYeuCau, nguoiDuyet);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadRequestGrid();
            LoadReturnGrid();
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null) return;

            string lyDo = PromptInput("Nhập lý do từ chối (có thể để trống):", "Từ chối phiếu mượn");
            var cu = UserStore.CurrentUser;
            string nguoiDuyet = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            var result = LibraryDataService.RejectBorrowRequest(request.MaYeuCau, nguoiDuyet, lyDo);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadRequestGrid();
        }

        private void BtnCollectFine_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedBorrow();
            if (record == null) return;

            var result = LibraryDataService.CollectFine(record.MaMuon);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadReturnGrid();
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedBorrow();
            if (record == null) return;

            var result = LibraryDataService.ReturnBookByBorrowCode(record.MaMuon, DateTime.Now);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadReturnGrid();
        }

        private void BtnNotifyReader_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedBorrow();
            if (record == null) return;

            string message = PromptInput("Nội dung thông báo gửi độc giả:", "Gửi thông báo");
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Nội dung thông báo không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cu = UserStore.CurrentUser;
            string nguoiGui = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            LibraryDataService.SendNotificationToReader(record.MaDocGia, "Thông báo từ thủ thư", message, nguoiGui);
            MessageBox.Show("Đã gửi thông báo cho độc giả.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
