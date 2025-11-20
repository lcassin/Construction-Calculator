using System;
using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Construction.Drywall;

public partial class DrywallCalculatorWindow : Window
{
    public DrywallCalculatorWindow()
    {
        InitializeComponent();
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double length = double.Parse(LengthTextBox.Text);
            double width = double.Parse(WidthTextBox.Text);
            double height = double.Parse(HeightTextBox.Text);
            bool includeCeiling = IncludeCeilingCheckBox.IsChecked ?? false;
            double wastePercent = double.Parse(WastePercentTextBox.Text);

            double sheetArea = GetSheetArea();

            double wallArea = 2 * (length + width) * height;
            double ceilingArea = includeCeiling ? length * width : 0;
            double totalArea = wallArea + ceilingArea;

            double sheetsNeeded = totalArea / sheetArea;
            double sheetsWithWaste = sheetsNeeded * (1 + wastePercent / 100.0);
            int roundedSheets = (int)Math.Ceiling(sheetsWithWaste);

            string sheetSize = GetSheetSizeString();

            ResultTextBlock.Text = $"Drywall Requirements:\n\n" +
                                  $"Wall Area: {wallArea:F2} sq ft\n" +
                                  $"Ceiling Area: {ceilingArea:F2} sq ft\n" +
                                  $"Total Area: {totalArea:F2} sq ft\n\n" +
                                  $"Sheet Size: {sheetSize}\n" +
                                  $"Sheets Needed: {sheetsNeeded:F2}\n" +
                                  $"With {wastePercent}% waste: {sheetsWithWaste:F2}\n" +
                                  $"Order: {roundedSheets} sheets";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nPlease check your inputs.", 
                          "Calculation Error", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private double GetSheetArea()
    {
        if (SheetSizeComboBox.SelectedItem is ComboBoxItem item)
        {
            string content = item.Content.ToString() ?? "";
            if (content.Contains("4' × 8'")) return 32;
            if (content.Contains("4' × 10'")) return 40;
            if (content.Contains("4' × 12'")) return 48;
        }
        return 32;
    }

    private string GetSheetSizeString()
    {
        if (SheetSizeComboBox.SelectedItem is ComboBoxItem item)
        {
            string content = item.Content.ToString() ?? "";
            if (content.Contains("4' × 8'")) return "4' × 8'";
            if (content.Contains("4' × 10'")) return "4' × 10'";
            if (content.Contains("4' × 12'")) return "4' × 12'";
        }
        return "4' × 8'";
    }
}
