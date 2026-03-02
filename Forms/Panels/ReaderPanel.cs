using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Forms.Dialogs;

namespace LibraryManagement.Forms.Panels
{
    public class ReaderPanel : UserControl
    {
        private DataGridView dgv = null!;
        private RoundedTextBox txtSearch = null!;
        private List<Reader> readers = null!;

        public ReaderPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            readers = new List<Reader>(SampleData.Readers);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ ĐỘC GIẢ",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Quản lý thông tin độc giả thư viện",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 62),
                Size = new Size(400, 22),
                BackColor = Color.Transparent
            };
            Controls.Add(lblSubtitle);

            txtSearch = new RoundedTextBox
            {
                Placeholder = "🔍  Tìm kiếm theo tên, email, SĐT...",
                Location = new Point(32, 100),
                Size = new Size(500, 44)
            };
            txtSearch.TextChanged += (s, e) => FilterData();
            Controls.Add(txtSearch);

            RoundedButton btnAdd = new RoundedButton
            {
                Text = "Thêm độc giả",
                IconText = "➕",
                Size = new Size(170, 44),
                Location = new Point(700, 100),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            dgv = new DataGridView
            {
                Location = new Point(32, 164),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            ModernDataGridView.ApplyStyle(dgv);

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "MaDG", HeaderText = "Mã ĐG", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ và tên", FillWeight = 150 },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 150 },
                new DataGridViewTextBoxColumn { Name = "SDT", HeaderText = "Số điện thoại", Width = 130 },
                new DataGridViewTextBoxColumn { Name = "DiaChi", HeaderText = "Địa chỉ", FillWeight = 120 },
                new DataGridViewTextBoxColumn { Name = "NgayDK", HeaderText = "Ngày đăng ký", Width = 120 }
            });

            Controls.Add(dgv);

            RoundedButton btnEdit = new RoundedButton
            {
                Text = "Sửa", IconText = "✏️", Size = new Size(100, 38),
                ButtonColor = ThemeColors.Warning, HoverColor = Color.FromArgb(217, 142, 8),
                Font = ThemeColors.SmallFont
            };
            RoundedButton btnDelete = new RoundedButton
            {
                Text = "Xóa", IconText = "🗑️", Size = new Size(100, 38),
                ButtonColor = ThemeColors.Danger, HoverColor = Color.FromArgb(220, 50, 50),
                Font = ThemeColors.SmallFont
            };
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            Controls.AddRange(new Control[] { btnEdit, btnDelete });

            Resize += (s, e) =>
            {
                dgv.Size = new Size(Width - 64, Height - 230);
                btnEdit.Location = new Point(Width - 240, Height - 56);
                btnDelete.Location = new Point(Width - 130, Height - 56);
            };

            LoadData();
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var r in readers)
            {
                dgv.Rows.Add(r.MaDocGia, r.HoTen, r.Email, r.SDT, r.DiaChi, r.NgayDangKy.ToString("dd/MM/yyyy"));
            }
        }

        private void FilterData()
        {
            string search = txtSearch.InputText.ToLower();
            readers = SampleData.Readers.Where(r =>
                string.IsNullOrEmpty(search) ||
                r.HoTen.ToLower().Contains(search) ||
                r.Email.ToLower().Contains(search) ||
                r.SDT.Contains(search)
            ).ToList();
            LoadData();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using (var dlg = new ReaderDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultReader != null)
                {
                    SampleData.Readers.Add(dlg.ResultReader);
                    FilterData();
                    MessageBox.Show("Thêm độc giả thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string maDG = dgv.CurrentRow.Cells["MaDG"].Value?.ToString() ?? "";
            var reader = SampleData.Readers.FirstOrDefault(r => r.MaDocGia == maDG);
            if (reader == null) return;

            using (var dlg = new ReaderDialog(reader))
            {
                if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultReader != null)
                {
                    int idx = SampleData.Readers.IndexOf(reader);
                    SampleData.Readers[idx] = dlg.ResultReader;
                    FilterData();
                    MessageBox.Show("Cập nhật độc giả thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string maDG = dgv.CurrentRow.Cells["MaDG"].Value?.ToString() ?? "";

            using (var dlg = new ConfirmDialog("Bạn có chắc chắn muốn xóa độc giả này?", "Xóa độc giả"))
            {
                if (dlg.ShowDialog() == DialogResult.Yes)
                {
                    SampleData.Readers.RemoveAll(r => r.MaDocGia == maDG);
                    FilterData();
                    MessageBox.Show("Xóa độc giả thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
