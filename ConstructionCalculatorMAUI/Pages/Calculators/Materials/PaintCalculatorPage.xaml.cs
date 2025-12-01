namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class PaintCalculatorPage : ContentPage
{
    private int lastCalculatedGallons = 0;

    public PaintCalculatorPage()
    {
        InitializeComponent();
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        try
        {
            double length = double.Parse(LengthEntry.Text);
            double width = double.Parse(WidthEntry.Text);
            double height = double.Parse(HeightEntry.Text);
            bool includeCeiling = IncludeCeilingCheckBox.IsChecked;
            int coats = int.Parse(CoatsEntry.Text);
            double coverage = double.Parse(CoverageEntry.Text);
            int doors = int.Parse(DoorsEntry.Text);
            int windows = int.Parse(WindowsEntry.Text);

            double wallArea = 2 * (length + width) * height;
            double ceilingArea = includeCeiling ? length * width : 0;
            double totalArea = wallArea + ceilingArea;
            
            double openingsArea = (doors * 20) + (windows * 15);
            double paintableArea = Math.Max(0, totalArea - openingsArea);
            
            double totalAreaWithCoats = paintableArea * coats;
            double gallonsNeeded = totalAreaWithCoats / coverage;
            int roundedGallons = (int)Math.Ceiling(gallonsNeeded);

            ResultLabel.Text = $"Paint Requirements:\n\n" +
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
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease check your inputs.", "OK");
        }
    }

    private void CostPerGallonEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null || CostPerGallonEntry == null) return;
        
        if (double.TryParse(CostPerGallonEntry.Text, out double costPerGallon) && lastCalculatedGallons > 0)
        {
            double totalCost = lastCalculatedGallons * costPerGallon;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage(CalculatorKind.Paint));
    }
}
