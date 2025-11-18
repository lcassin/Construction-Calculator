using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MaterialSkin;

namespace ConstructionCalculator
{
    public static class ThemeHelper
    {
        public static GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        public static void ApplyRoundedCorners(Button btn, int radius = 8)
        {
            if (btn.ClientRectangle.Width > 0 && btn.ClientRectangle.Height > 0)
            {
                btn.Region = new Region(CreateRoundedRectPath(btn.ClientRectangle, radius));
            }
        }

        public static Color Lighten(Color color, float amount = 0.1f)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * amount)),
                Math.Min(255, (int)(color.G + (255 - color.G) * amount)),
                Math.Min(255, (int)(color.B + (255 - color.B) * amount))
            );
        }

        public static Color Darken(Color color, float amount = 0.1f)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - amount))),
                Math.Max(0, (int)(color.G * (1 - amount))),
                Math.Max(0, (int)(color.B * (1 - amount)))
            );
        }

        public static void ApplyMauiButtonStyle(Button btn, ButtonType buttonType, bool isDark)
        {
            Color numberButtonColor = isDark ? Color.FromArgb(56, 56, 56) : Color.FromArgb(208, 208, 208);
            Color operatorButtonColor = isDark ? Color.FromArgb(255, 152, 0) : Color.FromArgb(255, 200, 100);
            Color actionButtonColor = isDark ? Color.FromArgb(44, 95, 141) : Color.FromArgb(179, 217, 255);

            btn.FlatStyle = FlatStyle.Flat;
            btn.UseVisualStyleBackColor = false;
            btn.Cursor = Cursors.Hand;

            switch (buttonType)
            {
                case ButtonType.Operator:
                    btn.BackColor = operatorButtonColor;
                    btn.ForeColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseDownBackColor = Darken(operatorButtonColor, 0.15f);
                    btn.FlatAppearance.MouseOverBackColor = Lighten(operatorButtonColor, 0.1f);
                    if (btn.Font.Size < 12)
                        btn.Font = new Font(btn.Font.FontFamily, 10, FontStyle.Bold);
                    break;

                case ButtonType.Action:
                    btn.BackColor = actionButtonColor;
                    btn.ForeColor = isDark ? Color.White : Color.Black;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseDownBackColor = Darken(actionButtonColor, 0.15f);
                    btn.FlatAppearance.MouseOverBackColor = Lighten(actionButtonColor, 0.1f);
                    if (btn.Font.Size < 12)
                        btn.Font = new Font(btn.Font.FontFamily, 10, FontStyle.Regular);
                    break;

                case ButtonType.Number:
                default:
                    btn.BackColor = numberButtonColor;
                    btn.ForeColor = isDark ? Color.White : Color.Black;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseDownBackColor = Darken(numberButtonColor, 0.15f);
                    btn.FlatAppearance.MouseOverBackColor = Lighten(numberButtonColor, 0.1f);
                    if (btn.Font.Size < 12)
                        btn.Font = new Font(btn.Font.FontFamily, 10, FontStyle.Regular);
                    break;
            }

            ApplyRoundedCorners(btn, 8);
            
            btn.Resize -= Button_Resize;
            btn.Resize += Button_Resize;
        }

        private static void Button_Resize(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                ApplyRoundedCorners(btn, 8);
            }
        }

        public static void ApplyMauiTextBoxStyle(TextBox textBox, bool isDark, bool isDisplay = false)
        {
            if (isDisplay)
            {
                textBox.BackColor = isDark ? Color.FromArgb(30, 30, 30) : Color.FromArgb(245, 245, 245);
                textBox.ForeColor = isDark ? Color.White : Color.Black;
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else
            {
                textBox.BackColor = isDark ? Color.FromArgb(30, 30, 30) : Color.White;
                textBox.ForeColor = isDark ? Color.White : Color.Black;
            }
        }

        public static void ApplyMauiLabelStyle(Label label, bool isDark, bool isSecondary = false)
        {
            if (isSecondary)
            {
                label.ForeColor = isDark ? Color.FromArgb(179, 179, 179) : Color.FromArgb(117, 117, 117);
            }
            else
            {
                label.ForeColor = isDark ? Color.White : Color.Black;
            }
        }

        public static ButtonType ClassifyButton(string buttonText)
        {
            if (buttonText == "Calculate" || buttonText == "Calculate Angle" || 
                buttonText == "Calculate Ratio" || buttonText == "Solve" ||
                buttonText == "=" || buttonText == "+" || buttonText == "-" || 
                buttonText == "*" || buttonText == "×" || buttonText == "÷")
            {
                return ButtonType.Operator;
            }
            else if (buttonText == "C" || buttonText == "CE" || buttonText == "Copy" || 
                     buttonText == "Mode" || buttonText == "Clear" || buttonText == "Reset" ||
                     buttonText == "Degrees" || buttonText == "Radians")
            {
                return ButtonType.Action;
            }
            else
            {
                return ButtonType.Number;
            }
        }
    }

    public enum ButtonType
    {
        Number,
        Operator,
        Action
    }
}
