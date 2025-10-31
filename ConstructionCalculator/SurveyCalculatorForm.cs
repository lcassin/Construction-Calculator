using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class SurveyCalculatorForm : MaterialForm
    {
        private TextBox bearingTextBox;
        private TextBox azimuthTextBox;
        private TextBox conversionResultLabel;
        
        private TextBox startNorthingTextBox;
        private TextBox startEastingTextBox;
        private TextBox distanceTextBox;
        private TextBox directionTextBox;
        private TextBox coordinateResultLabel;
        
        private TextBox? focusedTextBox;

        public SurveyCalculatorForm()
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
            this.Text = "Survey Calculator";
            this.ClientSize = new Size(450, 620);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label symbolLabel = new Label
            {
                Location = new Point(20, 78),
                Size = new Size(100, 20),
                Text = "Insert Symbol:",
                Font = new Font("Segoe UI", 9)
            };
            this.Controls.Add(symbolLabel);

            Button degreeButton = new Button
            {
                Location = new Point(130, 75),
                Size = new Size(30, 25),
                Text = "°",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false,
                TabStop = false
            };
            degreeButton.Click += (s, e) => InsertSymbol("°");
            this.Controls.Add(degreeButton);

            Button minuteButton = new Button
            {
                Location = new Point(165, 75),
                Size = new Size(30, 25),
                Text = "'",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false,
                TabStop = false
            };
            minuteButton.Click += (s, e) => InsertSymbol("'");
            this.Controls.Add(minuteButton);

            Button secondButton = new Button
            {
                Location = new Point(200, 75),
                Size = new Size(30, 25),
                Text = "\"",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false,
                TabStop = false
            };
            secondButton.Click += (s, e) => InsertSymbol("\"");
            this.Controls.Add(secondButton);

            Label section1Label = new Label
            {
                Location = new Point(20, 110),
                Size = new Size(410, 25),
                Text = "Bearing / Azimuth Converter",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(section1Label);

            Label bearingLabel = new Label
            {
                Location = new Point(20, 145),
                Size = new Size(100, 25),
                Text = "Bearing:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(bearingLabel);

            bearingTextBox = new TextBox
            {
                Location = new Point(130, 145),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., N 45° 30' E or N45°30'E"
            };
            bearingTextBox.Enter += (s, e) => focusedTextBox = bearingTextBox;
            this.Controls.Add(bearingTextBox);

            Label azimuthLabel = new Label
            {
                Location = new Point(20, 185),
                Size = new Size(100, 25),
                Text = "Azimuth:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(azimuthLabel);

            azimuthTextBox = new TextBox
            {
                Location = new Point(130, 185),
                Size = new Size(290, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 045° 30' or 45.5"
            };
            azimuthTextBox.Enter += (s, e) => focusedTextBox = azimuthTextBox;
            this.Controls.Add(azimuthTextBox);

            Button bearingToAzimuthButton = new Button
            {
                Location = new Point(130, 225),
                Size = new Size(135, 30),
                Text = "Bearing → Azimuth",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            bearingToAzimuthButton.Click += ConvertBearingToAzimuth;
            this.Controls.Add(bearingToAzimuthButton);

            Button azimuthToBearingButton = new Button
            {
                Location = new Point(275, 225),
                Size = new Size(145, 30),
                Text = "Azimuth → Bearing",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            azimuthToBearingButton.Click += ConvertAzimuthToBearing;
            this.Controls.Add(azimuthToBearingButton);

            conversionResultLabel = new TextBox
            {
                Location = new Point(20, 265),
                Size = new Size(410, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Multiline = true,
                TabStop = false
            };
            this.Controls.Add(conversionResultLabel);

            Label separatorLabel = new Label
            {
                Location = new Point(20, 320),
                Size = new Size(410, 2),
                BackColor = Color.FromArgb(200, 200, 200)
            };
            this.Controls.Add(separatorLabel);

            Label section2Label = new Label
            {
                Location = new Point(20, 335),
                Size = new Size(410, 25),
                Text = "Coordinate Geometry",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(section2Label);

            Label startNorthingLabel = new Label
            {
                Location = new Point(20, 370),
                Size = new Size(130, 25),
                Text = "Start Northing:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(startNorthingLabel);

            startNorthingTextBox = new TextBox
            {
                Location = new Point(160, 370),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 5000.00"
            };
            startNorthingTextBox.Enter += (s, e) => focusedTextBox = startNorthingTextBox;
            this.Controls.Add(startNorthingTextBox);

            Label startEastingLabel = new Label
            {
                Location = new Point(20, 405),
                Size = new Size(130, 25),
                Text = "Start Easting:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(startEastingLabel);

            startEastingTextBox = new TextBox
            {
                Location = new Point(160, 405),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 2000.00"
            };
            startEastingTextBox.Enter += (s, e) => focusedTextBox = startEastingTextBox;
            this.Controls.Add(startEastingTextBox);

            Label distanceLabel = new Label
            {
                Location = new Point(20, 440),
                Size = new Size(130, 25),
                Text = "Distance:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(distanceLabel);

            distanceTextBox = new TextBox
            {
                Location = new Point(160, 440),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 100' or 100.5 (feet)"
            };
            distanceTextBox.Enter += (s, e) => focusedTextBox = distanceTextBox;
            this.Controls.Add(distanceTextBox);

            Label directionLabel = new Label
            {
                Location = new Point(20, 475),
                Size = new Size(130, 25),
                Text = "Azimuth/Bearing:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(directionLabel);

            directionTextBox = new TextBox
            {
                Location = new Point(160, 475),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 045° or N45°E"
            };
            directionTextBox.Enter += (s, e) => focusedTextBox = directionTextBox;
            this.Controls.Add(directionTextBox);

            Button calculatePointButton = new Button
            {
                Location = new Point(160, 515),
                Size = new Size(260, 35),
                Text = "Calculate End Point",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            calculatePointButton.Click += CalculateEndPoint;
            this.Controls.Add(calculatePointButton);

            coordinateResultLabel = new TextBox
            {
                Location = new Point(20, 565),
                Size = new Size(410, 60),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                Multiline = true,
                TabStop = false
            };
            this.Controls.Add(coordinateResultLabel);
        }

        private void InsertSymbol(string symbol)
        {
            if (focusedTextBox == null)
            {
                MessageBox.Show("Please click in a text field first to position the cursor.", "No Field Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cursorPosition = focusedTextBox.SelectionStart;
            string currentText = focusedTextBox.Text;
            
            focusedTextBox.Text = currentText.Insert(cursorPosition, symbol);
            
            focusedTextBox.SelectionStart = cursorPosition + symbol.Length;
            focusedTextBox.Focus();
        }

        private void ConvertBearingToAzimuth(object? sender, EventArgs e)
        {
            try
            {
                string bearingStr = bearingTextBox.Text.Trim().ToUpper();
                double azimuth = ParseBearingToAzimuth(bearingStr);
                
                int degrees = (int)azimuth;
                double minutesDecimal = (azimuth - degrees) * 60;
                int minutes = (int)minutesDecimal;
                double seconds = (minutesDecimal - minutes) * 60;
                
                conversionResultLabel.Text = $"Azimuth: {degrees:D3}° {minutes:D2}' {seconds:F1}\"\n({azimuth:F4}°)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConvertAzimuthToBearing(object? sender, EventArgs e)
        {
            try
            {
                double azimuth = ParseAzimuth(azimuthTextBox.Text.Trim());
                string bearing = AzimuthToBearing(azimuth);
                
                conversionResultLabel.Text = $"Bearing: {bearing}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateEndPoint(object? sender, EventArgs e)
        {
            try
            {
                double startNorthing = double.Parse(startNorthingTextBox.Text.Trim());
                double startEasting = double.Parse(startEastingTextBox.Text.Trim());
                
                double distanceFeet;
                string distanceStr = distanceTextBox.Text.Trim();
                if (distanceStr.Contains("'") || distanceStr.Contains("\""))
                {
                    Measurement distMeasurement = Measurement.Parse(distanceStr);
                    distanceFeet = distMeasurement.ToTotalInches() / 12.0;
                }
                else
                {
                    distanceFeet = double.Parse(distanceStr);
                }
                
                string directionStr = directionTextBox.Text.Trim();
                double azimuth;
                if (directionStr.ToUpper().Contains("N") || directionStr.ToUpper().Contains("S") || 
                    directionStr.ToUpper().Contains("E") || directionStr.ToUpper().Contains("W"))
                {
                    azimuth = ParseBearingToAzimuth(directionStr.ToUpper());
                }
                else
                {
                    azimuth = ParseAzimuth(directionStr);
                }
                
                double azimuthRadians = azimuth * (Math.PI / 180.0);
                
                double deltaNorthing = distanceFeet * Math.Cos(azimuthRadians);
                double deltaEasting = distanceFeet * Math.Sin(azimuthRadians);
                
                double endNorthing = startNorthing + deltaNorthing;
                double endEasting = startEasting + deltaEasting;
                
                coordinateResultLabel.Text = $"End Point:\nNorthing: {endNorthing:F2}\nEasting: {endEasting:F2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double ParseBearingToAzimuth(string bearing)
        {
            char firstDir = bearing[0];
            char lastDir = bearing[^1];
            
            if ((firstDir != 'N' && firstDir != 'S') || (lastDir != 'E' && lastDir != 'W'))
            {
                throw new FormatException("Bearing must be in format like N45E, S30W, etc.");
            }
            
            string numberPart = bearing[1..^1];
            numberPart = numberPart.Replace("°", " ").Replace("'", " ").Replace("\"", " ").Trim();
            
            string[] parts = numberPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            double degrees = double.Parse(parts[0]);
            double minutes = 0;
            double seconds = 0;
            
            if (parts.Length > 1)
            {
                minutes = double.Parse(parts[1]);
            }
            if (parts.Length > 2)
            {
                seconds = double.Parse(parts[2]);
            }
            
            double angle = degrees + (minutes / 60.0) + (seconds / 3600.0);
            
            if (angle < 0 || angle > 90)
            {
                throw new ArgumentException("Bearing angle must be between 0 and 90 degrees");
            }
            
            double azimuth;
            if (firstDir == 'N' && lastDir == 'E')
            {
                azimuth = angle;
            }
            else if (firstDir == 'S' && lastDir == 'E')
            {
                azimuth = 180 - angle;
            }
            else if (firstDir == 'S' && lastDir == 'W')
            {
                azimuth = 180 + angle;
            }
            else
            {
                azimuth = 360 - angle;
            }
            
            return azimuth;
        }

        private string AzimuthToBearing(double azimuth)
        {
            azimuth = azimuth % 360;
            if (azimuth < 0) azimuth += 360;
            
            int degrees;
            int minutes;
            double seconds;
            string quadrant;
            double bearingAngle;
            
            if (azimuth >= 0 && azimuth <= 90)
            {
                quadrant = "N{0}E";
                bearingAngle = azimuth;
            }
            else if (azimuth > 90 && azimuth <= 180)
            {
                quadrant = "S{0}E";
                bearingAngle = 180 - azimuth;
            }
            else if (azimuth > 180 && azimuth <= 270)
            {
                quadrant = "S{0}W";
                bearingAngle = azimuth - 180;
            }
            else
            {
                quadrant = "N{0}W";
                bearingAngle = 360 - azimuth;
            }
            
            degrees = (int)bearingAngle;
            double minutesDecimal = (bearingAngle - degrees) * 60;
            minutes = (int)minutesDecimal;
            seconds = (minutesDecimal - minutes) * 60;
            
            string angleStr = $"{degrees:D2}° {minutes:D2}' {seconds:F1}\"";
            return string.Format(quadrant, angleStr);
        }

        private double ParseAzimuth(string azimuth)
        {
            azimuth = azimuth.Replace("°", "").Replace("'", "").Replace("\"", "").Trim();
            
            string[] parts = azimuth.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            double degrees = double.Parse(parts[0]);
            double minutes = 0;
            double seconds = 0;
            
            if (parts.Length > 1)
            {
                minutes = double.Parse(parts[1]);
            }
            if (parts.Length > 2)
            {
                seconds = double.Parse(parts[2]);
            }
            
            double result = degrees + (minutes / 60.0) + (seconds / 3600.0);
            
            if (result < 0 || result >= 360)
            {
                throw new ArgumentException("Azimuth must be between 0 and 360 degrees");
            }
            
            return result;
        }
    }
}
