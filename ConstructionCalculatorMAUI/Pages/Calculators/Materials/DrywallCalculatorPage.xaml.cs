using ConstructionCalculatorMAUI.Shared.Help;

namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class DrywallCalculatorPage : ContentPage
{
    private int lastCalculatedSheets = 0;

    public DrywallCalculatorPage()
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
            double wastePercent = double.Parse(WastePercentEntry.Text);

            double sheetArea = GetSheetArea();

            double wallArea = 2 * (length + width) * height;
            double ceilingArea = includeCeiling ? length * width : 0;
            double totalArea = wallArea + ceilingArea;

            double sheetsNeeded = totalArea / sheetArea;
            double sheetsWithWaste = sheetsNeeded * (1 + wastePercent / 100.0);
            int roundedSheets = (int)Math.Ceiling(sheetsWithWaste);

            string sheetSize = GetSheetSizeString();

            ResultLabel.Text = $"Drywall Requirements:\n\n" +
                              $"Wall Area: {wallArea:F2} sq ft\n" +
                              $"Ceiling Area: {ceilingArea:F2} sq ft\n" +
                              $"Total Area: {totalArea:F2} sq ft\n\n" +
                              $"Sheet Size: {sheetSize}\n" +
                              $"Sheets Needed: {sheetsNeeded:F2}\n" +
                              $"With {wastePercent}% waste: {sheetsWithWaste:F2}\n" +
                              $"Order: {roundedSheets} sheets";
            
            lastCalculatedSheets = roundedSheets;
            UpdateCost();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease check your inputs.", "OK");
        }
    }

    private double GetSheetArea()
    {
        string content = SheetSizePicker.SelectedItem?.ToString() ?? "";
        if (content.Contains("4' × 8'")) return 32;
        if (content.Contains("4' × 10'")) return 40;
        if (content.Contains("4' × 12'")) return 48;
        return 32;
    }

    private string GetSheetSizeString()
    {
        string content = SheetSizePicker.SelectedItem?.ToString() ?? "";
        if (content.Contains("4' × 8'")) return "4' × 8'";
        if (content.Contains("4' × 10'")) return "4' × 10'";
        if (content.Contains("4' × 12'")) return "4' × 12'";
        return "4' × 8'";
    }

    private void CostPerSheetEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null || CostPerSheetEntry == null) return;
        
        if (double.TryParse(CostPerSheetEntry.Text, out double costPerSheet) && lastCalculatedSheets > 0)
        {
            double totalCost = lastCalculatedSheets * costPerSheet;
            TotalCostLabel.Text = $"Estimated Total Cost: ${totalCost:F2}";
        }
        else
        {
            TotalCostLabel.Text = "Estimated Total Cost: $0.00";
        }
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage(CalculatorKind.Drywall));
    }
}
