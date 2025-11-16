using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class AngleCalculatorForm : MaterialForm
    {
        private TextBox riseTextBox;
        private TextBox runTextBox;
        private TextBox angleTextBox;
        private TextBox resultLabel;
        private bool isRadianMode = false;
        
        private TextBox solverAngleTextBox;
        private TextBox solverOppositeTextBox;
        private TextBox solverAdjacentTextBox;
        private TextBox solverHypotenuseTextBox;
        private TextBox solverResultTextBox;

        public AngleCalculatorForm()
        {
            InitializeComponent();
            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            
            try
            {
                this.Icon = new Icon("Assets/SmallTile.scale-100.ico");
            }
            catch
            {
            }
            
            ApplyMauiStyling();
            this.Activated += (s, e) => BeginInvoke(new Action(ApplyMauiStyling));
        }

        private void ApplyMauiStyling()
        {
            bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
            
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    ButtonType buttonType = ThemeHelper.ClassifyButton(btn.Text);
                    ThemeHelper.ApplyMauiButtonStyle(btn, buttonType, isDark);
                }
                else if (control is TextBox textBox)
                {
                    ThemeHelper.ApplyMauiTextBoxStyle(textBox, isDark, isDisplay: textBox.ReadOnly);
                }
                else if (control is Label label)
                {
                    ThemeHelper.ApplyMauiLabelStyle(label, isDark, isSecondary: false);
                }
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Angle Calculator";
            this.ClientSize = new Size(400, 750);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label riseLabel = new Label
            {
                Location = new Point(20, 90),
                Size = new Size(100, 25),
                Text = "Rise:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(riseLabel);

            riseTextBox = new TextBox
            {
                Location = new Point(130, 90),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 4' or 4\" or 4"
            };
            this.Controls.Add(riseTextBox);

            Label runLabel = new Label
            {
                Location = new Point(20, 130),
                Size = new Size(100, 25),
                Text = "Run:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(runLabel);

            runTextBox = new TextBox
            {
                Location = new Point(130, 130),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 12' or 12\" or 12"
            };
            this.Controls.Add(runTextBox);

            Button calcAngleButton = new Button
            {
                Location = new Point(130, 170),
                Size = new Size(240, 35),
                Text = "Calculate Angle",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            calcAngleButton.Click += CalculateAngle;
            this.Controls.Add(calcAngleButton);

            Label angleLabel = new Label
            {
                Location = new Point(20, 220),
                Size = new Size(100, 25),
                Text = "Angle:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(angleLabel);

            angleTextBox = new TextBox
            {
                Location = new Point(130, 220),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 45 (degrees by default)"
            };
            this.Controls.Add(angleTextBox);

            Button modeButton = new Button
            {
                Location = new Point(130, 255),
                Size = new Size(115, 30),
                Text = "Degrees",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(200, 200, 200),
                UseVisualStyleBackColor = false
            };
            modeButton.Click += (s, e) =>
            {
                isRadianMode = !isRadianMode;
                modeButton.Text = isRadianMode ? "Radians" : "Degrees";
            };
            this.Controls.Add(modeButton);

            Button calcRatioButton = new Button
            {
                Location = new Point(255, 255),
                Size = new Size(115, 30),
                Text = "Calculate Ratio",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            calcRatioButton.Click += CalculateRatio;
            this.Controls.Add(calcRatioButton);

            resultLabel = new TextBox
            {
                Location = new Point(20, 300),
                Size = new Size(360, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Multiline = true,
                TabStop = false
            };
            this.Controls.Add(resultLabel);
            
            Label solverSectionLabel = new Label
            {
                Location = new Point(20, 365),
                Size = new Size(360, 25),
                Text = "Right Triangle Solver",
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            this.Controls.Add(solverSectionLabel);

            Label solverInstructionLabel = new Label
            {
                Location = new Point(20, 395),
                Size = new Size(360, 30),
                Text = "Enter any 2 values to calculate the rest:",
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            this.Controls.Add(solverInstructionLabel);

            Label solverAngleLabel = new Label
            {
                Location = new Point(20, 430),
                Size = new Size(100, 25),
                Text = "Angle:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(solverAngleLabel);

            solverAngleTextBox = new TextBox
            {
                Location = new Point(130, 430),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 45 (degrees)"
            };
            this.Controls.Add(solverAngleTextBox);

            Label solverOppositeLabel = new Label
            {
                Location = new Point(20, 465),
                Size = new Size(100, 25),
                Text = "Opposite (Rise):",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(solverOppositeLabel);

            solverOppositeTextBox = new TextBox
            {
                Location = new Point(130, 465),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 4' or 48\""
            };
            this.Controls.Add(solverOppositeTextBox);

            Label solverAdjacentLabel = new Label
            {
                Location = new Point(20, 500),
                Size = new Size(100, 25),
                Text = "Adjacent (Run):",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(solverAdjacentLabel);

            solverAdjacentTextBox = new TextBox
            {
                Location = new Point(130, 500),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 12' or 144\""
            };
            this.Controls.Add(solverAdjacentTextBox);

            Label solverHypotenuseLabel = new Label
            {
                Location = new Point(20, 535),
                Size = new Size(100, 25),
                Text = "Hypotenuse:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(solverHypotenuseLabel);

            solverHypotenuseTextBox = new TextBox
            {
                Location = new Point(130, 535),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 12' 8\""
            };
            this.Controls.Add(solverHypotenuseTextBox);

            Button solverCalculateButton = new Button
            {
                Location = new Point(130, 575),
                Size = new Size(115, 35),
                Text = "Calculate",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            solverCalculateButton.Click += SolveTriangle;
            this.Controls.Add(solverCalculateButton);

            Button solverClearButton = new Button
            {
                Location = new Point(255, 575),
                Size = new Size(115, 35),
                Text = "Clear",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(200, 200, 200),
                UseVisualStyleBackColor = false
            };
            solverClearButton.Click += (s, e) =>
            {
                solverAngleTextBox.Clear();
                solverOppositeTextBox.Clear();
                solverAdjacentTextBox.Clear();
                solverHypotenuseTextBox.Clear();
                solverResultTextBox.Clear();
            };
            this.Controls.Add(solverClearButton);

            solverResultTextBox = new TextBox
            {
                Location = new Point(20, 625),
                Size = new Size(360, 100),
                Font = new Font("Consolas", 9),
                TextAlign = HorizontalAlignment.Left,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Multiline = true,
                TabStop = false,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(solverResultTextBox);
        }

        private void CalculateAngle(object? sender, EventArgs e)
        {
            try
            {
                Measurement rise = Measurement.Parse(riseTextBox.Text);
                Measurement run = Measurement.Parse(runTextBox.Text);

                double riseInches = rise.ToTotalInches();
                double runInches = run.ToTotalInches();

                double angleRadians = Math.Atan2(riseInches, runInches);
                double angleDegrees = angleRadians * (180.0 / Math.PI);

                resultLabel.Text = $"Angle: {angleDegrees:F2}° ({angleRadians:F4} rad)\nRatio: {riseInches:F2}:{runInches:F2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateRatio(object? sender, EventArgs e)
        {
            try
            {
                double angle = double.Parse(angleTextBox.Text);
                double angleRadians = isRadianMode ? angle : angle * (Math.PI / 180.0);

                double runInches = 12.0;
                double riseInches = Math.Tan(angleRadians) * runInches;

                Measurement rise = Measurement.FromDecimalInches(riseInches);
                Measurement run = Measurement.FromDecimalInches(runInches);

                resultLabel.Text = $"Rise: {rise.ToFractionString()}\nRun: {run.ToFractionString()}\n(for 12\" run)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SolveTriangle(object? sender, EventArgs e)
        {
            try
            {
                bool hasAngle = !string.IsNullOrWhiteSpace(solverAngleTextBox.Text);
                bool hasOpposite = !string.IsNullOrWhiteSpace(solverOppositeTextBox.Text);
                bool hasAdjacent = !string.IsNullOrWhiteSpace(solverAdjacentTextBox.Text);
                bool hasHypotenuse = !string.IsNullOrWhiteSpace(solverHypotenuseTextBox.Text);

                int inputCount = (hasAngle ? 1 : 0) + (hasOpposite ? 1 : 0) + (hasAdjacent ? 1 : 0) + (hasHypotenuse ? 1 : 0);

                if (inputCount != 2)
                {
                    MessageBox.Show("Please enter exactly 2 values to calculate the rest.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                double? angle = null;
                double? opposite = null;
                double? adjacent = null;
                double? hypotenuse = null;

                StringBuilder result = new StringBuilder();
                StringBuilder formulas = new StringBuilder();

                if (hasAngle)
                {
                    angle = double.Parse(solverAngleTextBox.Text);
                    if (!isRadianMode)
                    {
                        angle = angle.Value * (Math.PI / 180.0);
                    }
                }
                if (hasOpposite)
                {
                    opposite = Measurement.Parse(solverOppositeTextBox.Text).ToTotalInches();
                }
                if (hasAdjacent)
                {
                    adjacent = Measurement.Parse(solverAdjacentTextBox.Text).ToTotalInches();
                }
                if (hasHypotenuse)
                {
                    hypotenuse = Measurement.Parse(solverHypotenuseTextBox.Text).ToTotalInches();
                }

                if (hasAngle && hasOpposite)
                {
                    adjacent = opposite.Value / Math.Tan(angle.Value);
                    hypotenuse = opposite.Value / Math.Sin(angle.Value);
                    formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                    formulas.AppendLine("      sin(θ) = opposite/hypotenuse");
                }
                else if (hasAngle && hasAdjacent)
                {
                    opposite = adjacent.Value * Math.Tan(angle.Value);
                    hypotenuse = adjacent.Value / Math.Cos(angle.Value);
                    formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                    formulas.AppendLine("      cos(θ) = adjacent/hypotenuse");
                }
                else if (hasAngle && hasHypotenuse)
                {
                    opposite = hypotenuse.Value * Math.Sin(angle.Value);
                    adjacent = hypotenuse.Value * Math.Cos(angle.Value);
                    formulas.AppendLine("Used: sin(θ) = opposite/hypotenuse");
                    formulas.AppendLine("      cos(θ) = adjacent/hypotenuse");
                }
                else if (hasOpposite && hasAdjacent)
                {
                    angle = Math.Atan2(opposite.Value, adjacent.Value);
                    hypotenuse = Math.Sqrt(opposite.Value * opposite.Value + adjacent.Value * adjacent.Value);
                    formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                    formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
                }
                else if (hasOpposite && hasHypotenuse)
                {
                    angle = Math.Asin(opposite.Value / hypotenuse.Value);
                    adjacent = Math.Sqrt(hypotenuse.Value * hypotenuse.Value - opposite.Value * opposite.Value);
                    formulas.AppendLine("Used: sin(θ) = opposite/hypotenuse");
                    formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
                }
                else if (hasAdjacent && hasHypotenuse)
                {
                    angle = Math.Acos(adjacent.Value / hypotenuse.Value);
                    opposite = Math.Sqrt(hypotenuse.Value * hypotenuse.Value - adjacent.Value * adjacent.Value);
                    formulas.AppendLine("Used: cos(θ) = adjacent/hypotenuse");
                    formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
                }

                double angleDegrees = angle.Value * (180.0 / Math.PI);
                result.AppendLine("CALCULATED VALUES:");
                result.AppendLine($"Angle: {angleDegrees:F2}° ({angle.Value:F4} rad)");
                result.AppendLine($"Opposite (Rise): {Measurement.FromDecimalInches(opposite.Value).ToFractionString()}");
                result.AppendLine($"Adjacent (Run): {Measurement.FromDecimalInches(adjacent.Value).ToFractionString()}");
                result.AppendLine($"Hypotenuse: {Measurement.FromDecimalInches(hypotenuse.Value).ToFractionString()}");
                result.AppendLine();
                result.Append(formulas.ToString());

                solverResultTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
