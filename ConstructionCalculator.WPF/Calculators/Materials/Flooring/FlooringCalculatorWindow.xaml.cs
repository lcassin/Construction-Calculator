using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Materials.Flooring;

public partial class FlooringCalculatorWindow : Window
{
    private readonly List<SectionData> sections = new();
    private bool _isInitialized = false;

    public FlooringCalculatorWindow()
    {
        InitializeComponent();
        Loaded += (_, __) => 
        { 
            _isInitialized = true; 
            UpdateTotals(); 
        };
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

            if (!double.TryParse(WasteFactorTextBox.Text, out double wasteFactor) || wasteFactor < 0 || wasteFactor > 100)
            {
                MessageBox.Show("Please enter a valid waste factor between 0 and 100.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Measurement length = Measurement.Parse(LengthTextBox.Text);
            Measurement width = Measurement.Parse(WidthTextBox.Text);

            double lengthFeet = length.ToTotalInches() / 12.0;
            double widthFeet = width.ToTotalInches() / 12.0;
            double sqft = lengthFeet * widthFeet;
            double sqftWithWaste = sqft * (1 + wasteFactor / 100.0);

            string materialType = ((ComboBoxItem)MaterialTypeComboBox.SelectedItem).Content.ToString() ?? "Unknown";

            var section = new SectionData
            {
                Length = length,
                Width = width,
                MaterialType = materialType,
                WasteFactor = wasteFactor,
                AreaSqFt = sqft,
                AreaWithWasteSqFt = sqftWithWaste
            };

            sections.Add(section);

            string displayText = $"{length.ToFractionString()} × {width.ToFractionString()} | {materialType} | {sqft:F2} sq ft (+{wasteFactor}% = {sqftWithWaste:F2} sq ft)";
            SectionsListBox.Items.Add(displayText);

            UpdateTotals();

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
            UpdateTotals();
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
        UpdateTotals();
        LengthTextBox.Clear();
        WidthTextBox.Clear();
        CostPerUnitTextBox.Text = "0";
    }

    private void CopyResultsButton_Click(object sender, RoutedEventArgs e)
    {
        double totalArea = sections.Sum(s => s.AreaSqFt);
        double totalWithWaste = sections.Sum(s => s.AreaWithWasteSqFt);
        
        if (double.TryParse(CostPerUnitTextBox.Text, out double costPerUnit))
        {
            double totalCost = totalWithWaste * costPerUnit;
            string result = $"Total Area: {totalArea:F2} sq ft\nMaterial Needed (with waste): {totalWithWaste:F2} sq ft\nEstimated Total Cost: ${totalCost:F2}";
            Clipboard.SetText(result);
            MessageBox.Show("Results copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            string result = $"Total Area: {totalArea:F2} sq ft\nMaterial Needed (with waste): {totalWithWaste:F2} sq ft";
            Clipboard.SetText(result);
            MessageBox.Show("Results copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void MaterialTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (WasteFactorTextBox == null) return;

        int selectedIndex = MaterialTypeComboBox.SelectedIndex;
        switch (selectedIndex)
        {
            case 0:
            case 1:
            case 2:
                WasteFactorTextBox.Text = "10";
                break;
            case 3:
            case 4:
            case 5:
                WasteFactorTextBox.Text = "7";
                break;
            case 6:
                WasteFactorTextBox.Text = "5";
                break;
            case 7:
                WasteFactorTextBox.Text = "15";
                break;
            default:
                WasteFactorTextBox.Text = "10";
                break;
        }
    }

    private void CostPerUnitTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isInitialized) return;
        UpdateTotals();
    }

    private void UpdateTotals()
    {
        if (TotalAreaLabel == null || MaterialNeededLabel == null || TotalCostLabel == null || CostPerUnitTextBox == null)
            return;

        double totalArea = sections.Sum(s => s.AreaSqFt);
        double totalWithWaste = sections.Sum(s => s.AreaWithWasteSqFt);

        TotalAreaLabel.Text = $"Total Area: {totalArea:F2} sq ft";
        MaterialNeededLabel.Text = $"Material Needed (with waste): {totalWithWaste:F2} sq ft";

        if (double.TryParse(CostPerUnitTextBox.Text, out double costPerUnit) && costPerUnit > 0)
        {
            double totalCost = totalWithWaste * costPerUnit;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }

    private class SectionData
    {
        public Measurement Length { get; set; }
        public Measurement Width { get; set; }
        public string MaterialType { get; set; } = "";
        public double WasteFactor { get; set; }
        public double AreaSqFt { get; set; }
        public double AreaWithWasteSqFt { get; set; }
    }
}
