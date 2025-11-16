using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class SeatingLayoutCalculatorForm : MaterialForm
    {
        private TextBox numberOfRowsTextBox;
        private TextBox startingRadiusTextBox;
        private TextBox rowSpacingTextBox;
        private TextBox arcSpanTextBox;
        private TextBox centerNorthingTextBox;
        private TextBox centerEastingTextBox;
        
        private TextBox chairAWidthTextBox;
        private TextBox chairADepthTextBox;
        private TextBox chairBWidthTextBox;
        private TextBox chairBDepthTextBox;
        
        private TextBox numberOfAislesTextBox;
        private TextBox aisleWidthTextBox;
        private ComboBox aisleSpacingComboBox;
        
        private TextBox minAisleWidthTextBox;
        private TextBox maxSeatsTextBox;
        
        private Label resultLabel;
        private StringBuilder calculationResults;

        public SeatingLayoutCalculatorForm()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            this.Text = "Seating Layout Calculator";
            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            
            calculationResults = new StringBuilder();

            InitializeComponents();
            
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

        private void InitializeComponents()
        {
            int leftX = 20;
            int leftLabelWidth = 170;
            int leftTextBoxX = 200;
            int leftTextBoxWidth = 180;
            
            int rightX = 430;
            int rightLabelWidth = 170;
            int rightTextBoxX = 610;
            int rightTextBoxWidth = 180;
            
            int spacing = 35;
            int yPos = 80;

            Label rowConfigLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(380, 25),
                Text = "Row Configuration",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(rowConfigLabel);
            yPos += spacing;

            Label numberOfRowsLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Number of Rows:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(numberOfRowsLabel);

            numberOfRowsTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "10"
            };
            this.Controls.Add(numberOfRowsTextBox);
            yPos += spacing;

            Label startingRadiusLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Starting Radius (ft):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(startingRadiusLabel);

            startingRadiusTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "20"
            };
            this.Controls.Add(startingRadiusTextBox);
            yPos += spacing;

            Label rowSpacingLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Row Spacing (ft):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(rowSpacingLabel);

            rowSpacingTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "3"
            };
            this.Controls.Add(rowSpacingTextBox);
            yPos += spacing;

            Label arcSpanLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Arc Span (degrees):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(arcSpanLabel);

            arcSpanTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "180"
            };
            this.Controls.Add(arcSpanTextBox);
            yPos += spacing;

            Label centerNorthingLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Center Northing:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(centerNorthingLabel);

            centerNorthingTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "0"
            };
            this.Controls.Add(centerNorthingTextBox);
            yPos += spacing;

            Label centerEastingLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Center Easting:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(centerEastingLabel);

            centerEastingTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "0"
            };
            this.Controls.Add(centerEastingTextBox);

            yPos += spacing + 10;
            Label codeComplianceLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(380, 25),
                Text = "Code Compliance",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(codeComplianceLabel);
            yPos += spacing;

            Label minAisleWidthLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Min Aisle Width (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(minAisleWidthLabel);

            minAisleWidthTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "36"
            };
            this.Controls.Add(minAisleWidthTextBox);
            yPos += spacing;

            Label maxSeatsLabel = new Label
            {
                Location = new Point(leftX, yPos),
                Size = new Size(leftLabelWidth, 25),
                Text = "Max Seats/Aisle:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(maxSeatsLabel);

            maxSeatsTextBox = new TextBox
            {
                Location = new Point(leftTextBoxX, yPos),
                Size = new Size(leftTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "14"
            };
            this.Controls.Add(maxSeatsTextBox);

            yPos = 80;
            Label chairSpecLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(380, 25),
                Text = "Chair Specifications",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(chairSpecLabel);
            yPos += spacing;

            Label chairAWidthLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Type A Width (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(chairAWidthLabel);

            chairAWidthTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "24"
            };
            this.Controls.Add(chairAWidthTextBox);
            yPos += spacing;

            Label chairADepthLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Type A Depth (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(chairADepthLabel);

            chairADepthTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "20"
            };
            this.Controls.Add(chairADepthTextBox);
            yPos += spacing;

            Label chairBWidthLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Type B Width (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(chairBWidthLabel);

            chairBWidthTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "20"
            };
            this.Controls.Add(chairBWidthTextBox);
            yPos += spacing;

            Label chairBDepthLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Type B Depth (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(chairBDepthLabel);

            chairBDepthTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "20"
            };
            this.Controls.Add(chairBDepthTextBox);

            yPos += spacing + 10;
            Label aisleConfigLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(380, 25),
                Text = "Aisle Configuration",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(aisleConfigLabel);
            yPos += spacing;

            Label numberOfAislesLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Number of Aisles:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(numberOfAislesLabel);

            numberOfAislesTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "2"
            };
            this.Controls.Add(numberOfAislesTextBox);
            yPos += spacing;

            Label aisleWidthLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Aisle Width (in):",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(aisleWidthLabel);

            aisleWidthTextBox = new TextBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                Text = "36"
            };
            this.Controls.Add(aisleWidthTextBox);
            yPos += spacing;

            Label aisleSpacingLabel = new Label
            {
                Location = new Point(rightX, yPos),
                Size = new Size(rightLabelWidth, 25),
                Text = "Aisle Spacing:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White
            };
            this.Controls.Add(aisleSpacingLabel);

            aisleSpacingComboBox = new ComboBox
            {
                Location = new Point(rightTextBoxX, yPos),
                Size = new Size(rightTextBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            aisleSpacingComboBox.Items.AddRange(new object[] { "Evenly Spaced", "Custom Positions" });
            aisleSpacingComboBox.SelectedIndex = 0;
            this.Controls.Add(aisleSpacingComboBox);

            yPos = 445;
            Button calculateButton = new Button
            {
                Location = new Point(220, yPos),
                Size = new Size(180, 40),
                Text = "Calculate Layout",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            calculateButton.Click += CalculateLayout;
            this.Controls.Add(calculateButton);

            Button exportButton = new Button
            {
                Location = new Point(420, yPos),
                Size = new Size(180, 40),
                Text = "Export to CSV",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            exportButton.Click += ExportToCSV;
            this.Controls.Add(exportButton);
            yPos += 50;

            resultLabel = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(800, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false
            };
            this.Controls.Add(resultLabel);
        }

        private void CalculateLayout(object? sender, EventArgs e)
        {
            try
            {
                int numberOfRows = int.Parse(numberOfRowsTextBox.Text.Trim());
                double startingRadius = double.Parse(startingRadiusTextBox.Text.Trim());
                double rowSpacing = double.Parse(rowSpacingTextBox.Text.Trim());
                double arcSpan = double.Parse(arcSpanTextBox.Text.Trim());
                double centerNorthing = double.Parse(centerNorthingTextBox.Text.Trim());
                double centerEasting = double.Parse(centerEastingTextBox.Text.Trim());
                
                double chairAWidth = double.Parse(chairAWidthTextBox.Text.Trim()) / 12.0;
                double chairBWidth = double.Parse(chairBWidthTextBox.Text.Trim()) / 12.0;
                
                int numberOfAisles = int.Parse(numberOfAislesTextBox.Text.Trim());
                double aisleWidth = double.Parse(aisleWidthTextBox.Text.Trim()) / 12.0;
                
                double minAisleWidth = double.Parse(minAisleWidthTextBox.Text.Trim()) / 12.0;
                int maxSeatsPerAisle = int.Parse(maxSeatsTextBox.Text.Trim());

                if (aisleWidth < minAisleWidth)
                {
                    MessageBox.Show($"Aisle width ({aisleWidth * 12:F1}\") is less than minimum ({minAisleWidth * 12:F1}\")!", 
                        "Code Compliance Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                calculationResults.Clear();
                calculationResults.AppendLine("Row,Seat,Type,X_Coordinate,Y_Coordinate,Rotation_Angle,Notes");

                int totalSeats = 0;
                double arcSpanRadians = arcSpan * (Math.PI / 180.0);
                double startAngle = -(arcSpanRadians / 2.0);

                for (int row = 1; row <= numberOfRows; row++)
                {
                    double currentRadius = startingRadius + ((row - 1) * rowSpacing);
                    double arcLength = currentRadius * arcSpanRadians;
                    double availableWidth = arcLength - (numberOfAisles * aisleWidth);

                    List<char> chairPattern = GenerateChairPattern(availableWidth, chairAWidth, chairBWidth, numberOfAisles, maxSeatsPerAisle);
                    
                    if (chairPattern.Count == 0)
                    {
                        MessageBox.Show($"Cannot fit any chairs in row {row}. Radius too small or too many aisles.", 
                            "Layout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    double currentAngle = startAngle;
                    int seatNumber = 1;
                    int seatsInSection = 0;

                    foreach (char chairType in chairPattern)
                    {
                        if (chairType == '|')
                        {
                            currentAngle += aisleWidth / currentRadius;
                            seatsInSection = 0;
                            continue;
                        }

                        double chairWidth = (chairType == 'A') ? chairAWidth : chairBWidth;
                        currentAngle += chairWidth / (2.0 * currentRadius);

                        double x = centerEasting + (currentRadius * Math.Cos(currentAngle));
                        double y = centerNorthing + (currentRadius * Math.Sin(currentAngle));
                        double rotationDegrees = (currentAngle * 180.0 / Math.PI) + 90.0;

                        string notes = "";
                        seatsInSection++;

                        calculationResults.AppendLine($"{row},{seatNumber},{chairType},{x:F3},{y:F3},{rotationDegrees:F2},{notes}");
                        
                        currentAngle += chairWidth / (2.0 * currentRadius);
                        seatNumber++;
                        totalSeats++;
                    }
                }

                resultLabel.Text = $"Layout calculated successfully!\n" +
                                  $"Total Seats: {totalSeats}\n" +
                                  $"Rows: {numberOfRows}\n" +
                                  $"Click 'Export to CSV' to save.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<char> GenerateChairPattern(double availableWidth, double chairAWidth, double chairBWidth, 
                                                 int numberOfAisles, int maxSeatsPerAisle)
        {
            List<char> pattern = new List<char>();
            int sections = numberOfAisles + 1;
            double widthPerSection = availableWidth / sections;

            for (int section = 0; section < sections; section++)
            {
                if (section > 0)
                {
                    pattern.Add('|');
                }

                List<char> sectionPattern = new List<char>();
                double remainingWidth = widthPerSection;
                int seatCount = 0;
                bool useTypeA = true;

                sectionPattern.Add('A');
                remainingWidth -= chairAWidth;
                seatCount++;

                while (remainingWidth > 0 && seatCount < maxSeatsPerAisle)
                {
                    double nextWidth = useTypeA ? chairAWidth : chairBWidth;
                    
                    if (remainingWidth < nextWidth)
                    {
                        break;
                    }

                    sectionPattern.Add(useTypeA ? 'A' : 'B');
                    remainingWidth -= nextWidth;
                    seatCount++;
                    useTypeA = !useTypeA;
                }

                if (sectionPattern.Count > 0 && sectionPattern[^1] != 'A')
                {
                    sectionPattern[^1] = 'A';
                }

                pattern.AddRange(sectionPattern);
            }

            return pattern;
        }

        private void ExportToCSV(object? sender, EventArgs e)
        {
            try
            {
                if (calculationResults.Length == 0 || calculationResults.ToString().Split('\n').Length <= 2)
                {
                    MessageBox.Show("Please calculate layout first before exporting.", 
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    FileName = $"SeatingLayout_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, calculationResults.ToString());
                    MessageBox.Show($"Layout exported successfully to:\n{saveFileDialog.FileName}", 
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting file: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
