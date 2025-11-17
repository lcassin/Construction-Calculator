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
        private Label runWidthLabel;
        private CheckBox includeLandingCheckBox;
        private Label landingDepthLabel;
        private TextBox landingDepthTextBox;
        private Label stepsBeforeLandingLabel;
        private NumericUpDown stepsBeforeLandingNumeric;
        private Label stepsAfterLandingLabel;
        private Label totalRunWithLandingLabel;

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
            this.Text = "Stair Calculator";
            this.ClientSize = new Size(400, 860);
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
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter
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
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter
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

            runWidthLabel = new Label
            {
                Location = new Point(20, 300),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(runWidthLabel);

            complianceLabel = new Label
            {
                Location = new Point(20, 340),
                Size = new Size(360, 50),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(complianceLabel);

            includeLandingCheckBox = new CheckBox
            {
                Location = new Point(20, 400),
                Size = new Size(150, 25),
                Text = "Include Landing",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Checked = false
            };
            includeLandingCheckBox.CheckedChanged += IncludeLandingCheckBox_CheckedChanged;
            this.Controls.Add(includeLandingCheckBox);

            landingDepthLabel = new Label
            {
                Location = new Point(20, 435),
                Size = new Size(120, 25),
                Text = "Landing Depth:",
                Font = new Font("Segoe UI", 10),
                Visible = false
            };
            this.Controls.Add(landingDepthLabel);

            landingDepthTextBox = new TextBox
            {
                Location = new Point(150, 435),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 3' or 36\" (min 36\")",
                Visible = false
            };
            landingDepthTextBox.TextChanged += (s, e) => { if (includeLandingCheckBox.Checked) Calculate(s, e); };
            this.Controls.Add(landingDepthTextBox);

            stepsBeforeLandingLabel = new Label
            {
                Location = new Point(20, 475),
                Size = new Size(140, 25),
                Text = "Steps Before Landing:",
                Font = new Font("Segoe UI", 10),
                Visible = false
            };
            this.Controls.Add(stepsBeforeLandingLabel);

            stepsBeforeLandingNumeric = new NumericUpDown
            {
                Location = new Point(170, 475),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 1,
                Maximum = 50,
                Value = 8,
                Visible = false
            };
            stepsBeforeLandingNumeric.ValueChanged += (s, e) => { if (includeLandingCheckBox.Checked) Calculate(s, e); };
            this.Controls.Add(stepsBeforeLandingNumeric);

            stepsAfterLandingLabel = new Label
            {
                Location = new Point(20, 515),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            this.Controls.Add(stepsAfterLandingLabel);

            totalRunWithLandingLabel = new Label
            {
                Location = new Point(20, 555),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            this.Controls.Add(totalRunWithLandingLabel);

            Label diagramLabel = new Label
            {
                Location = new Point(20, 600),
                Size = new Size(360, 25),
                Text = "Visual Stair Profile:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(diagramLabel);

            visualDiagramTextBox = new TextBox
            {
                Location = new Point(20, 630),
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

        private void IncludeLandingCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool showLanding = includeLandingCheckBox.Checked;
            landingDepthLabel.Visible = showLanding;
            landingDepthTextBox.Visible = showLanding;
            stepsBeforeLandingLabel.Visible = showLanding;
            stepsBeforeLandingNumeric.Visible = showLanding;
            stepsAfterLandingLabel.Visible = showLanding;
            totalRunWithLandingLabel.Visible = showLanding;

            if (showLanding && !string.IsNullOrWhiteSpace(totalRiseTextBox.Text))
            {
                Calculate(sender, e);
            }
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

                double totalRunInches = (numberOfSteps - 1) * treadDepthInches;
                Measurement totalRun = Measurement.FromDecimalInches(totalRunInches);
                runWidthLabel.Text = $"Total Run Width: {totalRun.ToFractionString()}";

                if (includeLandingCheckBox.Checked)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(landingDepthTextBox.Text))
                        {
                            stepsAfterLandingLabel.Text = "Enter landing depth";
                            totalRunWithLandingLabel.Text = "";
                        }
                        else
                        {
                            Measurement landingDepth = Measurement.Parse(landingDepthTextBox.Text);
                            double landingDepthInches = landingDepth.ToTotalInches();

                            if (landingDepthInches < 36.0)
                            {
                                MessageBox.Show("Landing depth should be at least 36\" per typical building codes.", 
                                    "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            int stepsBeforeLanding = (int)stepsBeforeLandingNumeric.Value;
                            if (stepsBeforeLanding >= numberOfSteps)
                            {
                                stepsAfterLandingLabel.Text = "Steps before landing must be less than total steps";
                                totalRunWithLandingLabel.Text = "";
                            }
                            else
                            {
                                int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;
                                double runBeforeLanding = (stepsBeforeLanding - 1) * treadDepthInches;
                                double runAfterLanding = (stepsAfterLanding - 1) * treadDepthInches;
                                double totalRunWithLanding = runBeforeLanding + landingDepthInches + runAfterLanding;

                                stepsAfterLandingLabel.Text = $"Steps After Landing: {stepsAfterLanding}";
                                Measurement totalRunWithLandingMeasurement = Measurement.FromDecimalInches(totalRunWithLanding);
                                totalRunWithLandingLabel.Text = $"Total Run with Landing: {totalRunWithLandingMeasurement.ToFractionString()}";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        stepsAfterLandingLabel.Text = "Invalid landing depth";
                        totalRunWithLandingLabel.Text = "";
                    }
                }

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
            
            if (includeLandingCheckBox.Checked && !string.IsNullOrWhiteSpace(landingDepthTextBox.Text))
            {
                try
                {
                    Measurement landingDepth = Measurement.Parse(landingDepthTextBox.Text);
                    int stepsBeforeLanding = (int)stepsBeforeLandingNumeric.Value;
                    int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;
                    
                    diagram.AppendLine();
                    diagram.AppendLine("WITH LANDING:");
                    diagram.AppendLine($"Steps Before Landing: {stepsBeforeLanding}");
                    diagram.AppendLine($"Landing Depth: {landingDepth.ToFractionString()}");
                    diagram.AppendLine($"Steps After Landing: {stepsAfterLanding}");
                }
                catch
                {
                }
            }
            
            diagram.AppendLine();
            diagram.AppendLine("Stair Profile:");
            diagram.AppendLine();

            if (includeLandingCheckBox.Checked && !string.IsNullOrWhiteSpace(landingDepthTextBox.Text))
            {
                try
                {
                    int stepsBeforeLanding = (int)stepsBeforeLandingNumeric.Value;
                    int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;
                    
                    int stepsToShowBefore = Math.Min(3, stepsBeforeLanding);
                    for (int i = 0; i < stepsToShowBefore; i++)
                    {
                        diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                        if (i < stepsToShowBefore - 1)
                        {
                            diagram.AppendLine($"└──────────┐");
                        }
                        else
                        {
                            diagram.AppendLine($"└──────────");
                        }
                    }
                    
                    if (stepsBeforeLanding > stepsToShowBefore)
                    {
                        diagram.AppendLine($"  (... {stepsBeforeLanding - stepsToShowBefore} more steps)");
                    }
                    
                    diagram.AppendLine();
                    diagram.AppendLine($"  ═══════════════════ LANDING ═══════════════════");
                    diagram.AppendLine();
                    
                    int stepsToShowAfter = Math.Min(3, stepsAfterLanding);
                    for (int i = 0; i < stepsToShowAfter; i++)
                    {
                        diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                        if (i < stepsToShowAfter - 1)
                        {
                            diagram.AppendLine($"└──────────┐");
                        }
                        else
                        {
                            diagram.AppendLine($"└──────────");
                        }
                    }
                    
                    if (stepsAfterLanding > stepsToShowAfter)
                    {
                        diagram.AppendLine($"  (... {stepsAfterLanding - stepsToShowAfter} more steps)");
                    }
                }
                catch
                {
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
                    if (numberOfSteps > stepsToShow)
                    {
                        diagram.AppendLine();
                        diagram.AppendLine($"(showing {stepsToShow} of {numberOfSteps} total steps)");
                    }
                }
            }
            else
            {
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
            }

            visualDiagramTextBox.Text = diagram.ToString();
        }
    }
}
