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
    public class BorrowReturnPanel : UserControl
    {
        private DataGridView dgvBooks = null!;
        private DateTimePicker dtpNgayMuon = null!;
        private TextBox txtSoNgay = null!;
        private Label lblNgayTra = null!;
        private Label lblSoLuong = null!;

        public BorrowReturnPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            // Title
            Controls.Add(new Label { Text = "MƯỢN / TRẢ SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Chọn sách và thực hiện mượn hoặc trả", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // --- Search button ---
            var btnTimKiem = new RoundedButton { Text = "Tìm kiếm sách", Size = new Size(160, 42), Location = new Point(32, 96), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnTimKiem.Click += BtnTimKiem_Click;
            Controls.Add(btnTimKiem);

            // --- Book list DGV ---
            dgvBooks = new DataGridView { Location = new Point(32, 150), Size = new Size(900, 260), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            dgvBooks.Columns.Add("MaSach", "Mã sách");
            dgvBooks.Columns.Add("TenSach", "Nhan đề");
            dgvBooks.Columns.Add("TacGia", "Tác giả");
            dgvBooks.Columns.Add("TheLoai", "Thể loại");
            dgvBooks.Columns.Add("ConLai", "Còn lại");
            dgvBooks.Columns.Add("TrangThai", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgvBooks);
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LoadBooks();
            Controls.Add(dgvBooks);

            // --- Bottom panel: borrow form ---
            var bottomCard = new Panel { Location = new Point(32, 424), Size = new Size(900, 200), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            bottomCard.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, bottomCard.Width - 2, bottomCard.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            // Left side: loan info
            lblSoLuong = new Label { Text = "Sách đã chọn: 0", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(260, 28), BackColor = Color.Transparent };
            bottomCard.Controls.Add(lblSoLuong);

            bottomCard.Controls.Add(new Label { Text = "Ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 56), Size = new Size(120, 18), BackColor = Color.Transparent });
            dtpNgayMuon = new DateTimePicker { Location = new Point(20, 76), Size = new Size(180, 32), Font = ThemeColors.BodyFont, Format = DateTimePickerFormat.Short };
            dtpNgayMuon.ValueChanged += UpdateNgayTra;
            bottomCard.Controls.Add(dtpNgayMuon);

            bottomCard.Controls.Add(new Label { Text = "Số ngày mượn", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(220, 56), Size = new Size(120, 18), BackColor = Color.Transparent });
            txtSoNgay = new TextBox { Text = "14", Location = new Point(220, 76), Size = new Size(100, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center };
            txtSoNgay.TextChanged += UpdateNgayTra;
            bottomCard.Controls.Add(txtSoNgay);

            bottomCard.Controls.Add(new Label { Text = "Ngày trả (dự kiến)", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(340, 56), Size = new Size(160, 18), BackColor = Color.Transparent });
            lblNgayTra = new Label
            {
                Text = DateTime.Now.AddDays(14).ToString("dd/MM/yyyy"),
                Font = ThemeColors.SubTitleFont,
                ForeColor = ThemeColors.Primary,
                Location = new Point(340, 76),
                Size = new Size(180, 32),
                BackColor = ThemeColors.InfoLight,
                TextAlign = ContentAlignment.MiddleCenter
            };
            bottomCard.Controls.Add(lblNgayTra);

            // Right side buttons
            var btnMuon = new RoundedButton { Text = "Mượn sách", Size = new Size(150, 44), Location = new Point(600, 56), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnMuon.Click += BtnMuon_Click;
            bottomCard.Controls.Add(btnMuon);

            var btnTra = new RoundedButton { Text = "Trả sách", Size = new Size(150, 44), Location = new Point(600, 112), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnTra.Click += BtnTra_Click;
            bottomCard.Controls.Add(btnTra);

            Controls.Add(bottomCard);

            dgvBooks.SelectionChanged += (s, e) =>
            {
                lblSoLuong.Text = $"Sách đã chọn: {dgvBooks.SelectedRows.Count}";
            };
        }

        private void LoadBooks()
        {
            dgvBooks.Rows.Clear();
            foreach (var b in SampleData.Books)
            {
                dgvBooks.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.TheLoai, b.SoLuongHienCo, b.TrangThai);
            }
        }

        private void UpdateNgayTra(object? sender, EventArgs e)
        {
            if (int.TryParse(txtSoNgay.Text, out int days) && days > 0)
            {
                lblNgayTra.Text = dtpNgayMuon.Value.AddDays(days).ToString("dd/MM/yyyy");
            }
        }

        private void BtnTimKiem_Click(object? sender, EventArgs e)
        {
            // Open search as modal
            using var searchForm = new Form
            {
                Text = "Tìm kiếm sách",
                Size = new Size(1000, 700),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = ThemeColors.Background
            };
            var searchPanel = new SearchBookPanel { Dock = DockStyle.Fill };
            searchForm.Controls.Add(searchPanel);
            searchForm.ShowDialog(FindForm());
        }

        private void BtnMuon_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần mượn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            string maSach = row.Cells["MaSach"].Value?.ToString() ?? "";
            string tenSach = row.Cells["TenSach"].Value?.ToString() ?? "";
            int conLai = int.Parse(row.Cells["ConLai"].Value?.ToString() ?? "0");

            if (conLai <= 0)
            {
                MessageBox.Show("Sách này hiện đã hết!", "Không thể mượn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoNgay.Text, out int soNgay) || soNgay <= 0)
            {
                MessageBox.Show("Số ngày mượn không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var cu = UserStore.CurrentUser;
            var record = new BorrowRecord
            {
                MaMuon = "M" + (SampleData.BorrowRecords.Count + 1).ToString("D3"),
                MaDocGia = cu?.Username ?? "",
                TenDocGia = cu?.HoTen ?? "",
                MaSach = maSach,
                TenSach = tenSach,
                NgayMuon = dtpNgayMuon.Value,
                NgayHenTra = dtpNgayMuon.Value.AddDays(soNgay),
                TrangThai = "Đang mượn"
            };
            SampleData.BorrowRecords.Add(record);

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book != null) book.SoLuongDangMuon++;

            MessageBox.Show($"Mượn sách \"{tenSach}\" thành công!\nHạn trả: {record.NgayHenTra:dd/MM/yyyy}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBooks();
        }

        private void BtnTra_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sách cần trả!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            string maSach = row.Cells["MaSach"].Value?.ToString() ?? "";
            var cu = UserStore.CurrentUser;
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaSach == maSach && r.MaDocGia == (cu?.Username ?? "") && r.TrangThai == "Đang mượn");

            if (record == null)
            {
                MessageBox.Show("Không tìm thấy phiếu mượn cho sách này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            record.NgayTraThuc = DateTime.Now;
            record.TrangThai = "Đã trả";
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book != null && book.SoLuongDangMuon > 0) book.SoLuongDangMuon--;

            MessageBox.Show($"Trả sách \"{record.TenSach}\" thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBooks();
        }
    }
}
