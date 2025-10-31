using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class StairCalculatorForm : MaterialForm
    {
        private TextBox totalRiseTextBox;
        private NumericUpDown numberOfStepsNumeric;
        private Label riserHeightLabel;
        private Label complianceLabel;

        public StairCalculatorForm()
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
        }

        private void InitializeComponent()
        {
            this.Text = "Stair Calculator";
            this.ClientSize = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label riseLabel = new Label
            {
                Location = new Point(20, 90),
                Size = new Size(120, 25),
                Text = "Total Rise:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(riseLabel);

            totalRiseTextBox = new TextBox
            {
                Location = new Point(150, 90),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 9' or 108\""
            };
            this.Controls.Add(totalRiseTextBox);

            Label stepsLabel = new Label
            {
                Location = new Point(20, 130),
                Size = new Size(120, 25),
                Text = "Number of Steps:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(stepsLabel);

            numberOfStepsNumeric = new NumericUpDown
            {
                Location = new Point(150, 130),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 1,
                Maximum = 100,
                Value = 15
            };
            this.Controls.Add(numberOfStepsNumeric);

            Button calculateButton = new Button
            {
                Location = new Point(150, 170),
                Size = new Size(220, 35),
                Text = "Calculate",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            calculateButton.Click += Calculate;
            this.Controls.Add(calculateButton);

            riserHeightLabel = new Label
            {
                Location = new Point(20, 220),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(riserHeightLabel);

            complianceLabel = new Label
            {
                Location = new Point(20, 260),
                Size = new Size(360, 40),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(complianceLabel);
        }

        private void Calculate(object? sender, EventArgs e)
        {
            try
            {
                Measurement totalRise = Measurement.Parse(totalRiseTextBox.Text);
                int numberOfSteps = (int)numberOfStepsNumeric.Value;

                double totalRiseInches = totalRise.ToTotalInches();
                double riserHeightInches = totalRiseInches / numberOfSteps;

                Measurement riserHeight = Measurement.FromDecimalInches(riserHeightInches);
                riserHeightLabel.Text = $"Riser Height: {riserHeight.ToFractionString()}";

                if (riserHeightInches >= 7.0 && riserHeightInches <= 7.75)
                {
                    complianceLabel.Text = "✓ Within typical residential code range (7-7.75\")";
                    complianceLabel.ForeColor = Color.Green;
                }
                else if (riserHeightInches >= 6.0 && riserHeightInches <= 8.0)
                {
                    complianceLabel.Text = "⚠ Outside typical range but may be acceptable\nCheck local building codes";
                    complianceLabel.ForeColor = Color.Orange;
                }
                else
                {
                    complianceLabel.Text = "⚠ Outside typical code range\nCheck local building codes";
                    complianceLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
