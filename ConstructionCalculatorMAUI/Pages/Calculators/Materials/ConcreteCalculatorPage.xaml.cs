namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class ConcreteCalculatorPage : ContentPage
{
    private bool _isInitializing;
    private int lastCalculatedYards = 0;

    public ConcreteCalculatorPage()
    {
        _isInitializing = true;
        InitializeComponent();
        CalculationTypePicker.SelectedIndex = 0;
        _isInitializing = false;
        
        UpdatePanels();
    }

    private void CalculationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isInitializing) return;
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        var type = CalculationTypePicker.SelectedItem?.ToString() ?? "Slab";
        
        SlabInputs.IsVisible = type == "Slab";
        FootingInputs.IsVisible = type == "Footing";
        ColumnInputs.IsVisible = type == "Column";
        
        ResultLabel.Text = "";
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        try
        {
            string type = CalculationTypePicker.SelectedItem?.ToString() ?? "";
            
            if (!double.TryParse(WastePercentEntry.Text, out double wastePercent))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid waste percentage.", "OK");
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

            ResultLabel.Text = $"Concrete Required:\n\n" +
                              $"Base: {cubicYards:F2} cubic yards\n" +
                              $"With {wastePercent}% waste: {cubicYardsWithWaste:F2} cubic yards\n" +
                              $"Order: {roundedYards} cubic yards";
            
            lastCalculatedYards = roundedYards;
            UpdateCost();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease check your inputs.", "OK");
        }
    }

    private double CalculateSlab()
    {
        if (!double.TryParse(SlabLengthEntry.Text, out double length) ||
            !double.TryParse(SlabWidthEntry.Text, out double width) ||
            !double.TryParse(SlabThicknessEntry.Text, out double thicknessInches))
        {
            throw new FormatException("Please enter valid numbers for length, width, and thickness.");
        }
        
        double thicknessFeet = thicknessInches / 12.0;
        return length * width * thicknessFeet;
    }

    private double CalculateFooting()
    {
        if (!double.TryParse(FootingPerimeterEntry.Text, out double perimeter) ||
            !double.TryParse(FootingWidthEntry.Text, out double widthInches) ||
            !double.TryParse(FootingDepthEntry.Text, out double depthInches))
        {
            throw new FormatException("Please enter valid numbers for perimeter, width, and depth.");
        }
        
        double widthFeet = widthInches / 12.0;
        double depthFeet = depthInches / 12.0;
        return perimeter * widthFeet * depthFeet;
    }

    private double CalculateColumn()
    {
        if (!double.TryParse(ColumnDiameterEntry.Text, out double diameterInches) ||
            !double.TryParse(ColumnHeightEntry.Text, out double heightFeet) ||
            !int.TryParse(ColumnQuantityEntry.Text, out int quantity))
        {
            throw new FormatException("Please enter valid numbers for diameter, height, and quantity.");
        }
        
        double radiusFeet = (diameterInches / 2.0) / 12.0;
        double volumePerColumn = Math.PI * radiusFeet * radiusFeet * heightFeet;
        return volumePerColumn * quantity;
    }

    private void CostPerYardEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateCost();
    }

    private void UpdateCost()
    {
        if (TotalCostLabel == null || CostPerYardEntry == null) return;
        
        if (double.TryParse(CostPerYardEntry.Text, out double costPerYard) && lastCalculatedYards > 0)
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
