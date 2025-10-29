using System;
using System.Drawing;
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
        private Label resultLabel;
        private bool isRadianMode = false;

        public AngleCalculatorForm()
        {
            InitializeComponent();
            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Angle Calculator";
            this.ClientSize = new Size(400, 370);
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

            resultLabel = new Label
            {
                Location = new Point(20, 300),
                Size = new Size(360, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(resultLabel);
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
    }
}
