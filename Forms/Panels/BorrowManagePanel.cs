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
    /// <summary>
    /// 2.1.3 Quản lý mượn–trả: chỉ có 2 giao diện chính
    /// - Duyệt mượn sách: duyệt/từ chối yêu cầu mượn
    /// - Xử lý trả sách: danh sách đang mượn (thu phạt + xác nhận trả) và danh sách đã trả
    /// </summary>
    public class BorrowManagePanel : UserControl
    {
        private TextBox txtSearchRequest = null!;
        private TextBox txtSearchActive = null!;
        private TextBox txtSearchReturned = null!;

        private DataGridView dgvRequests = null!;
        private DataGridView dgvActive = null!;
        private DataGridView dgvReturned = null!;

        public BorrowManagePanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            // Cùng cách bố cục với OverduePanel: header Dock Top, vùng nội dung Dock Fill
            // để TabControl/DataGridView luôn khớp chiều rộng client — tránh lệch header/cột.
            var header = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Color.Transparent };
            header.Controls.Add(new Label
            {
                Text = "QUẢN LÝ MƯỢN TRẢ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(460, 40),
                BackColor = Color.Transparent
            });
            header.Controls.Add(new Label
            {
                Text = "1) Duyệt mượn sách (yêu cầu mượn)  2) Xử lý trả sách (thu phạt + xác nhận trả, kèm tình trạng sách)",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(980, 36),
                BackColor = Color.Transparent
            });

            var tabs = new TabControl { Dock = DockStyle.Fill };

            var tabRequest = new TabPage("Duyệt mượn sách") { BackColor = ThemeColors.Background };
            var tabReturn = new TabPage("Xử lý trả sách") { BackColor = ThemeColors.Background };

            InitializeRequestTab(tabRequest);
            InitializeReturnTab(tabReturn);

            tabs.TabPages.Add(tabRequest);
            tabs.TabPages.Add(tabReturn);

            // Lề ngang 32px giống OverduePanel (filter + bảng); Padding của Panel luôn áp dụng với Dock Fill
            var tabsWrap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background,
                Padding = new Padding(32, 8, 32, 24)
            };
            tabsWrap.Controls.Add(tabs);

            Controls.Add(tabsWrap);
            Controls.Add(header);
        }

        private void InitializeRequestTab(TabPage page)
        {
            page.Padding = new Padding(0, 8, 0, 0);

            var topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.Transparent
            };

            txtSearchRequest = new TextBox
            {
                Location = new Point(0, 10),
                Size = new Size(330, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã YC, độc giả, mã sách..."
            };
            txtSearchRequest.TextChanged += (_, _) => LoadRequestGrid();
            topBar.Controls.Add(txtSearchRequest);

            var btnApprove = new RoundedButton
            {
                Text = "Duyệt phiếu",
                Size = new Size(130, 36),
                Location = new Point(344, 7),
                ButtonColor = ThemeColors.Success,
                Font = ThemeColors.ButtonFont
            };
            btnApprove.Click += BtnApprove_Click;
            topBar.Controls.Add(btnApprove);

            var btnReject = new RoundedButton
            {
                Text = "Từ chối",
                Size = new Size(110, 36),
                Location = new Point(482, 7),
                ButtonColor = ThemeColors.Danger,
                Font = ThemeColors.ButtonFont
            };
            btnReject.Click += BtnReject_Click;
            topBar.Controls.Add(btnReject);

            var btnRefresh = new RoundedButton
            {
                Text = "Làm mới",
                Size = new Size(110, 36),
                Location = new Point(600, 7),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnRefresh.Click += (_, _) => LoadRequestGrid();
            topBar.Controls.Add(btnRefresh);

            dgvRequests = new DataGridView { Dock = DockStyle.Fill };
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
            dgvRequests.MultiSelect = false;
            ApplyRequestColumnWeights(dgvRequests);

            // Thứ tự Dock WinForms: thêm Fill trước (nền), Top sau — Top chiếm mép trên, Fill phần còn lại
            page.Controls.Add(dgvRequests);
            page.Controls.Add(topBar);

            LoadRequestGrid();
        }

        /// <summary>
        /// Cân FillWeight để cột mã gọn, cột tên rộng — tương tự cảm giác bảng Quản lý quá hạn.
        /// </summary>
        private static void ApplyRequestColumnWeights(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;
            void W(string name, int weight, int minW)
            {
                if (dgv.Columns[name] is { } c)
                {
                    c.FillWeight = weight;
                    c.MinimumWidth = minW;
                }
            }
            W("MaYeuCau", 85, 72);
            W("MaDocGia", 85, 72);
            W("TenDocGia", 140, 120);
            W("MaSach", 90, 72);
            W("TenSach", 160, 100);
            W("NgayMuonDuKien", 95, 88);
            W("SoNgay", 75, 64);
            W("TrangThai", 110, 88);
            W("NguoiDuyet", 100, 88);
        }

        private void InitializeReturnTab(TabPage page)
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };
            page.Controls.Add(split);

            // --- Panel 1: đang mượn ---
            var activeHeader = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.Transparent };
            txtSearchActive = new TextBox
            {
                Location = new Point(12, 12),
                Size = new Size(380, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã phiếu / độc giả / sách..."
            };
            txtSearchActive.TextChanged += (_, _) => LoadActiveGrid();
            activeHeader.Controls.Add(txtSearchActive);

            var btnFine = new RoundedButton
            {
                Text = "Thu phạt",
                Size = new Size(120, 34),
                Location = new Point(406, 10),
                ButtonColor = ThemeColors.Warning,
                Font = ThemeColors.ButtonFont
            };
            btnFine.Click += BtnCollectFine_Click;
            activeHeader.Controls.Add(btnFine);

            var btnReturn = new RoundedButton
            {
                Text = "Xác nhận trả",
                Size = new Size(140, 34),
                Location = new Point(532, 10),
                ButtonColor = ThemeColors.Success,
                Font = ThemeColors.ButtonFont
            };
            btnReturn.Click += BtnReturn_Click;
            activeHeader.Controls.Add(btnReturn);

            var btnRefresh = new RoundedButton
            {
                Text = "Làm mới",
                Size = new Size(110, 34),
                Location = new Point(678, 10),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnRefresh.Click += (_, _) =>
            {
                LoadActiveGrid();
                LoadReturnedGrid();
            };
            activeHeader.Controls.Add(btnRefresh);

            dgvActive = new DataGridView { Dock = DockStyle.Fill };
            dgvActive.Columns.Add("MaMuon", "Mã phiếu");
            dgvActive.Columns.Add("MaQuyen", "Mã quyển");
            dgvActive.Columns.Add("MaDocGia", "Mã ĐG");
            dgvActive.Columns.Add("TenDocGia", "Độc giả");
            dgvActive.Columns.Add("TenSach", "Tên sách");
            dgvActive.Columns.Add("NgayMuon", "Ngày mượn");
            dgvActive.Columns.Add("HanTra", "Hạn trả");
            dgvActive.Columns.Add("QuaHan", "Quá hạn");
            dgvActive.Columns.Add("TienPhat", "Tiền phạt");
            dgvActive.Columns.Add("DaThuPhat", "Thu phạt");

            ModernDataGridView.ApplyStyle(dgvActive);
            dgvActive.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvActive.MultiSelect = false;

            split.Panel1.Controls.Add(dgvActive);
            split.Panel1.Controls.Add(activeHeader);

            // --- Panel 2: đã trả ---
            var returnedHeader = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.Transparent };
            txtSearchReturned = new TextBox
            {
                Location = new Point(12, 12),
                Size = new Size(400, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã phiếu / độc giả / sách..."
            };
            txtSearchReturned.TextChanged += (_, _) => LoadReturnedGrid();
            returnedHeader.Controls.Add(txtSearchReturned);

            var btnRefreshReturned = new RoundedButton
            {
                Text = "Làm mới",
                Size = new Size(110, 34),
                Location = new Point(424, 10),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnRefreshReturned.Click += (_, _) => LoadReturnedGrid();
            returnedHeader.Controls.Add(btnRefreshReturned);

            dgvReturned = new DataGridView { Dock = DockStyle.Fill };
            dgvReturned.Columns.Add("MaMuon", "Mã phiếu");
            dgvReturned.Columns.Add("MaDocGia", "Mã ĐG");
            dgvReturned.Columns.Add("TenDocGia", "Độc giả");
            dgvReturned.Columns.Add("TenSach", "Tên sách");
            dgvReturned.Columns.Add("NgayMuon", "Ngày mượn");
            dgvReturned.Columns.Add("HanTra", "Hạn trả");
            dgvReturned.Columns.Add("NgayTra", "Ngày trả");
            dgvReturned.Columns.Add("TraMuon", "Trả muộn");
            dgvReturned.Columns.Add("TinhTrangSach", "Tình trạng sách");
            dgvReturned.Columns.Add("TienPhat", "Tiền phạt");

            ModernDataGridView.ApplyStyle(dgvReturned);
            dgvReturned.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReturned.MultiSelect = false;

            split.Panel2.Controls.Add(dgvReturned);
            split.Panel2.Controls.Add(returnedHeader);

            LoadActiveGrid();
            LoadReturnedGrid();
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
                    dgvRequests.Rows[row].DefaultCellStyle.BackColor = ThemeColors.WarningLight;
                else if (r.TrangThai == "Đã duyệt")
                    dgvRequests.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Success;
                else if (r.TrangThai == "Từ chối")
                    dgvRequests.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
            }
        }

        private void LoadActiveGrid()
        {
            dgvActive.Rows.Clear();
            string keyword = txtSearchActive?.Text.Trim() ?? "";

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
                    r.TenSach.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (r.MaQuyenSach?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            foreach (var r in data)
            {
                int quaHanNgay = r.SoNgayQuaHan;
                decimal fine = LibraryDataService.CalculateLateFee(r);

                int row = dgvActive.Rows.Add(
                    r.MaMuon,
                    string.IsNullOrWhiteSpace(r.MaQuyenSach) ? "—" : r.MaQuyenSach,
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
                    dgvActive.Rows[row].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgvActive.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }
        }

        private void LoadReturnedGrid()
        {
            dgvReturned.Rows.Clear();
            string keyword = txtSearchReturned?.Text.Trim() ?? "";

            var data = SampleData.BorrowRecords
                .Where(r => r.TrangThai == "Đã trả")
                .OrderByDescending(r => r.NgayTraThuc ?? r.NgayMuon)
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
                string traMuon = r.LaTraMuon ? "Có" : "Không";
                string tinhTrang = string.IsNullOrWhiteSpace(r.TinhTrangSachKhiTra) ? "Tốt" : r.TinhTrangSachKhiTra;
                decimal fine = r.TienPhat > 0 ? r.TienPhat : LibraryDataService.CalculateLateFee(r);

                dgvReturned.Rows.Add(
                    r.MaMuon,
                    r.MaDocGia,
                    r.TenDocGia,
                    r.TenSach,
                    r.NgayMuon.ToString("dd/MM/yyyy"),
                    r.NgayHenTra.ToString("dd/MM/yyyy"),
                    r.NgayTraThuc?.ToString("dd/MM/yyyy") ?? "—",
                    traMuon,
                    tinhTrang,
                    fine > 0 ? $"{fine:N0} VNĐ" : "—");
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

        private BorrowRecord? GetSelectedActiveBorrow()
        {
            if (dgvActive.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu mượn đang mượn cần xử lý.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string maMuon = dgvActive.SelectedRows[0].Cells["MaMuon"].Value?.ToString() ?? "";
            return SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
        }

        private void BtnApprove_Click(object? sender, EventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null) return;

            var cu = UserStore.CurrentUser;
            string nguoiDuyet = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            var result = LibraryDataService.ApproveBorrowRequest(request.MaYeuCau, nguoiDuyet);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadRequestGrid();
            LoadActiveGrid();
            LoadReturnedGrid();
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            var request = GetSelectedRequest();
            if (request == null) return;

            string lyDo = PromptInput("Nhập lý do từ chối (có thể để trống):", "Từ chối phiếu mượn");
            var cu = UserStore.CurrentUser;
            string nguoiDuyet = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            var result = LibraryDataService.RejectBorrowRequest(request.MaYeuCau, nguoiDuyet, lyDo);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadRequestGrid();
        }

        private void BtnCollectFine_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedActiveBorrow();
            if (record == null) return;

            var result = LibraryDataService.CollectFine(record.MaMuon);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadActiveGrid();
            LoadReturnedGrid();
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedActiveBorrow();
            if (record == null) return;

            using var dlg = new ReturnBookConditionDialog(record.TenSach, record.MaMuon);
            if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
                return;

            var result = LibraryDataService.ReturnBookByBorrowCode(record.MaMuon, DateTime.Now, dlg.SelectedCondition);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadActiveGrid();
            LoadReturnedGrid();
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
