using System;
using System.Windows;

namespace ConstructionCalculator.WPF.Calculators.Materials.Paint;

public partial class PaintCalculatorWindow : Window
{
    private int lastCalculatedGallons = 0;

    public PaintCalculatorWindow()
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
            int coats = int.Parse(CoatsTextBox.Text);
            double coverage = double.Parse(CoverageTextBox.Text);
            int doors = int.Parse(DoorsTextBox.Text);
            int windows = int.Parse(WindowsTextBox.Text);

            double wallArea = 2 * (length + width) * height;
            double ceilingArea = includeCeiling ? length * width : 0;
            double totalArea = wallArea + ceilingArea;
            
            double openingsArea = (doors * 20) + (windows * 15);
            double paintableArea = Math.Max(0, totalArea - openingsArea);
            
            double totalAreaWithCoats = paintableArea * coats;
            double gallonsNeeded = totalAreaWithCoats / coverage;
            int roundedGallons = (int)Math.Ceiling(gallonsNeeded);

            ResultTextBlock.Text = $"Paint Requirements:\n\n" +
                                  $"Wall Area: {wallArea:F2} sq ft\n" +
                                  $"Ceiling Area: {ceilingArea:F2} sq ft\n" +
                                  $"Openings: {openingsArea:F2} sq ft\n" +
                                  $"Paintable Area: {paintableArea:F2} sq ft\n\n" +
                                  $"Total with {coats} coat(s): {totalAreaWithCoats:F2} sq ft\n" +
                                  $"Gallons Needed: {gallonsNeeded:F2}\n" +
                                  $"Order: {roundedGallons} gallon(s)";
            
            lastCalculatedGallons = roundedGallons;
            UpdateCost();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nPlease check your inputs.", 
                          "Calculation Error", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void CostPerGallonTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null || CostPerGallonTextBox == null) return;
        
        if (double.TryParse(CostPerGallonTextBox.Text, out double costPerGallon) && lastCalculatedGallons > 0)
        {
            double totalCost = lastCalculatedGallons * costPerGallon;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }
}
