using System;
using System.Drawing;
using System.Text;
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
        private Label treadDepthLabel;
        private TextBox visualDiagramTextBox;
        private Button autoCalculateButton;

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
            this.ClientSize = new Size(400, 650);
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

            autoCalculateButton = new Button
            {
                Location = new Point(20, 170),
                Size = new Size(120, 35),
                Text = "Auto-Calculate Steps",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            autoCalculateButton.Click += AutoCalculateSteps;
            this.Controls.Add(autoCalculateButton);

            Button calculateButton = new Button
            {
                Location = new Point(250, 170),
                Size = new Size(120, 35),
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

            treadDepthLabel = new Label
            {
                Location = new Point(20, 260),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(treadDepthLabel);

            complianceLabel = new Label
            {
                Location = new Point(20, 300),
                Size = new Size(360, 60),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(complianceLabel);

            Label diagramLabel = new Label
            {
                Location = new Point(20, 370),
                Size = new Size(360, 25),
                Text = "Visual Stair Profile:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(diagramLabel);

            visualDiagramTextBox = new TextBox
            {
                Location = new Point(20, 400),
                Size = new Size(360, 220),
                Font = new Font("Consolas", 9),
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical,
                TabStop = false
            };
            this.Controls.Add(visualDiagramTextBox);
        }

        private void AutoCalculateSteps(object? sender, EventArgs e)
        {
            try
            {
                Measurement totalRise = Measurement.Parse(totalRiseTextBox.Text);
                double totalRiseInches = totalRise.ToTotalInches();

                double targetRiserHeight = 7.375;
                int optimalSteps = (int)Math.Round(totalRiseInches / targetRiserHeight);

                if (optimalSteps < 2) optimalSteps = 2;

                double resultingRiserHeight = totalRiseInches / optimalSteps;

                if (resultingRiserHeight > 7.75 && totalRiseInches > 15.5)
                {
                    optimalSteps++;
                }
                else if (resultingRiserHeight < 7.0 && optimalSteps > 2)
                {
                    optimalSteps--;
                }

                numberOfStepsNumeric.Value = optimalSteps;

                Calculate(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                double treadDepthInches = 25.0 - (2.0 * riserHeightInches);
                Measurement treadDepth = Measurement.FromDecimalInches(treadDepthInches);
                treadDepthLabel.Text = $"Tread Depth: {treadDepth.ToFractionString()}";

                StringBuilder complianceMessage = new StringBuilder();
                bool riserCompliant = false;
                bool treadCompliant = false;

                if (riserHeightInches >= 7.0 && riserHeightInches <= 7.75)
                {
                    complianceMessage.AppendLine("✓ Riser height within typical residential code (7-7.75\")");
                    riserCompliant = true;
                }
                else if (riserHeightInches >= 6.0 && riserHeightInches <= 8.0)
                {
                    complianceMessage.AppendLine("⚠ Riser height outside typical range but may be acceptable");
                }
                else
                {
                    complianceMessage.AppendLine("⚠ Riser height outside typical code range");
                }

                if (treadDepthInches >= 10.0 && treadDepthInches <= 11.0)
                {
                    complianceMessage.AppendLine("✓ Tread depth within typical code (10-11\")");
                    treadCompliant = true;
                }
                else if (treadDepthInches >= 9.0 && treadDepthInches <= 12.0)
                {
                    complianceMessage.AppendLine("⚠ Tread depth outside typical range but may be acceptable");
                }
                else
                {
                    complianceMessage.AppendLine("⚠ Tread depth outside typical code range");
                }

                if (!riserCompliant || !treadCompliant)
                {
                    complianceMessage.AppendLine("Check local building codes");
                }

                complianceLabel.Text = complianceMessage.ToString().Trim();

                if (riserCompliant && treadCompliant)
                {
                    complianceLabel.ForeColor = Color.Green;
                }
                else if ((riserHeightInches >= 6.0 && riserHeightInches <= 8.0) &&
                         (treadDepthInches >= 9.0 && treadDepthInches <= 12.0))
                {
                    complianceLabel.ForeColor = Color.Orange;
                }
                else
                {
                    complianceLabel.ForeColor = Color.Red;
                }

                GenerateVisualDiagram(totalRise, numberOfSteps, riserHeight, treadDepth);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateVisualDiagram(Measurement totalRise, int numberOfSteps, Measurement riserHeight, Measurement treadDepth)
        {
            StringBuilder diagram = new StringBuilder();
            
            diagram.AppendLine($"Total Rise: {totalRise.ToFractionString()}");
            diagram.AppendLine($"Number of Steps: {numberOfSteps}");
            diagram.AppendLine($"Riser Height: {riserHeight.ToFractionString()} ({riserHeight.ToTotalInches():F2}\")");
            diagram.AppendLine($"Tread Depth: {treadDepth.ToFractionString()} ({treadDepth.ToTotalInches():F2}\")");
            diagram.AppendLine();
            diagram.AppendLine("Stair Profile (showing first 4 steps):");
            diagram.AppendLine();

            int stepsToShow = Math.Min(4, numberOfSteps);
            
            for (int i = 0; i < stepsToShow; i++)
            {
                diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                
                if (i < stepsToShow - 1)
                {
                    diagram.AppendLine($"└──────────┐");
                }
                else
                {
                    diagram.AppendLine($"└──────────");
                }
            }
            
            diagram.AppendLine($"  {treadDepth.ToFractionString()}");
            
            if (numberOfSteps > stepsToShow)
            {
                diagram.AppendLine();
                diagram.AppendLine($"(showing {stepsToShow} of {numberOfSteps} total steps)");
            }

            visualDiagramTextBox.Text = diagram.ToString();
        }
    }
}
