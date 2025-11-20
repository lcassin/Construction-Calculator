using System;
using System.Windows;

namespace ConstructionCalculator.WPF.Calculators.Geometry.Roofing;

public partial class RoofingCalculatorWindow : Window
{
    public RoofingCalculatorWindow()
    {
        InitializeComponent();
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double length = double.Parse(LengthTextBox.Text);
            double width = double.Parse(WidthTextBox.Text);
            string pitchStr = PitchTextBox.Text.Trim();
            double bundlesPerSquare = double.Parse(BundlesPerSquareTextBox.Text);
            double wastePercent = double.Parse(WastePercentTextBox.Text);

            double pitchFactor = CalculatePitchFactor(pitchStr);
            
            double planArea = length * width;
            double roofArea = planArea * pitchFactor;
            
            double squares = roofArea / 100.0;
            double squaresWithWaste = squares * (1 + wastePercent / 100.0);
            
            int totalBundles = (int)Math.Ceiling(squaresWithWaste * bundlesPerSquare);
            int roundedSquares = (int)Math.Ceiling(squaresWithWaste);

            double angle = CalculatePitchAngle(pitchStr);

            ResultTextBlock.Text = $"Roofing Requirements:\n\n" +
                                  $"Plan Area: {planArea:F2} sq ft\n" +
                                  $"Roof Area: {roofArea:F2} sq ft\n" +
                                  $"Pitch Factor: {pitchFactor:F3}\n" +
                                  $"Pitch Angle: {angle:F1}°\n\n" +
                                  $"Squares Needed: {squaresWithWaste:F2} ({roundedSquares} rounded)\n" +
                                  $"Bundles Needed: {totalBundles}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nPlease check your inputs.", 
                          "Calculation Error", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private double CalculatePitchFactor(string pitchStr)
    {
        string[] parts = pitchStr.Split('/');
        if (parts.Length != 2)
            throw new FormatException("Pitch must be in format rise/run (e.g., 4/12)");

        double rise = double.Parse(parts[0].Trim());
        double run = double.Parse(parts[1].Trim());
        
        double slopeLength = Math.Sqrt(rise * rise + run * run);
        return slopeLength / run;
    }

    private double CalculatePitchAngle(string pitchStr)
    {
        string[] parts = pitchStr.Split('/');
        double rise = double.Parse(parts[0].Trim());
        double run = double.Parse(parts[1].Trim());
        
        return Math.Atan(rise / run) * (180.0 / Math.PI);
    }
}
