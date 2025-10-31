using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public partial class CalculatorForm : MaterialForm
    {
        private TextBox displayTextBox;
        private Label modeLabel;
        private Label chainLabel;
        private bool isDecimalMode = false;
        private string currentOperation = "";
        private Measurement? storedValue = null;
        private bool shouldClearDisplay = false;
		private readonly List<string> calculationChain = [];

        public CalculatorForm()
        {
            InitializeComponent();
            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            
            SetupCalculator();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            MenuStrip menuStrip = new();
            ToolStripMenuItem toolsMenu = new("Tools");
            
            ToolStripMenuItem angleCalcMenuItem = new("Angle Calculator");
            angleCalcMenuItem.Click += (s, e) => ShowAngleCalculator();
            toolsMenu.DropDownItems.Add(angleCalcMenuItem);
            
            ToolStripMenuItem stairCalcMenuItem = new("Stair Calculator");
            stairCalcMenuItem.Click += (s, e) => ShowStairCalculator();
            toolsMenu.DropDownItems.Add(stairCalcMenuItem);
            
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            
            ToolStripMenuItem themeMenu = new("Theme");
            
            ToolStripMenuItem lightThemeMenuItem = new ("Light");
            lightThemeMenuItem.Click += (s, e) => SetTheme(MaterialSkinManager.Themes.LIGHT);
            themeMenu.DropDownItems.Add(lightThemeMenuItem);
            
            ToolStripMenuItem darkThemeMenuItem = new("Dark");
            darkThemeMenuItem.Click += (s, e) => SetTheme(MaterialSkinManager.Themes.DARK);
            themeMenu.DropDownItems.Add(darkThemeMenuItem);
            
            ToolStripMenuItem systemDefaultMenuItem = new("System Default");
            systemDefaultMenuItem.Click += (s, e) => SetSystemDefaultTheme();
            themeMenu.DropDownItems.Add(systemDefaultMenuItem);
            
            toolsMenu.DropDownItems.Add(themeMenu);
            
            menuStrip.Items.Add(toolsMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
            
            this.ClientSize = new Size(400, 685);
            this.Text = "Construction Calculator";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;
            this.KeyPress += CalculatorForm_KeyPress;
            this.KeyDown += CalculatorForm_KeyDown;
            
            this.Activated += (s, e) => BeginInvoke(new Action(ApplyButtonColors));
            this.VisibleChanged += (s, e) => { if (this.Visible) BeginInvoke(new Action(ApplyButtonColors)); };
            
            this.ResumeLayout(false);
        }

        private void SetupCalculator()
        {
            modeLabel = new Label
            {
                Location = new Point(20, 100),
                Size = new Size(360, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Text = "Mode: Fractions (1/16\")",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            this.Controls.Add(modeLabel);

            chainLabel = new Label
            {
                Location = new Point(20, 125),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Text = "",
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                ForeColor = Color.Gray
            };
            this.Controls.Add(chainLabel);

            displayTextBox = new TextBox
            {
                Location = new Point(20, 160),
                Size = new Size(360, 60),
                Font = new Font("Consolas", 24, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                Text = "0\"",
                ReadOnly = false,
                BackColor = Color.White
            };
            displayTextBox.KeyPress += DisplayTextBox_KeyPress;
            displayTextBox.KeyDown += DisplayTextBox_KeyDown;
            this.Controls.Add(displayTextBox);

            string[] buttonLabels =
			[
				"7", "8", "9", "/",
                "4", "5", "6", "*",
                "1", "2", "3", "-",
                "0", ".", "=", "+",
                "C", "CE", "Copy", "Mode"
            ];

            int buttonWidth = 80;
            int buttonHeight = 60;
            int padding = 10;
            int startX = 20;
            int startY = 240;

            for (int i = 0; i < buttonLabels.Length; i++)
            {
                int row = i / 4;
                int col = i % 4;

                Button btn = new()
				{
                    Location = new Point(startX + col * (buttonWidth + padding), startY + row * (buttonHeight + padding)),
                    Size = new Size(buttonWidth, buttonHeight),
                    Text = buttonLabels[i],
                    FlatStyle = FlatStyle.Flat,
                    UseVisualStyleBackColor = false
                };

                btn.Click += Button_Click;
                this.Controls.Add(btn);
            }

            ApplyButtonColors();

            Label instructionLabel = new()
			{
                Location = new Point(20, 600),
                Size = new Size(360, 60),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                Text = "Enter measurements: 23' 6 1/2\" or 6 1/2\" or 1/2\"\nShortcuts: Enter/= (calc), Esc (clear), Ctrl+Z (undo), Ctrl+M (mode)\nCtrl+C (copy), Ctrl++/−/×/÷ (operators)",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            this.Controls.Add(instructionLabel);
            
            displayTextBox.Focus();
			displayTextBox.SelectAll();
		}

        // Update the event handler signature to explicitly allow nullable sender
        private void CalculatorForm_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                if (!string.IsNullOrEmpty(currentOperation))
                {
                    PerformCalculation();
                }
            }
        }

        private void DisplayTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                PerformCalculation();
            }
        }

        private void DisplayTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                PerformCalculation();
            }
        }

        private void CalculatorForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C && !e.Shift && !e.Alt)
            {
                System.Windows.Forms.Clipboard.SetText(displayTextBox.Text);
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add))
            {
                SetOperation("+");
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract))
            {
                SetOperation("-");
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == Keys.Multiply || (e.KeyCode == Keys.D8 && e.Shift)))
            {
                SetOperation("*");
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion))
            {
                SetOperation("/");
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                ToggleMode();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Clear();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                ClearEntry();
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string buttonText = btn.Text;

            try
            {
                if (buttonText == "C")
                {
                    Clear();
                }
                else if (buttonText == "CE")
                {
                    ClearEntry();
                }
                else if (buttonText == "Mode")
                {
                    ToggleMode();
                }
                else if (buttonText == "Copy")
                {
                    System.Windows.Forms.Clipboard.SetText(displayTextBox.Text);
                }
                else if (buttonText == "±")
                {
                    ToggleSign();
                }
                else if (buttonText == "=")
                {
                    PerformCalculation();
                    displayTextBox.Focus();
					displayTextBox.SelectAll();
				}
                else if (buttonText == "+" || buttonText == "-" || buttonText == "*" || buttonText == "/")
                {
                    SetOperation(buttonText);
                }
                else
                {
                    AppendToDisplay(buttonText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
        }

        private void Clear()
        {
            displayTextBox.Text = isDecimalMode ? "0" : "0\"";
            currentOperation = "";
            storedValue = null;
            shouldClearDisplay = false;
            calculationChain.Clear();
            UpdateChainDisplay();
            
            displayTextBox.Focus();
			displayTextBox.SelectAll();
		}

        private void ClearEntry()
        {
            if (calculationChain.Count > 0)
            {
                calculationChain.RemoveAt(calculationChain.Count - 1);
                UpdateChainDisplay();
                
                if (calculationChain.Count > 0)
                {
                    try
                    {
                        ReplayCalculationChain();
                    }
                    catch
                    {
                        displayTextBox.Text = isDecimalMode ? "0" : "0\"";
                        storedValue = null;
                        currentOperation = "";
                    }
                }
                else
                {
                    displayTextBox.Text = isDecimalMode ? "0" : "0\"";
                    storedValue = null;
                    currentOperation = "";
                }
            }
            else
            {
                displayTextBox.Text = isDecimalMode ? "0" : "0\"";
            }
            shouldClearDisplay = false;
        }

        private void UpdateChainDisplay()
        {
            if (calculationChain.Count > 0)
            {
                chainLabel.Text = string.Join(" ", calculationChain);
            }
            else
            {
                chainLabel.Text = "";
            }
        }

        private void ReplayCalculationChain()
        {
            storedValue = null;
            currentOperation = "";
            
            for (int i = 0; i < calculationChain.Count; i++)
            {
                string item = calculationChain[i];
                
                if (item == "+" || item == "-" || item == "*" || item == "/")
                {
                    currentOperation = item;
                }
                else
                {
                    Measurement value = Measurement.Parse(item);
                    
                    if (storedValue == null)
                    {
                        storedValue = value;
                    }
                    else if (!string.IsNullOrEmpty(currentOperation))
                    {
                        switch (currentOperation)
                        {
                            case "+":
                                storedValue += value;
                                break;
                            case "-":
                                storedValue -= value;
                                break;
                            case "*":
                                storedValue *= value.ToTotalInches();
                                break;
                            case "/":
                                storedValue /= value.ToTotalInches();
                                break;
                        }
                        currentOperation = "";
                    }
                }
            }
            
            if (storedValue != null)
            {
                UpdateDisplay(storedValue);
            }
        }

        private void ToggleMode()
        {
            isDecimalMode = !isDecimalMode;
            modeLabel.Text = isDecimalMode ? "Mode: Decimal (inches)" : "Mode: Fractions (1/16\")";
            
            try
            {
                if (!string.IsNullOrEmpty(displayTextBox.Text) && displayTextBox.Text != "0" && displayTextBox.Text != "0\"")
                {
                    Measurement current = ParseCurrentDisplay();
                    UpdateDisplay(current);
                }
                else
                {
                    displayTextBox.Text = isDecimalMode ? "0" : "0\"";
                }
            }
            catch
            {
                displayTextBox.Text = isDecimalMode ? "0" : "0\"";
            }
        }

        private void ToggleSign()
        {
            try
            {
                Measurement current = ParseCurrentDisplay();
                current.Feet = -current.Feet;
                current.Inches = -current.Inches;
                current.Numerator = -current.Numerator;
                UpdateDisplay(current);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

	        private void SetOperation(string operation)
	        {
	            try
	            {
	                bool calculationPerformed = false;
                
	                if (storedValue != null && !string.IsNullOrEmpty(displayTextBox.Text.Trim()) && !string.IsNullOrEmpty(currentOperation))
	                {
	                    PerformCalculation();
	                    calculationPerformed = true;
	                }

	                Measurement currentValue = ParseCurrentDisplay();
	                string displayText = displayTextBox.Text.Trim();
                
	                if (storedValue == null || calculationChain.Count == 0)
	                {
	                    calculationChain.Add(displayText);
	                }
	                else if (!shouldClearDisplay)
	                {
	                    calculationChain.Add(displayText);
	                }
                
	                calculationChain.Add(operation);
	                UpdateChainDisplay();

	                if (!calculationPerformed)
	                {
	                    storedValue = currentValue;
	                }
                
	                currentOperation = operation;
	                shouldClearDisplay = true;
	                displayTextBox.Text = "";
                
	                displayTextBox.Focus();
				displayTextBox.SelectAll();
			}
	            catch (Exception ex)
	            {
	                MessageBox.Show($"Error parsing value: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
	                Clear();
	            }
	        }

        private void PerformCalculation()
        {
            try
            {
                string displayText = displayTextBox.Text.Trim();
                
                if (ContainsOperator(displayText))
                {
                    EvaluateExpression(displayText);
                    return;
                }
                
                if (storedValue == null || string.IsNullOrEmpty(currentOperation))
                    return;

                Measurement current = ParseCurrentDisplay();
                
                if (!shouldClearDisplay && calculationChain.Count > 0 && 
                    calculationChain[^1] != "+" && 
                    calculationChain[^1] != "-" && 
                    calculationChain[^1] != "*" && 
                    calculationChain[^1] != "/")
                {
                    calculationChain.Add(displayText);
                }
                else if (calculationChain.Count > 0 && 
                         (calculationChain[^1] == "+" || 
                          calculationChain[^1] == "-" || 
                          calculationChain[^1] == "*" || 
                          calculationChain[^1] == "/"))
                {
                    calculationChain.Add(displayText);
                }
                
                UpdateChainDisplay();
                
                Measurement result;

                switch (currentOperation)
                {
                    case "+":
                        result = storedValue + current;
                        break;
                    case "-":
                        result = storedValue - current;
                        break;
                    case "*":
                        result = storedValue * current.ToTotalInches();
                        break;
                    case "/":
                        if (current.ToTotalInches() == 0)
                        {
                            MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        result = storedValue / current.ToTotalInches();
                        break;
                    default:
                        return;
                }

                UpdateDisplay(result);
                storedValue = result;
                currentOperation = "";
                shouldClearDisplay = true;
                
                displayTextBox.Focus();
				displayTextBox.SelectAll();
			}
            catch (Exception ex)
            {
                MessageBox.Show($"Calculation error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
        }

        private void AppendToDisplay(string text)
        {
            if (shouldClearDisplay)
            {
                displayTextBox.Text = "";
                shouldClearDisplay = false;
            }

            if (displayTextBox.Text == "0" || displayTextBox.Text == "0\"")
            {
                displayTextBox.Text = text;
            }
            else
            {
                displayTextBox.Text += text;
            }
        }

        private Measurement ParseCurrentDisplay()
        {
            string text = displayTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text) || text == "0" || text == "0\"")
            {
                return new Measurement(0, 0, 0, 16);
            }

            return Measurement.Parse(text);
        }

        private void UpdateDisplay(Measurement measurement)
        {
            displayTextBox.Text = isDecimalMode ? measurement.ToDecimalString() : measurement.ToFractionString();
        }

        private bool ContainsOperator(string text)
        {
            bool hasQuotes = text.Contains("'") || text.Contains("\"");
            
            if (hasQuotes)
            {
                bool lastWasQuote = false;
                
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    
                    if (c == '"' || c == '\'')
                    {
                        lastWasQuote = true;
                        continue;
                    }
                    
                    if (lastWasQuote && (c == '+' || c == '*' || c == '/' || c == '-'))
                    {
                        return true;
                    }
                    
                    if (c != ' ' && c != '\t')
                    {
                        lastWasQuote = false;
                    }
                }
                
                return false;
            }
            else
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    
                    if (c == '+' || c == '*' || c == '-')
                    {
                        return true;
                    }
                    
                    if (c == '/')
                    {
                        if (!IsFractionNotDivision(text, i))
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }
        }
        private bool IsFractionNotDivision(string text, int slashIndex)
        {
            int startIdx = slashIndex - 1;
            while (startIdx >= 0 && char.IsDigit(text[startIdx]))
            {
                startIdx--;
            }
            startIdx++;
            
            int endIdx = slashIndex + 1;
            while (endIdx < text.Length && char.IsDigit(text[endIdx]))
            {
                endIdx++;
            }
            
            if (startIdx >= slashIndex || endIdx <= slashIndex + 1)
            {
                return false;
            }
            
            string beforeSlash = text.Substring(startIdx, slashIndex - startIdx);
            string afterSlash = text.Substring(slashIndex + 1, endIdx - slashIndex - 1);
            
            if (int.TryParse(beforeSlash, out int numerator) && int.TryParse(afterSlash, out int denominator))
            {
                return numerator <= 16 && denominator <= 16;
            }
            
            return false;
        }



        private void EvaluateExpression(string expression)
        {
            try
            {
                bool hasQuotes = expression.Contains("'") || expression.Contains("\"");
                
                var parts = new System.Collections.Generic.List<string>();
                
                if (!hasQuotes)
                {
                    var currentPart = "";
                    
                    for (int i = 0; i < expression.Length; i++)
                    {
                        char c = expression[i];
                        
                        if (c == '+' || c == '*' || c == '-')
                        {
                            if (!string.IsNullOrWhiteSpace(currentPart))
                            {
                                parts.Add(currentPart.Trim());
                                parts.Add(c.ToString());
                                currentPart = "";
                            }
                        }
                        else if (c == '/')
                        {
                            if (IsFractionNotDivision(expression, i))
                            {
                                currentPart += c;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(currentPart))
                                {
                                    parts.Add(currentPart.Trim());
                                    parts.Add("/");
                                    currentPart = "";
                                }
                            }
                        }
                        else
                        {
                            currentPart += c;
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(currentPart))
                    {
                        parts.Add(currentPart.Trim());
                    }
                }
                else
                {
                    var currentPart = "";
                    bool lastWasQuote = false;
                    
                    for (int i = 0; i < expression.Length; i++)
                    {
                        char c = expression[i];
                        
                        if (c == '"' || c == '\'')
                        {
                            currentPart += c;
                            lastWasQuote = true;
                            continue;
                        }
                        
                        if (lastWasQuote && (c == '+' || c == '*' || c == '/' || c == '-'))
                        {
                            if (!string.IsNullOrWhiteSpace(currentPart))
                            {
                                parts.Add(currentPart.Trim());
                                parts.Add(c.ToString());
                                currentPart = "";
                                lastWasQuote = false;
                            }
                        }
                        else
                        {
                            currentPart += c;
                            if (c != ' ' && c != '\t')
                            {
                                lastWasQuote = false;
                            }
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(currentPart))
                    {
                        parts.Add(currentPart.Trim());
                    }
                }
                
                if (parts.Count == 0)
                    return;
                
                Measurement result = Measurement.Parse(parts[0]);
                calculationChain.Clear();
                calculationChain.Add(parts[0]);
                
                for (int i = 1; i < parts.Count; i += 2)
                {
                    if (i + 1 >= parts.Count)
                        break;
                    
                    string op = parts[i];
                    string nextValue = parts[i + 1];
                    
                    calculationChain.Add(op);
                    calculationChain.Add(nextValue);
                    
                    Measurement nextMeasurement = Measurement.Parse(nextValue);
                    
                    switch (op)
                    {
                        case "+":
                            result = result + nextMeasurement;
                            break;
                        case "-":
                            result = result - nextMeasurement;
                            break;
                        case "*":
                            result = result * nextMeasurement.ToTotalInches();
                            break;
                        case "/":
                            if (nextMeasurement.ToTotalInches() == 0)
                            {
                                MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            result = result / nextMeasurement.ToTotalInches();
                            break;
                    }
                }
                
                UpdateChainDisplay();
                UpdateDisplay(result);
                storedValue = result;
                currentOperation = "";
                shouldClearDisplay = true;
                
                displayTextBox.Focus();
				displayTextBox.SelectAll();
			}
            catch (Exception)
            {
                //MessageBox.Show($"Error evaluating expression: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearEntry();
            }
        }

        private void ShowAngleCalculator()
        {
            using (var angleCalc = new AngleCalculatorForm())
            {
                angleCalc.ShowDialog(this);
				BeginInvoke(new Action(ApplyButtonColors));
			}
        }

        private void ShowStairCalculator()
        {
            using (var stairCalc = new StairCalculatorForm())
            {
                stairCalc.ShowDialog(this);
				BeginInvoke(new Action(ApplyButtonColors));
			}
        }

        private void SetTheme(MaterialSkinManager.Themes theme)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = theme;
            
            if (theme == MaterialSkinManager.Themes.DARK)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            }
            else
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            }
            
            this.Invalidate(true);
            this.Refresh();
            
            BeginInvoke(new Action(ApplyButtonColors));
        }

        private void SetSystemDefaultTheme()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var appsUseLightTheme = key.GetValue("AppsUseLightTheme");
                        if (appsUseLightTheme != null && (int)appsUseLightTheme == 0)
                        {
                            SetTheme(MaterialSkinManager.Themes.DARK);
                        }
                        else
                        {
                            SetTheme(MaterialSkinManager.Themes.LIGHT);
                        }
                    }
                    else
                    {
                        SetTheme(MaterialSkinManager.Themes.LIGHT);
                    }
                }
            }
            catch
            {
                SetTheme(MaterialSkinManager.Themes.LIGHT);
            }
        }

        private void ApplyButtonColors()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    if (btn.Text == "=" || btn.Text == "+" || btn.Text == "-" || 
                        btn.Text == "*" || btn.Text == "/")
                    {
                        btn.BackColor = Color.FromArgb(255, 200, 100);
                        btn.UseVisualStyleBackColor = false;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 200, 100);
                        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 180, 80);
                        btn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    }
                    else if (btn.Text == "C" || btn.Text == "CE" || btn.Text == "Copy" || btn.Text == "Mode")
                    {
                        btn.BackColor = Color.FromArgb(220, 220, 220);
                        btn.UseVisualStyleBackColor = false;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(220, 220, 220);
                        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 200, 200);
                        btn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    }
                    else if (btn.Text.Length <= 2)
                    {
                        btn.BackColor = Color.FromArgb(173, 216, 230);
                        btn.UseVisualStyleBackColor = false;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(173, 216, 230);
                        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(153, 206, 220);
                        btn.Font = new Font("Segoe UI", 14, FontStyle.Regular);
                    }
                }
            }
        }
    }
}
