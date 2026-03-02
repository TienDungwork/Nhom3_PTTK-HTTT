using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class RoundedButton : Control
    {
        private bool _isHovered = false;
        private bool _isPressed = false;

        public Color ButtonColor { get; set; } = ThemeColors.Primary;
        public Color HoverColor { get; set; } = ThemeColors.PrimaryDark;
        public Color PressedColor { get; set; } = ThemeColors.PrimaryDark;
        public Color TextColor { get; set; } = Color.White;
        public int Radius { get; set; } = ThemeColors.BorderRadius;
        public string IconText { get; set; } = "";
        public bool IsOutline { get; set; } = false;

        public RoundedButton()
        {
            DoubleBuffered = true;
            Size = new Size(140, 42);
            Font = ThemeColors.ButtonFont;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(1, 1, Width - 3, Height - 3);
            using (GraphicsPath path = ThemeColors.GetRoundedRect(rect, Radius))
            {
                Color bgColor = _isPressed ? PressedColor :
                                _isHovered ? HoverColor : ButtonColor;

                if (IsOutline)
                {
                    using (SolidBrush bg = new SolidBrush(
                        _isHovered ? Color.FromArgb(20, ButtonColor) : Color.White))
                    {
                        g.FillPath(bg, path);
                    }
                    using (Pen pen = new Pen(ButtonColor, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                Color textCol = IsOutline ? ButtonColor : TextColor;
                string fullText = string.IsNullOrEmpty(IconText) ? Text : $"{IconText}  {Text}";

                using (SolidBrush textBrush = new SolidBrush(textCol))
                using (StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.DrawString(fullText, Font, textBrush, rect, sf);
                }
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
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }
    }
}
