using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public static class ModernDataGridView
    {
        public static void ApplyStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = ThemeColors.BorderLight;
            dgv.Font = ThemeColors.BodyFont;

            // Header style
            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.GridHeaderBackground,
                ForeColor = ThemeColors.TextSecondary,
                Font = new Font("Segoe UI Semibold", 10),
                Padding = new Padding(12, 8, 12, 8),
                SelectionBackColor = ThemeColors.GridHeaderBackground,
                SelectionForeColor = ThemeColors.TextSecondary,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            dgv.ColumnHeadersHeight = 48;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Row style
            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = ThemeColors.TextPrimary,
                SelectionBackColor = ThemeColors.GridSelectedRow,
                SelectionForeColor = ThemeColors.TextPrimary,
                Font = ThemeColors.BodyFont,
                Padding = new Padding(12, 6, 12, 6),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Alternating row
            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.GridAlternateRow,
                ForeColor = ThemeColors.TextPrimary,
                SelectionBackColor = ThemeColors.GridSelectedRow,
                SelectionForeColor = ThemeColors.TextPrimary,
                Font = ThemeColors.BodyFont,
                Padding = new Padding(12, 6, 12, 6)
            };

            dgv.RowTemplate.Height = 44;
            dgv.ScrollBars = ScrollBars.Vertical;

            // Hover effect
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        Color.FromArgb(245, 248, 255);
                }
            };

            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : ThemeColors.GridAlternateRow;
                }
            };
        }
    }
}
