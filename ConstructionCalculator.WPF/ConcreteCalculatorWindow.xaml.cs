using System;
using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF;

public partial class ConcreteCalculatorWindow : Window
{
    public ConcreteCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CalculationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CalculationTypeComboBox.SelectedItem is ComboBoxItem item)
        {
            string type = item.Content.ToString() ?? "";
            
            SlabInputs.Visibility = type == "Slab" ? Visibility.Visible : Visibility.Collapsed;
            FootingInputs.Visibility = type == "Footing" ? Visibility.Visible : Visibility.Collapsed;
            ColumnInputs.Visibility = type == "Column" ? Visibility.Visible : Visibility.Collapsed;
            
            ResultTextBlock.Text = "";
        }
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CalculationTypeComboBox.SelectedItem is not ComboBoxItem item)
                return;

            string type = item.Content.ToString() ?? "";
            double wastePercent = double.Parse(WastePercentTextBox.Text);
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
        double length = double.Parse(SlabLengthTextBox.Text);
        double width = double.Parse(SlabWidthTextBox.Text);
        double thicknessInches = double.Parse(SlabThicknessTextBox.Text);
        
        double thicknessFeet = thicknessInches / 12.0;
        return length * width * thicknessFeet;
    }

    private double CalculateFooting()
    {
        double perimeter = double.Parse(FootingPerimeterTextBox.Text);
        double widthInches = double.Parse(FootingWidthTextBox.Text);
        double depthInches = double.Parse(FootingDepthTextBox.Text);
        
        double widthFeet = widthInches / 12.0;
        double depthFeet = depthInches / 12.0;
        return perimeter * widthFeet * depthFeet;
    }

    private double CalculateColumn()
    {
        double diameterInches = double.Parse(ColumnDiameterTextBox.Text);
        double heightFeet = double.Parse(ColumnHeightTextBox.Text);
        int quantity = int.Parse(ColumnQuantityTextBox.Text);
        
        double radiusFeet = (diameterInches / 2.0) / 12.0;
        double volumePerColumn = Math.PI * radiusFeet * radiusFeet * heightFeet;
        return volumePerColumn * quantity;
    }
}
