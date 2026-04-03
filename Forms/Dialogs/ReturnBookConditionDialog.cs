using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms.Dialogs
{
    /// <summary>Thủ thư chọn tình trạng sách khi tiếp nhận trả (Tốt / Hỏng / Mất).</summary>
    public class ReturnBookConditionDialog : Form
    {
        private readonly ComboBox cbo = null!;

        public string SelectedCondition => cbo.SelectedItem?.ToString() ?? "Tốt";

        public ReturnBookConditionDialog(string tenSach, string maMuon)
        {
            Text = "Tiếp nhận trả sách";
            Size = new Size(420, 220);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            Controls.Add(new Label
            {
                Text = $"Phiếu: {maMuon}\nSách: {tenSach}",
                Location = new Point(16, 16),
                Size = new Size(380, 44),
                Font = ThemeColors.BodyFont
            });

            Controls.Add(new Label
            {
                Text = "Tình trạng sách khi trả:",
                Location = new Point(16, 72),
                Size = new Size(200, 22),
                Font = ThemeColors.BodyFont
            });

            cbo = new ComboBox
            {
                Location = new Point(16, 96),
                Size = new Size(360, 30),
                Font = ThemeColors.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo.Items.AddRange(new object[] { "Tốt", "Hỏng", "Mất" });
            cbo.SelectedIndex = 0;
            Controls.Add(cbo);

            var btnOk = new Button { Text = "Xác nhận trả", Location = new Point(200, 144), Size = new Size(100, 32), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Hủy", Location = new Point(308, 144), Size = new Size(68, 32), DialogResult = DialogResult.Cancel };
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
