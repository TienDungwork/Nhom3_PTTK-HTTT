using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms.Dialogs
{
    public class ConfirmDialog : Form
    {
        public string Message { get; set; } = "Bạn có chắc chắn muốn thực hiện hành động này?";

        public ConfirmDialog(string message, string title = "Xác nhận")
        {
            Message = message;
            InitializeComponent(title);
        }

        private void InitializeComponent(string title)
        {
            Size = new Size(420, 220);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = ThemeColors.GetRoundedRect(rect, 12))
                using (Pen pen = new Pen(ThemeColors.Border, 1))
                    g.DrawPath(pen, path);
            };

            // Warning icon and title
            Panel header = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.White };
            header.Paint += (s, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (SolidBrush b = new SolidBrush(ThemeColors.Danger))
                    e.Graphics.DrawString("⚠", new Font("Segoe UI", 18), b, 20, 12);
                using (SolidBrush b = new SolidBrush(ThemeColors.TextPrimary))
                    e.Graphics.DrawString(title, ThemeColors.TitleFont, b, 56, 16);
            };
            Controls.Add(header);

            // Message
            Label lblMessage = new Label
            {
                Text = Message,
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(28, 70),
                Size = new Size(360, 60),
                BackColor = Color.Transparent
            };
            Controls.Add(lblMessage);

            // Buttons
            RoundedButton btnConfirm = new RoundedButton
            {
                Text = "Xác nhận",
                Size = new Size(120, 42),
                Location = new Point(Width - 280, 150),
                ButtonColor = ThemeColors.Danger,
                HoverColor = Color.FromArgb(220, 50, 50)
            };
            btnConfirm.Click += (s, e) => { DialogResult = DialogResult.Yes; Close(); };

            RoundedButton btnCancel = new RoundedButton
            {
                Text = "Hủy",
                Size = new Size(120, 42),
                Location = new Point(Width - 148, 150),
                IsOutline = true,
                ButtonColor = ThemeColors.Border,
                TextColor = ThemeColors.TextPrimary,
                HoverColor = Color.FromArgb(220, 220, 220)
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.No; Close(); };

            Controls.AddRange(new Control[] { btnConfirm, btnCancel });
        }
    }
}
