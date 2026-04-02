using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ReportPanel : UserControl
    {
        private DataGridView dgvInventory = null!;
        private Label lblInventorySummary = null!;
        private InventorySession? activeSession;
        private DataGridView dgvTimeStats = null!;

        public ReportPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            Controls.Add(new Label { Text = "BÁO CÁO & THỐNG KÊ", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(500, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Thống kê sách và tình hình mượn trả", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });
            RenderAdminOverview();

            // --- Report 1: Inventory stats ---
            Controls.Add(new Label { Text = "Báo cáo 1: Thống kê số lượng sách", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 190), Size = new Size(400, 26), BackColor = Color.Transparent });

            var dgv1 = new DataGridView { Location = new Point(32, 220), Size = new Size(920, 190), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            dgv1.Columns.Add("MaSach", "Mã sách");
            dgv1.Columns.Add("TenSach", "Tên sách");
            dgv1.Columns.Add("TacGia", "Tác giả");
            dgv1.Columns.Add("DangMuon", "Đang mượn");
            dgv1.Columns.Add("MatHong", "Mất/hỏng");
            dgv1.Columns.Add("HienCo", "Hiện có");
            dgv1.Columns.Add("ViTri", "Vị trí kho");
            dgv1.Columns.Add("NCC", "Nhà cung cấp");
            ModernDataGridView.ApplyStyle(dgv1);

            foreach (var b in SampleData.Books)
            {
                LibraryDataService.SyncBookStatus(b);
                dgv1.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.SoLuongDangMuon, b.SoLuongMatHong, b.SoLuongHienCo, b.ViTriKho, b.NhaCungCap);
            }
            Controls.Add(dgv1);

            // --- Report 2: Most borrowed ---
            Controls.Add(new Label { Text = "Báo cáo 2: Theo thời gian", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 420), Size = new Size(400, 26), BackColor = Color.Transparent });

            var cboPeriod = new ComboBox { Location = new Point(220, 420), Size = new Size(140, 28), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPeriod.Items.AddRange(new object[] { "day", "month", "year" });
            cboPeriod.SelectedIndex = 1;
            Controls.Add(cboPeriod);

            dgvTimeStats = new DataGridView { Location = new Point(32, 452), Size = new Size(300, 200), Anchor = AnchorStyles.Top | AnchorStyles.Left };
            dgvTimeStats.Columns.Add("Period", "Kỳ");
            dgvTimeStats.Columns.Add("Count", "Số lượt mượn");
            ModernDataGridView.ApplyStyle(dgvTimeStats);
            Controls.Add(dgvTimeStats);
            cboPeriod.SelectedIndexChanged += (_, _) => LoadTimeStats(cboPeriod.SelectedItem?.ToString() ?? "month");
            LoadTimeStats("month");

            Controls.Add(new Label { Text = "Báo cáo 3: Sách được mượn nhiều nhất", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(350, 420), Size = new Size(400, 26), BackColor = Color.Transparent });

            var dgv2 = new DataGridView { Location = new Point(350, 452), Size = new Size(602, 200), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            dgv2.Columns.Add("TenSach", "Tên sách");
            dgv2.Columns.Add("TacGia", "Tác giả");
            dgv2.Columns.Add("TheLoai", "Thể loại");
            dgv2.Columns.Add("SoLanMuon", "Số lần mượn");
            dgv2.Columns.Add("DangMuon", "Đang mượn");
            ModernDataGridView.ApplyStyle(dgv2);

            var borrowCounts = SampleData.BorrowRecords
                .GroupBy(r => r.MaSach)
                .Select(g => new { MaSach = g.Key, Count = g.Count(), ActiveCount = g.Count(r => r.TrangThai == "Đang mượn") })
                .OrderByDescending(x => x.Count)
                .ToList();

            foreach (var bc in borrowCounts)
            {
                var book = SampleData.Books.FirstOrDefault(b => b.MaSach == bc.MaSach);
                if (book != null)
                    dgv2.Rows.Add(book.TenSach, book.TacGia, LibraryDataService.GetCategoryName(book.MaDanhMuc, book.TheLoai), bc.Count, bc.ActiveCount);
            }
            Controls.Add(dgv2);

            Controls.Add(new Label { Text = "Báo cáo 4: Kiểm kê kho theo từng quyển sách", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 664), Size = new Size(440, 26), BackColor = Color.Transparent });

            var btnCreateInventory = new RoundedButton { Text = "Tạo đợt kiểm kê", Size = new Size(150, 36), Location = new Point(32, 696), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnCreateInventory.Click += BtnCreateInventory_Click;
            Controls.Add(btnCreateInventory);

            var btnRunNotify = new RoundedButton { Text = "Chạy nhắc hạn/quá hạn", Size = new Size(190, 36), Location = new Point(190, 696), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnRunNotify.Click += BtnRunNotify_Click;
            Controls.Add(btnRunNotify);

            var btnExport = new RoundedButton { Text = "Xuất báo cáo CSV", Size = new Size(150, 36), Location = new Point(388, 696), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnExport.Click += BtnExport_Click;
            Controls.Add(btnExport);

            lblInventorySummary = new Label { Text = "Chưa có đợt kiểm kê", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(550, 704), Size = new Size(402, 20), BackColor = Color.Transparent };
            Controls.Add(lblInventorySummary);

            dgvInventory = new DataGridView { Location = new Point(32, 738), Size = new Size(920, 280), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgvInventory.Columns.Add("MaQuyenSach", "Mã quyển");
            dgvInventory.Columns.Add("MaSach", "Mã sách");
            dgvInventory.Columns.Add("TenSach", "Tên sách");
            dgvInventory.Columns.Add("HeThong", "TT hệ thống");
            dgvInventory.Columns.Add("ThucTe", "TT thực tế");
            dgvInventory.Columns.Add("ChenhLech", "Chênh lệch");
            dgvInventory.Columns.Add("GhiChu", "Ghi chú");
            ModernDataGridView.ApplyStyle(dgvInventory);
            dgvInventory.CellDoubleClick += DgvInventory_CellDoubleClick;
            Controls.Add(dgvInventory);

            LoadInventoryReport();
        }

        private void RenderAdminOverview()
        {
            dynamic stats = LibraryDataService.GetSystemStatistics();
            string[] cards =
            {
                $"Tổng sách: {stats.TongSoSach}",
                $"Tổng đầu sách: {stats.TongDauSach}",
                $"Tổng danh mục: {stats.TongDanhMuc}",
                $"Tổng người dùng: {stats.TongNguoiDung}",
                $"Đang mượn/Đã trả/Quá hạn: {stats.DangMuon}/{stats.DaTra}/{stats.QuaHan}",
                $"Tổng phạt: {stats.TongTienPhat:N0} | Chưa nộp: {stats.ChuaNopPhat}"
            };
            int x = 32;
            int y = 96;
            foreach (var text in cards)
            {
                var lbl = new Label
                {
                    Text = text,
                    Location = new Point(x, y),
                    Size = new Size(300, 24),
                    Font = ThemeColors.SmallFont,
                    ForeColor = ThemeColors.TextPrimary,
                    BackColor = Color.Transparent
                };
                Controls.Add(lbl);
                y += 26;
            }
        }

        private void LoadTimeStats(string period)
        {
            dgvTimeStats.Rows.Clear();
            foreach (var row in LibraryDataService.GetBorrowStatsByTime(period))
                dgvTimeStats.Rows.Add(row.Label, row.Value);
        }

        private void LoadInventoryReport()
        {
            dgvInventory.Rows.Clear();
            activeSession = LibraryDataService.GetLatestInventorySession();
            if (activeSession == null)
            {
                lblInventorySummary.Text = "Chưa có đợt kiểm kê. Hãy tạo đợt mới để đối soát kho.";
                return;
            }

            int mismatch = 0;
            foreach (var item in activeSession.Items.OrderBy(i => i.MaQuyenSach))
            {
                bool diff = item.ChenhLech;
                if (diff) mismatch++;
                int row = dgvInventory.Rows.Add(item.MaQuyenSach, item.MaSach, item.TenSach, item.TrangThaiHeThong, item.TrangThaiThucTe, diff ? "Có" : "Không", item.GhiChu);
                if (diff)
                {
                    dgvInventory.Rows[row].DefaultCellStyle.BackColor = ThemeColors.DangerLight;
                    dgvInventory.Rows[row].DefaultCellStyle.ForeColor = ThemeColors.Danger;
                }
            }

            lblInventorySummary.Text = $"Đợt {activeSession.MaDotKiemKe} - {activeSession.TenDot}: {activeSession.Items.Count} quyển, lệch {mismatch} quyển.";
        }

        private void BtnCreateInventory_Click(object? sender, EventArgs e)
        {
            string tenDot = PromptInput("Tên đợt kiểm kê:", "Tạo đợt kiểm kê");
            if (string.IsNullOrWhiteSpace(tenDot))
                return;

            string ghiChu = PromptInput("Ghi chú (tùy chọn):", "Tạo đợt kiểm kê");
            var cu = UserStore.CurrentUser;
            string nguoi = cu?.HoTen ?? cu?.Username ?? "Thủ thư";
            var result = LibraryDataService.StartInventorySession(tenDot, nguoi, ghiChu);
            MessageBox.Show(result.Message, result.Success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            LoadInventoryReport();
        }

        private void BtnRunNotify_Click(object? sender, EventArgs e)
        {
            var result = LibraryDataService.RunDueSoonAndOverdueNotificationJob(2);
            MessageBox.Show($"Đã gửi thông báo cho {result.SentCount} độc giả.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            activeSession = LibraryDataService.GetLatestInventorySession();
            if (activeSession == null)
            {
                MessageBox.Show("Chưa có đợt kiểm kê để xuất báo cáo.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var save = new SaveFileDialog
            {
                Title = "Xuất báo cáo kiểm kê",
                Filter = "CSV file (*.csv)|*.csv",
                FileName = $"inventory_{activeSession.MaDotKiemKe}_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };
            if (save.ShowDialog(FindForm()) != DialogResult.OK)
                return;

            string path = LibraryDataService.ExportInventoryReportToCsv(activeSession, save.FileName);
            MessageBox.Show($"Đã xuất báo cáo kiểm kê tại:\n{path}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DgvInventory_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || activeSession == null)
                return;

            string maQuyen = dgvInventory.Rows[e.RowIndex].Cells["MaQuyenSach"].Value?.ToString() ?? "";
            var item = activeSession.Items.FirstOrDefault(i => i.MaQuyenSach == maQuyen);
            if (item == null)
                return;

            string status = PromptInput("Trạng thái thực tế (Có sẵn/Đang mượn/Mất/Hỏng/Bảo trì):", "Cập nhật kiểm kê");
            if (string.IsNullOrWhiteSpace(status))
                return;
            string note = PromptInput("Ghi chú chênh lệch (tùy chọn):", "Cập nhật kiểm kê");
            var update = LibraryDataService.UpdateInventoryItem(activeSession.MaDotKiemKe, item.MaQuyenSach, status, note);
            if (!update.Success)
                MessageBox.Show(update.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LoadInventoryReport();
        }

        private static string PromptInput(string labelText, string title)
        {
            using var dlg = new Form
            {
                Text = title,
                Size = new Size(540, 220),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };
            var lbl = new Label { Text = labelText, Location = new Point(16, 16), Size = new Size(490, 20), Font = ThemeColors.BodyFont };
            var txt = new TextBox { Location = new Point(16, 44), Size = new Size(490, 72), Multiline = true, Font = ThemeColors.BodyFont };
            var btnOk = new Button { Text = "OK", Location = new Point(344, 128), Size = new Size(76, 30), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Hủy", Location = new Point(430, 128), Size = new Size(76, 30), DialogResult = DialogResult.Cancel };
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
