using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class BorrowPanel : UserControl
    {
        private DataGridView dgv = null!;
        private ComboBox cboReader = null!;
        private ComboBox cboBook = null!;
        private DateTimePicker dtpBorrow = null!;
        private DateTimePicker dtpReturn = null!;

        public BorrowPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ MƯỢN - TRẢ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(500, 40),
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Quản lý việc mượn và trả sách",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 62),
                Size = new Size(400, 22),
                BackColor = Color.Transparent
            };
            Controls.Add(lblSubtitle);

            // Left panel: Borrow form
            Panel leftPanel = new Panel
            {
                Location = new Point(32, 100),
                Size = new Size(360, 400),
                BackColor = Color.Transparent
            };
            leftPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(2, 2, leftPanel.Width - 6, leftPanel.Height - 6);
                using (var path = ThemeColors.GetRoundedRect(rect, 12))
                {
                    using (var shadow = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                        g.FillPath(shadow, ThemeColors.GetRoundedRect(new Rectangle(4, 4, leftPanel.Width - 6, leftPanel.Height - 6), 12));
                    using (var bg = new SolidBrush(Color.White))
                        g.FillPath(bg, path);
                }
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (var b = new SolidBrush(ThemeColors.TextPrimary))
                    g.DrawString("📋  Tạo phiếu mượn", ThemeColors.SubTitleFont, b, 20, 16);
            };
            Controls.Add(leftPanel);

            int y = 52;
            Label lblReader = new Label { Text = "Độc giả", Font = new Font("Segoe UI Semibold", 10), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, y), Size = new Size(320, 22), BackColor = Color.Transparent };
            leftPanel.Controls.Add(lblReader);
            y += 24;

            cboReader = new ComboBox
            {
                Location = new Point(20, y), Size = new Size(320, 36),
                Font = ThemeColors.InputFont, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat
            };
            foreach (var r in SampleData.Readers) cboReader.Items.Add($"{r.MaDocGia} - {r.HoTen}");
            if (cboReader.Items.Count > 0) cboReader.SelectedIndex = 0;
            leftPanel.Controls.Add(cboReader);
            y += 44;

            Label lblBook = new Label { Text = "Sách mượn", Font = new Font("Segoe UI Semibold", 10), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, y), Size = new Size(320, 22), BackColor = Color.Transparent };
            leftPanel.Controls.Add(lblBook);
            y += 24;

            cboBook = new ComboBox
            {
                Location = new Point(20, y), Size = new Size(320, 36),
                Font = ThemeColors.InputFont, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat
            };
            foreach (var b in SampleData.Books.Where(x => x.SoLuong > 0))
                cboBook.Items.Add($"{b.MaSach} - {b.TenSach}");
            if (cboBook.Items.Count > 0) cboBook.SelectedIndex = 0;
            leftPanel.Controls.Add(cboBook);
            y += 44;

            Label lblBorrowDate = new Label { Text = "Ngày mượn", Font = new Font("Segoe UI Semibold", 10), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, y), Size = new Size(320, 22), BackColor = Color.Transparent };
            leftPanel.Controls.Add(lblBorrowDate);
            y += 24;

            dtpBorrow = new DateTimePicker
            {
                Location = new Point(20, y), Size = new Size(320, 34), Font = ThemeColors.InputFont,
                Format = DateTimePickerFormat.Short, Value = DateTime.Now
            };
            leftPanel.Controls.Add(dtpBorrow);
            y += 42;

            Label lblReturnDate = new Label { Text = "Ngày hẹn trả", Font = new Font("Segoe UI Semibold", 10), ForeColor = ThemeColors.TextPrimary, Location = new Point(20, y), Size = new Size(320, 22), BackColor = Color.Transparent };
            leftPanel.Controls.Add(lblReturnDate);
            y += 24;

            dtpReturn = new DateTimePicker
            {
                Location = new Point(20, y), Size = new Size(320, 34), Font = ThemeColors.InputFont,
                Format = DateTimePickerFormat.Short, Value = DateTime.Now.AddDays(14)
            };
            leftPanel.Controls.Add(dtpReturn);
            y += 50;

            RoundedButton btnBorrow = new RoundedButton
            {
                Text = "Tạo phiếu mượn",
                IconText = "📝",
                Size = new Size(320, 48),
                Location = new Point(20, y),
                ButtonColor = ThemeColors.Primary,
                Font = new Font("Segoe UI Semibold", 11)
            };
            btnBorrow.Click += BtnBorrow_Click;
            leftPanel.Controls.Add(btnBorrow);

            // Right panel: Borrow list
            dgv = new DataGridView
            {
                Location = new Point(410, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            ModernDataGridView.ApplyStyle(dgv);

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "MaMuon", HeaderText = "Mã mượn", Width = 70 },
                new DataGridViewTextBoxColumn { Name = "TenDG", HeaderText = "Độc giả", FillWeight = 120 },
                new DataGridViewTextBoxColumn { Name = "TenSach", HeaderText = "Tên sách", FillWeight = 150 },
                new DataGridViewTextBoxColumn { Name = "NgayMuon", HeaderText = "Ngày mượn", Width = 90 },
                new DataGridViewTextBoxColumn { Name = "NgayHenTra", HeaderText = "Hẹn trả", Width = 90 },
                new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", Width = 90 },
                new DataGridViewTextBoxColumn { Name = "TienPhat", HeaderText = "Tiền phạt", Width = 100 }
            });

            dgv.CellFormatting += Dgv_CellFormatting;
            Controls.Add(dgv);

            // Return button
            RoundedButton btnReturn = new RoundedButton
            {
                Text = "Trả sách",
                IconText = "✅",
                Size = new Size(140, 44),
                ButtonColor = ThemeColors.Success,
                HoverColor = Color.FromArgb(14, 160, 108),
                Font = ThemeColors.ButtonFont
            };
            btnReturn.Click += BtnReturn_Click;
            Controls.Add(btnReturn);

            Resize += (s, e) =>
            {
                dgv.Size = new Size(Width - 444, Height - 160);
                btnReturn.Location = new Point(Width - 180, Height - 56);
            };

            LoadData();
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var r in SampleData.BorrowRecords.OrderByDescending(x => x.NgayMuon))
            {
                int rowIdx = dgv.Rows.Add(
                    r.MaMuon, r.TenDocGia, r.TenSach,
                    r.NgayMuon.ToString("dd/MM/yyyy"),
                    r.NgayHenTra.ToString("dd/MM/yyyy"),
                    r.TrangThai,
                    r.TienPhat > 0 ? $"{r.TienPhat:N0} VNĐ" : "—"
                );
            }
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgv.Rows[e.RowIndex];
            string trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "";
            string tienPhat = row.Cells["TienPhat"].Value?.ToString() ?? "";

            // Overdue rows in soft red
            if (trangThai == "Đang mượn" && tienPhat != "—" && tienPhat != "")
            {
                row.DefaultCellStyle.BackColor = ThemeColors.DangerLight;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "TrangThai")
            {
                if (trangThai == "Đã trả")
                {
                    e.CellStyle.ForeColor = ThemeColors.Success;
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                }
                else
                {
                    e.CellStyle.ForeColor = ThemeColors.Warning;
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                }
            }

            if (dgv.Columns[e.ColumnIndex].Name == "TienPhat")
            {
                if (tienPhat != "—" && tienPhat != "")
                {
                    e.CellStyle.ForeColor = ThemeColors.Danger;
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                }
            }
        }

        private void BtnBorrow_Click(object? sender, EventArgs e)
        {
            if (cboReader.SelectedIndex < 0 || cboBook.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string readerInfo = cboReader.SelectedItem!.ToString()!;
            string bookInfo = cboBook.SelectedItem!.ToString()!;
            string maDG = readerInfo.Split('-')[0].Trim();
            string maSach = bookInfo.Split('-')[0].Trim();

            var reader = SampleData.Readers.FirstOrDefault(r => r.MaDocGia == maDG);
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);

            if (reader == null || book == null) return;

            var newRecord = new BorrowRecord
            {
                MaMuon = $"M{SampleData.BorrowRecords.Count + 1:D3}",
                MaDocGia = maDG,
                TenDocGia = reader.HoTen,
                MaSach = maSach,
                TenSach = book.TenSach,
                NgayMuon = dtpBorrow.Value,
                NgayHenTra = dtpReturn.Value,
                TrangThai = "Đang mượn"
            };

            SampleData.BorrowRecords.Add(newRecord);
            book.SoLuong--;
            if (book.SoLuong <= 0) book.TrangThai = "Hết sách";

            LoadData();
            MessageBox.Show("Tạo phiếu mượn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string maMuon = dgv.CurrentRow.Cells["MaMuon"].Value?.ToString() ?? "";
            var record = SampleData.BorrowRecords.FirstOrDefault(r => r.MaMuon == maMuon);
            if (record == null || record.TrangThai == "Đã trả") return;

            record.TrangThai = "Đã trả";
            record.NgayTraThuc = DateTime.Now;

            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == record.MaSach);
            if (book != null)
            {
                book.SoLuong++;
                book.TrangThai = "Có sẵn";
            }

            LoadData();
            MessageBox.Show("Trả sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
