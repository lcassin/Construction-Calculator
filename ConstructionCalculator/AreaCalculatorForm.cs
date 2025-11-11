using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class AreaCalculatorForm : MaterialForm
    {
        private TextBox lengthTextBox;
        private TextBox widthTextBox;
        private ListBox sectionsListBox;
        private Label totalAreaLabel;
        private readonly List<(Measurement length, Measurement width, double sqft)> sections = [];

        public AreaCalculatorForm()
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
            this.Text = "Area Calculator";
            this.ClientSize = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label instructionLabel = new Label
            {
                Location = new Point(20, 80),
                Size = new Size(460, 40),
                Text = "Calculate total square footage by adding multiple rectangular sections.\nBreak complex floor plans into rectangles.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            this.Controls.Add(instructionLabel);

            Label lengthLabel = new Label
            {
                Location = new Point(20, 130),
                Size = new Size(100, 25),
                Text = "Length:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lengthLabel);

            lengthTextBox = new TextBox
            {
                Location = new Point(130, 130),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 23' 6\""
            };
            this.Controls.Add(lengthTextBox);

            Label widthLabel = new Label
            {
                Location = new Point(20, 165),
                Size = new Size(100, 25),
                Text = "Width:",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(widthLabel);

            widthTextBox = new TextBox
            {
                Location = new Point(130, 165),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., 15' 3\""
            };
            this.Controls.Add(widthTextBox);

            Button addSectionButton = new Button
            {
                Location = new Point(310, 130),
                Size = new Size(170, 60),
                Text = "Add Section",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 100),
                UseVisualStyleBackColor = false
            };
            addSectionButton.Click += AddSection;
            this.Controls.Add(addSectionButton);

            Label sectionsLabel = new Label
            {
                Location = new Point(20, 210),
                Size = new Size(200, 25),
                Text = "Sections:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(sectionsLabel);

            sectionsListBox = new ListBox
            {
                Location = new Point(20, 240),
                Size = new Size(460, 200),
                Font = new Font("Consolas", 9),
                BackColor = Color.White
            };
            this.Controls.Add(sectionsListBox);

            Button removeSectionButton = new Button
            {
                Location = new Point(20, 450),
                Size = new Size(150, 35),
                Text = "Remove Selected",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(220, 220, 220),
                UseVisualStyleBackColor = false
            };
            removeSectionButton.Click += RemoveSection;
            this.Controls.Add(removeSectionButton);

            Button clearAllButton = new Button
            {
                Location = new Point(180, 450),
                Size = new Size(150, 35),
                Text = "Clear All",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(220, 220, 220),
                UseVisualStyleBackColor = false
            };
            clearAllButton.Click += ClearAll;
            this.Controls.Add(clearAllButton);

            Button copyButton = new Button
            {
                Location = new Point(340, 450),
                Size = new Size(140, 35),
                Text = "Copy Total",
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(173, 216, 230),
                UseVisualStyleBackColor = false
            };
            copyButton.Click += CopyTotal;
            this.Controls.Add(copyButton);

            totalAreaLabel = new Label
            {
                Location = new Point(20, 500),
                Size = new Size(460, 60),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Total Area: 0.00 sq ft"
            };
            this.Controls.Add(totalAreaLabel);
        }

        private void AddSection(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lengthTextBox.Text) || string.IsNullOrWhiteSpace(widthTextBox.Text))
                {
                    MessageBox.Show("Please enter both length and width.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Measurement length = Measurement.Parse(lengthTextBox.Text);
                Measurement width = Measurement.Parse(widthTextBox.Text);

                double lengthFeet = length.ToTotalInches() / 12.0;
                double widthFeet = width.ToTotalInches() / 12.0;
                double sqft = lengthFeet * widthFeet;

                sections.Add((length, width, sqft));

                string displayText = $"{length.ToFractionString()} × {width.ToFractionString()} = {sqft:F2} sq ft";
                sectionsListBox.Items.Add(displayText);

                UpdateTotal();

                lengthTextBox.Clear();
                widthTextBox.Clear();
                lengthTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveSection(object? sender, EventArgs e)
        {
            if (sectionsListBox.SelectedIndex >= 0)
            {
                int index = sectionsListBox.SelectedIndex;
                sections.RemoveAt(index);
                sectionsListBox.Items.RemoveAt(index);
                UpdateTotal();
            }
            else
            {
                MessageBox.Show("Please select a section to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ClearAll(object? sender, EventArgs e)
        {
            sections.Clear();
            sectionsListBox.Items.Clear();
            UpdateTotal();
            lengthTextBox.Clear();
            widthTextBox.Clear();
        }

        private void CopyTotal(object? sender, EventArgs e)
        {
            double total = sections.Sum(s => s.sqft);
            System.Windows.Forms.Clipboard.SetText($"{total:F2}");
            MessageBox.Show($"Copied: {total:F2} sq ft", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateTotal()
        {
            double total = sections.Sum(s => s.sqft);
            totalAreaLabel.Text = $"Total Area: {total:F2} sq ft";
        }
    }
}
