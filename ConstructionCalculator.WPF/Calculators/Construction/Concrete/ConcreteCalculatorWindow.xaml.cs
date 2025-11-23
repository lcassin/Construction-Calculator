using System;
using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Construction.Concrete;

public partial class ConcreteCalculatorWindow : Window
{
    private bool _isInitializing;
    private int lastCalculatedYards = 0;

    public ConcreteCalculatorWindow()
    {
        _isInitializing = true;
        InitializeComponent();
        _isInitializing = false;
        
        UpdatePanels();
    }

    private void CalculationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        var type = (CalculationTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Slab";
        
        SlabInputs.Visibility = type == "Slab" ? Visibility.Visible : Visibility.Collapsed;
        FootingInputs.Visibility = type == "Footing" ? Visibility.Visible : Visibility.Collapsed;
        ColumnInputs.Visibility = type == "Column" ? Visibility.Visible : Visibility.Collapsed;
        
        ResultTextBlock.Text = "";
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CalculationTypeComboBox.SelectedItem is not ComboBoxItem item)
                return;

            string type = item.Content.ToString() ?? "";
            
            if (!double.TryParse(WastePercentTextBox.Text, out double wastePercent))
            {
                MessageBox.Show("Please enter a valid waste percentage.", 
                              "Invalid Input", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }
            
            double cubicFeet = 0;

            switch (type)
            {
                case "Slab":
                    cubicFeet = CalculateSlab();
                    break;
                case "Footing":
                    cubicFeet = CalculateFooting();
                    break;
                case "Column":
                    cubicFeet = CalculateColumn();
                    break;
            }

            double cubicYards = cubicFeet / 27.0;
            double cubicYardsWithWaste = cubicYards * (1 + wastePercent / 100.0);
            int roundedYards = (int)Math.Ceiling(cubicYardsWithWaste);

            ResultTextBlock.Text = $"Concrete Required:\n\n" +
                                  $"Base: {cubicYards:F2} cubic yards\n" +
                                  $"With {wastePercent}% waste: {cubicYardsWithWaste:F2} cubic yards\n" +
                                  $"Order: {roundedYards} cubic yards";
            
            lastCalculatedYards = roundedYards;
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

    private double CalculateSlab()
    {
        if (!double.TryParse(SlabLengthTextBox.Text, out double length) ||
            !double.TryParse(SlabWidthTextBox.Text, out double width) ||
            !double.TryParse(SlabThicknessTextBox.Text, out double thicknessInches))
        {
            throw new FormatException("Please enter valid numbers for length, width, and thickness.");
        }
        
        double thicknessFeet = thicknessInches / 12.0;
        return length * width * thicknessFeet;
    }

    private double CalculateFooting()
    {
        if (!double.TryParse(FootingPerimeterTextBox.Text, out double perimeter) ||
            !double.TryParse(FootingWidthTextBox.Text, out double widthInches) ||
            !double.TryParse(FootingDepthTextBox.Text, out double depthInches))
        {
            throw new FormatException("Please enter valid numbers for perimeter, width, and depth.");
        }
        
        double widthFeet = widthInches / 12.0;
        double depthFeet = depthInches / 12.0;
        return perimeter * widthFeet * depthFeet;
    }

    private double CalculateColumn()
    {
        if (!double.TryParse(ColumnDiameterTextBox.Text, out double diameterInches) ||
            !double.TryParse(ColumnHeightTextBox.Text, out double heightFeet) ||
            !int.TryParse(ColumnQuantityTextBox.Text, out int quantity))
        {
            throw new FormatException("Please enter valid numbers for diameter, height, and quantity.");
        }
        
        double radiusFeet = (diameterInches / 2.0) / 12.0;
        double volumePerColumn = Math.PI * radiusFeet * radiusFeet * heightFeet;
        return volumePerColumn * quantity;
    }

    private void CostPerYardTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null) return;
        
        if (double.TryParse(CostPerYardTextBox.Text, out double costPerYard) && lastCalculatedYards > 0)
        {
            double totalCost = lastCalculatedYards * costPerYard;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }
}
