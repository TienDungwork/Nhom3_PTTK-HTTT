using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class SidebarButton : Control
    {
        private bool _isHovered = false;
        private bool _isActive = false;

        public string IconText { get; set; } = "●";
        public Color ActiveColor { get; set; } = Color.White;
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; Invalidate(); }
        }

        public SidebarButton()
        {
            DoubleBuffered = true;
            Size = new Size(ThemeColors.SidebarWidth, 50);
            Font = ThemeColors.SidebarFont;
            Cursor = Cursors.Hand;
            ForeColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background
            Color bgColor;
            if (_isActive)
                bgColor = ThemeColors.SidebarActive;
            else if (_isHovered)
                bgColor = ThemeColors.SidebarHover;
            else
                bgColor = Color.Transparent;

            if (bgColor != Color.Transparent)
            {
                Rectangle bgRect = new Rectangle(8, 4, Width - 16, Height - 8);
                using (GraphicsPath path = ThemeColors.GetRoundedRect(bgRect, 8))
                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }

            // Active indicator
            if (_isActive)
            {
                using (SolidBrush indicator = new SolidBrush(ActiveColor))
                {
                    g.FillRectangle(indicator, 0, Height / 2 - 12, 3, 24);
                }
            }

            // Icon
            using (SolidBrush iconBrush = new SolidBrush(
                _isActive ? Color.White : Color.FromArgb(180, 255, 255, 255)))
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle iconRect = new Rectangle(20, 0, 36, Height);
                g.DrawString(IconText, new Font("Segoe UI Emoji", 14), iconBrush, iconRect, sf);
            }

            // Text
            Color textColor = _isActive ? Color.White : Color.FromArgb(200, 255, 255, 255);
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle textRect = new Rectangle(64, 0, Width - 72, Height);
                g.DrawString(Text, Font, textBrush, textRect, sf);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
