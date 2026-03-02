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
    public class BookPanel : UserControl
    {
        private DataGridView dgv = null!;
        private RoundedTextBox txtSearch = null!;
        private ComboBox cboCategory = null!;
        private List<Book> books = null!;

        public BookPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            books = new List<Book>(SampleData.Books);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ SÁCH",
                Font = ThemeColors.HeaderFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, 20),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            };
            Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Quản lý danh sách sách trong thư viện",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(32, 62),
                Size = new Size(400, 22),
                BackColor = Color.Transparent
            };
            Controls.Add(lblSubtitle);

            // Search bar
            txtSearch = new RoundedTextBox
            {
                Placeholder = "🔍  Tìm kiếm theo tên sách, tác giả, ISBN...",
                Location = new Point(32, 100),
                Size = new Size(450, 44)
            };
            txtSearch.TextChanged += (s, e) => FilterBooks();
            Controls.Add(txtSearch);

            // Category filter
            cboCategory = new ComboBox
            {
                Location = new Point(500, 104),
                Size = new Size(180, 36),
                Font = ThemeColors.InputFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            foreach (var cat in SampleData.Categories) cboCategory.Items.Add(cat);
            cboCategory.SelectedIndex = 0;
            cboCategory.SelectedIndexChanged += (s, e) => FilterBooks();
            Controls.Add(cboCategory);

            // Add button
            RoundedButton btnAdd = new RoundedButton
            {
                Text = "Thêm sách",
                IconText = "➕",
                Size = new Size(160, 44),
                Location = new Point(700, 100),
                ButtonColor = ThemeColors.Primary,
                Font = ThemeColors.ButtonFont
            };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            // DataGridView
            dgv = new DataGridView
            {
                Location = new Point(32, 164),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            ModernDataGridView.ApplyStyle(dgv);

            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "MaSach", HeaderText = "Mã sách", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "TenSach", HeaderText = "Tên sách", FillWeight = 200 },
                new DataGridViewTextBoxColumn { Name = "TacGia", HeaderText = "Tác giả", FillWeight = 120 },
                new DataGridViewTextBoxColumn { Name = "ISBN", HeaderText = "ISBN", Width = 140 },
                new DataGridViewTextBoxColumn { Name = "TheLoai", HeaderText = "Thể loại", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "SoLuong", HeaderText = "Số lượng", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", Width = 100 }
            });

            dgv.CellFormatting += Dgv_CellFormatting;
            Controls.Add(dgv);

            // Action buttons panel
            RoundedButton btnEdit = new RoundedButton
            {
                Text = "Sửa",
                IconText = "✏️",
                Size = new Size(100, 38),
                Location = new Point(32, 165),
                ButtonColor = ThemeColors.Warning,
                HoverColor = Color.FromArgb(217, 142, 8),
                Font = ThemeColors.SmallFont,
                Visible = false,
                Tag = "actionBtn"
            };

            RoundedButton btnDelete = new RoundedButton
            {
                Text = "Xóa",
                IconText = "🗑️",
                Size = new Size(100, 38),
                Location = new Point(140, 165),
                ButtonColor = ThemeColors.Danger,
                HoverColor = Color.FromArgb(220, 50, 50),
                Font = ThemeColors.SmallFont,
                Visible = false,
                Tag = "actionBtn"
            };

            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            // We'll show them below DGV in a final layout
            // For now these are positioned but hidden; we handle via resize
            Controls.AddRange(new Control[] { btnEdit, btnDelete });

            Resize += (s, e) =>
            {
                dgv.Size = new Size(Width - 64, Height - 230);
                btnEdit.Location = new Point(Width - 240, Height - 56);
                btnEdit.Visible = true;
                btnDelete.Location = new Point(Width - 130, Height - 56);
                btnDelete.Visible = true;
            };

            LoadData();
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var b in books)
            {
                dgv.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.ISBN, b.TheLoai, b.SoLuong, b.TrangThai);
            }
        }

        private void FilterBooks()
        {
            string search = txtSearch.InputText.ToLower();
            string category = cboCategory.SelectedItem?.ToString() ?? "Tất cả";

            var filtered = SampleData.Books.Where(b =>
            {
                bool matchSearch = string.IsNullOrEmpty(search) ||
                    b.TenSach.ToLower().Contains(search) ||
                    b.TacGia.ToLower().Contains(search) ||
                    b.ISBN.ToLower().Contains(search);

                bool matchCategory = category == "Tất cả" || b.TheLoai == category;

                return matchSearch && matchCategory;
            }).ToList();

            books = filtered;
            LoadData();
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == "TrangThai")
            {
                string val = e.Value?.ToString() ?? "";
                if (val == "Hết sách")
                {
                    e.CellStyle.BackColor = ThemeColors.DangerLight;
                    e.CellStyle.ForeColor = ThemeColors.Danger;
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                }
                else
                {
                    e.CellStyle.BackColor = ThemeColors.SuccessLight;
                    e.CellStyle.ForeColor = ThemeColors.Success;
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using (var dlg = new BookDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultBook != null)
                {
                    SampleData.Books.Add(dlg.ResultBook);
                    FilterBooks();
                    MessageBox.Show("Thêm sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string maSach = dgv.CurrentRow.Cells["MaSach"].Value?.ToString() ?? "";
            var book = SampleData.Books.FirstOrDefault(b => b.MaSach == maSach);
            if (book == null) return;

            using (var dlg = new BookDialog(book))
            {
                if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultBook != null)
                {
                    int idx = SampleData.Books.IndexOf(book);
                    SampleData.Books[idx] = dlg.ResultBook;
                    FilterBooks();
                    MessageBox.Show("Cập nhật sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            string maSach = dgv.CurrentRow.Cells["MaSach"].Value?.ToString() ?? "";

            using (var dlg = new ConfirmDialog("Bạn có chắc chắn muốn xóa sách này?", "Xóa sách"))
            {
                if (dlg.ShowDialog() == DialogResult.Yes)
                {
                    SampleData.Books.RemoveAll(b => b.MaSach == maSach);
                    FilterBooks();
                    MessageBox.Show("Xóa sách thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
