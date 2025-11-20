using System.Windows;

namespace ConstructionCalculator.WPF.Calculators.Geometry.Area;

public partial class AreaCalculatorWindow : Window
{
    private readonly List<(Measurement length, Measurement width, double sqft)> sections = new();

    public AreaCalculatorWindow()
    {
        InitializeComponent();
    }

    private void AddSectionButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LengthTextBox.Text) || string.IsNullOrWhiteSpace(WidthTextBox.Text))
            {
                MessageBox.Show("Please enter both length and width.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Measurement length = Measurement.Parse(LengthTextBox.Text);
            Measurement width = Measurement.Parse(WidthTextBox.Text);

            double lengthFeet = length.ToTotalInches() / 12.0;
            double widthFeet = width.ToTotalInches() / 12.0;
            double sqft = lengthFeet * widthFeet;

            sections.Add((length, width, sqft));

            string displayText = $"{length.ToFractionString()} × {width.ToFractionString()} = {sqft:F2} sq ft";
            SectionsListBox.Items.Add(displayText);

            UpdateTotal();

            LengthTextBox.Clear();
            WidthTextBox.Clear();
            LengthTextBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RemoveSectionButton_Click(object sender, RoutedEventArgs e)
    {
        if (SectionsListBox.SelectedIndex >= 0)
        {
            int index = SectionsListBox.SelectedIndex;
            sections.RemoveAt(index);
            SectionsListBox.Items.RemoveAt(index);
            UpdateTotal();
        }
        else
        {
            MessageBox.Show("Please select a section to remove.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearAllButton_Click(object sender, RoutedEventArgs e)
    {
        sections.Clear();
        SectionsListBox.Items.Clear();
        UpdateTotal();
        LengthTextBox.Clear();
        WidthTextBox.Clear();
    }

    private void CopyTotalButton_Click(object sender, RoutedEventArgs e)
    {
        double total = sections.Sum(s => s.sqft);
        Clipboard.SetText($"{total:F2}");
        MessageBox.Show($"Copied: {total:F2} sq ft", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void UpdateTotal()
    {
        double total = sections.Sum(s => s.sqft);
        TotalAreaLabel.Text = $"Total Area: {total:F2} sq ft";
    }
}
