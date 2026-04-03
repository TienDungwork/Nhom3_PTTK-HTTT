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
    /// Quản lý mượn–trả theo 2.1.3: duyệt yêu cầu, phiếu đang mượn (trả + tình trạng sách),
    /// danh sách đã trả, tạo phiếu tại quầy, tìm kiếm theo độc giả/sách.
    /// </summary>
    public class BorrowManagePanel : UserControl
    {
        private TextBox txtSearchRequest = null!;
        private TextBox txtSearchActive = null!;
        private TextBox txtSearchReturned = null!;
        private DataGridView dgvRequests = null!;
        private DataGridView dgvActive = null!;
        private DataGridView dgvReturned = null!;

        private ComboBox cboReader = null!;
        private DataGridView dgvCounterBooks = null!;
        private ComboBox cboCounterCopy = null!;
        private DateTimePicker dtpCounter = null!;
        private TextBox txtCounterDays = null!;

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
                Text = "Duyệt yêu cầu → lập phiếu; kiểm tra điều kiện mượn; tiếp nhận trả và ghi nhận tình trạng sách (Tốt/Hỏng/Mất); tra cứu phiếu đã trả.",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 60),
                Size = new Size(900, 36),
                BackColor = Color.Transparent
            });

            var tabs = new TabControl
            {
                Location = new Point(32, 100),
                Size = new Size(1100, 580),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var tabRequest = new TabPage("1. Duyệt yêu cầu mượn") { BackColor = ThemeColors.Background };
            var tabActive = new TabPage("2. Phiếu đang mượn & trả") { BackColor = ThemeColors.Background };
            var tabReturned = new TabPage("3. Phiếu đã trả") { BackColor = ThemeColors.Background };
            var tabCounter = new TabPage("4. Tạo phiếu tại quầy") { BackColor = ThemeColors.Background };

            InitializeRequestTab(tabRequest);
            InitializeActiveTab(tabActive);
            InitializeReturnedTab(tabReturned);
            InitializeCounterTab(tabCounter);

            tabs.TabPages.Add(tabRequest);
            tabs.TabPages.Add(tabActive);
            tabs.TabPages.Add(tabReturned);
            tabs.TabPages.Add(tabCounter);
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

            var btnApprove = new RoundedButton { Text = "Duyệt phiếu", Size = new Size(130, 36), Location = new Point(356, 11), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnApprove.Click += BtnApprove_Click;
            page.Controls.Add(btnApprove);

            var btnReject = new RoundedButton { Text = "Từ chối", Size = new Size(110, 36), Location = new Point(494, 11), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnReject.Click += BtnReject_Click;
            page.Controls.Add(btnReject);

            var btnRefresh = new RoundedButton { Text = "Làm mới", Size = new Size(110, 36), Location = new Point(612, 11), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnRefresh.Click += (_, _) => LoadRequestGrid();
            page.Controls.Add(btnRefresh);

            dgvRequests = new DataGridView
            {
                Location = new Point(12, 56),
                Size = new Size(1050, 460),
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

        private void InitializeActiveTab(TabPage page)
        {
            txtSearchActive = new TextBox
            {
                Location = new Point(12, 14),
                Size = new Size(360, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã phiếu, mã/tên độc giả, tên sách..."
            };
            txtSearchActive.TextChanged += (_, _) => LoadActiveGrid();
            page.Controls.Add(txtSearchActive);

            var btnFine = new RoundedButton { Text = "Thu phạt (quá hạn)", Size = new Size(150, 36), Location = new Point(386, 11), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnFine.Click += BtnCollectFine_Click;
            page.Controls.Add(btnFine);

            var btnReturn = new RoundedButton { Text = "Xác nhận trả (kiểm tra sách)", Size = new Size(220, 36), Location = new Point(544, 11), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnReturn.Click += BtnReturn_Click;
            page.Controls.Add(btnReturn);

            var btnRefresh = new RoundedButton { Text = "Làm mới", Size = new Size(110, 36), Location = new Point(772, 11), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnRefresh.Click += (_, _) => { LoadActiveGrid(); LoadReturnedGrid(); };
            page.Controls.Add(btnRefresh);

            page.Controls.Add(new Label
            {
                Text = "Danh sách: sách đang mượn. Trước khi duyệt yêu cầu, hệ thống kiểm tra: còn sách, chưa vượt số lượng, không nợ phạt quá hạn.",
                Font = ThemeColors.SmallFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(12, 52),
                Size = new Size(1000, 18),
                BackColor = Color.Transparent
            });

            dgvActive = new DataGridView
            {
                Location = new Point(12, 76),
                Size = new Size(1050, 440),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
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
            page.Controls.Add(dgvActive);

            LoadActiveGrid();
        }

        private void InitializeReturnedTab(TabPage page)
        {
            txtSearchReturned = new TextBox
            {
                Location = new Point(12, 14),
                Size = new Size(400, 30),
                Font = ThemeColors.BodyFont,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Tìm theo mã phiếu, độc giả, tên sách..."
            };
            txtSearchReturned.TextChanged += (_, _) => LoadReturnedGrid();
            page.Controls.Add(txtSearchReturned);

            var btnRefresh = new RoundedButton { Text = "Làm mới", Size = new Size(110, 36), Location = new Point(424, 11), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnRefresh.Click += (_, _) => LoadReturnedGrid();
            page.Controls.Add(btnRefresh);

            dgvReturned = new DataGridView
            {
                Location = new Point(12, 56),
                Size = new Size(1050, 460),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
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
            page.Controls.Add(dgvReturned);

            LoadReturnedGrid();
        }

        private void InitializeCounterTab(TabPage page)
        {
            page.Controls.Add(new Label
            {
                Text = "Chọn độc giả có trong hệ thống, chọn đầu sách và quyển còn sẵn, nhập ngày mượn và số ngày — thủ thư lập phiếu trực tiếp (không qua yêu cầu online).",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(12, 12),
                Size = new Size(1000, 40),
                BackColor = Color.Transparent
            });

            page.Controls.Add(new Label { Text = "Độc giả", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(12, 58), Size = new Size(200, 18), BackColor = Color.Transparent });
            cboReader = new ComboBox { Location = new Point(12, 78), Size = new Size(420, 32), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var r in SampleData.Readers.OrderBy(x => x.MaDocGia))
                cboReader.Items.Add($"{r.MaDocGia} | {r.HoTen}");
            if (cboReader.Items.Count > 0) cboReader.SelectedIndex = 0;
            page.Controls.Add(cboReader);

            dtpCounter = new DateTimePicker { Location = new Point(448, 78), Size = new Size(140, 32), Font = ThemeColors.BodyFont, Format = DateTimePickerFormat.Short };
            page.Controls.Add(new Label { Text = "Ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(448, 58), Size = new Size(100, 18), BackColor = Color.Transparent });
            page.Controls.Add(dtpCounter);

            txtCounterDays = new TextBox { Text = "14", Location = new Point(600, 78), Size = new Size(60, 32), Font = ThemeColors.BodyFont, TextAlign = HorizontalAlignment.Center };
            page.Controls.Add(new Label { Text = "Số ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(600, 58), Size = new Size(120, 18), BackColor = Color.Transparent });
            page.Controls.Add(txtCounterDays);

            var btnCreate = new RoundedButton { Text = "Lập phiếu mượn", Size = new Size(180, 40), Location = new Point(680, 72), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnCreate.Click += BtnCounterCreate_Click;
            page.Controls.Add(btnCreate);

            dgvCounterBooks = new DataGridView { Location = new Point(12, 124), Size = new Size(700, 360), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom };
            dgvCounterBooks.Columns.Add("MaSach", "Mã sách");
            dgvCounterBooks.Columns.Add("TenSach", "Tên sách");
            dgvCounterBooks.Columns.Add("ConLai", "Còn trong kho");
            dgvCounterBooks.Columns.Add("TrangThai", "Đầu sách");
            ModernDataGridView.ApplyStyle(dgvCounterBooks);
            dgvCounterBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCounterBooks.SelectionChanged += (_, _) => LoadCounterCopies();
            page.Controls.Add(dgvCounterBooks);

            page.Controls.Add(new Label { Text = "Quyển sách (Có sẵn)", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(728, 124), Size = new Size(200, 18), BackColor = Color.Transparent });
            cboCounterCopy = new ComboBox { Location = new Point(728, 144), Size = new Size(330, 32), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            page.Controls.Add(cboCounterCopy);

            foreach (var b in LibraryDataService.SearchBooks().OrderBy(x => x.TenSach))
            {
                LibraryDataService.SyncBookStatus(b);
                dgvCounterBooks.Rows.Add(b.MaSach, b.TenSach, b.SoLuongHienCo, b.TrangThai);
            }

            LoadCounterCopies();
        }

        private string GetSelectedReaderCode()
        {
            if (cboReader.SelectedItem == null) return "";
            string raw = cboReader.SelectedItem.ToString() ?? "";
            int idx = raw.IndexOf('|');
            return idx > 0 ? raw.Substring(0, idx).Trim() : raw.Trim();
        }

        private void LoadCounterCopies()
        {
            cboCounterCopy.Items.Clear();
            if (dgvCounterBooks.SelectedRows.Count == 0) return;
            string maSach = dgvCounterBooks.SelectedRows[0].Cells["MaSach"].Value?.ToString() ?? "";
            foreach (var c in LibraryDataService.GetCopiesForBook(maSach, includeUnavailable: false))
                cboCounterCopy.Items.Add($"{c.MaQuyenSach} | {c.TrangThai}");
            if (cboCounterCopy.Items.Count > 0) cboCounterCopy.SelectedIndex = 0;
        }

        private void BtnCounterCreate_Click(object? sender, EventArgs e)
        {
            string maDocGia = GetSelectedReaderCode();
            if (string.IsNullOrWhiteSpace(maDocGia))
            {
                MessageBox.Show("Chọn độc giả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvCounterBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn đầu sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maSach = dgvCounterBooks.SelectedRows[0].Cells["MaSach"].Value?.ToString() ?? "";
            if (!int.TryParse(txtCounterDays.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Số ngày mượn không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maQuyen = "";
            if (cboCounterCopy.SelectedItem != null)
            {
                string raw = cboCounterCopy.SelectedItem.ToString() ?? "";
                int i = raw.IndexOf(" | ", StringComparison.Ordinal);
                maQuyen = i > 0 ? raw.Substring(0, i).Trim() : raw.Trim();
            }

            var result = LibraryDataService.CreateBorrowSlipDirect(maDocGia, maSach, dtpCounter.Value, days, maQuyen);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (result.Success)
            {
                LoadActiveGrid();
                LoadReturnedGrid();
                foreach (DataGridViewRow row in dgvCounterBooks.Rows)
                {
                    if (row.Cells["MaSach"].Value?.ToString() == maSach)
                    {
                        var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
                        if (book != null)
                        {
                            LibraryDataService.SyncBookStatus(book);
                            row.Cells["ConLai"].Value = book.SoLuongHienCo;
                        }
                        break;
                    }
                }
                LoadCounterCopies();
            }
        }

        private void LoadRequestGrid()
        {
            dgvRequests.Rows.Clear();
            string keyword = txtSearchRequest?.Text.Trim() ?? "";

            var data = SampleData.BorrowRequests.OrderByDescending(r => r.NgayTaoYeuCau).AsEnumerable();

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
                    r.MaQuyenSach.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in data)
            {
                int quaHanNgay = r.SoNgayQuaHan;
                decimal fine = LibraryDataService.CalculateLateFee(r);
                r.TienPhat = fine;
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
                decimal fine = r.TienPhat;
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
                MessageBox.Show("Vui lòng chọn phiếu mượn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
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
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadRequestGrid();
        }

        private void BtnCollectFine_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedActiveBorrow();
            if (record == null) return;

            var result = LibraryDataService.CollectFine(record.MaMuon);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadActiveGrid();
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            var record = GetSelectedActiveBorrow();
            if (record == null) return;

            using var dlg = new ReturnBookConditionDialog(record.TenSach, record.MaMuon);
            if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
                return;

            var result = LibraryDataService.ReturnBookByBorrowCode(record.MaMuon, DateTime.Now, dlg.SelectedCondition);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
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
