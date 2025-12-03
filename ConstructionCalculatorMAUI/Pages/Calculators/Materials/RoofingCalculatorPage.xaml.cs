using ConstructionCalculatorMAUI.Shared.Help;

namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class RoofingCalculatorPage : ContentPage
{
    private int lastCalculatedBundles = 0;

    public RoofingCalculatorPage()
    {
        InitializeComponent();
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        try
        {
            double length = double.Parse(LengthEntry.Text);
            double width = double.Parse(WidthEntry.Text);
            string pitchStr = PitchEntry.Text.Trim();
            double bundlesPerSquare = double.Parse(BundlesPerSquareEntry.Text);
            double wastePercent = double.Parse(WastePercentEntry.Text);

            double pitchFactor = CalculatePitchFactor(pitchStr);
            
            double planArea = length * width;
            double roofArea = planArea * pitchFactor;
            
            double squares = roofArea / 100.0;
            double squaresWithWaste = squares * (1 + wastePercent / 100.0);
            
            int totalBundles = (int)Math.Ceiling(squaresWithWaste * bundlesPerSquare);
            int roundedSquares = (int)Math.Ceiling(squaresWithWaste);

            double angle = CalculatePitchAngle(pitchStr);

            ResultLabel.Text = $"Roofing Requirements:\n\n" +
                              $"Plan Area: {planArea:F2} sq ft\n" +
                              $"Roof Area: {roofArea:F2} sq ft\n" +
                              $"Pitch Factor: {pitchFactor:F3}\n" +
                              $"Pitch Angle: {angle:F1}°\n\n" +
                              $"Squares Needed: {squaresWithWaste:F2} ({roundedSquares} rounded)\n" +
                              $"Bundles Needed: {totalBundles}";
            
            lastCalculatedBundles = totalBundles;
            UpdateCost();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease check your inputs.", "OK");
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

    private void CostPerBundleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null || CostPerBundleEntry == null) return;
        
        if (double.TryParse(CostPerBundleEntry.Text, out double costPerBundle) && lastCalculatedBundles > 0)
        {
            double totalCost = lastCalculatedBundles * costPerBundle;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage(CalculatorKind.Roofing));
    }
}
