using System.Collections.ObjectModel;

namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class FlooringCalculatorPage : ContentPage
{
    private readonly List<SectionData> sections = new();
    private readonly ObservableCollection<string> sectionDisplayList = new();
    private bool _isInitialized = false;

    public FlooringCalculatorPage()
    {
        InitializeComponent();
        SectionsCollectionView.ItemsSource = sectionDisplayList;
        _isInitialized = true;
        UpdateTotals();
    }

    private async void AddSectionButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LengthEntry.Text) || string.IsNullOrWhiteSpace(WidthEntry.Text))
            {
                await DisplayAlert("Input Required", "Please enter both length and width.", "OK");
                return;
            }

            if (!double.TryParse(WasteFactorEntry.Text, out double wasteFactor) || wasteFactor < 0 || wasteFactor > 100)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid waste factor between 0 and 100.", "OK");
                return;
            }

            if (!double.TryParse(LengthEntry.Text, out double length) || !double.TryParse(WidthEntry.Text, out double width))
            {
                await DisplayAlert("Invalid Input", "Please enter valid numbers for length and width.", "OK");
                return;
            }

            double sqft = length * width;
            double sqftWithWaste = sqft * (1 + wasteFactor / 100.0);

            string materialType = MaterialTypePicker.SelectedItem?.ToString() ?? "Unknown";

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

            string displayText = $"{length:F2} × {width:F2} | {materialType} | {sqft:F2} sq ft (+{wasteFactor}% = {sqftWithWaste:F2} sq ft)";
            sectionDisplayList.Add(displayText);

            UpdateTotals();

            LengthEntry.Text = "";
            WidthEntry.Text = "";
            LengthEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void RemoveSectionButton_Clicked(object sender, EventArgs e)
    {
        var selectedItem = SectionsCollectionView.SelectedItem as string;
        if (selectedItem != null)
        {
            int index = sectionDisplayList.IndexOf(selectedItem);
            if (index >= 0)
            {
                sections.RemoveAt(index);
                sectionDisplayList.RemoveAt(index);
                UpdateTotals();
            }
        }
        else
        {
            await DisplayAlert("Selection Required", "Please select a section to remove.", "OK");
        }
    }

    private void ClearAllButton_Clicked(object sender, EventArgs e)
    {
        sections.Clear();
        sectionDisplayList.Clear();
        UpdateTotals();
        LengthEntry.Text = "";
        WidthEntry.Text = "";
        CostPerUnitEntry.Text = "0";
    }

    private async void CopyResultsButton_Clicked(object sender, EventArgs e)
    {
        double totalArea = sections.Sum(s => s.AreaSqFt);
        double totalWithWaste = sections.Sum(s => s.AreaWithWasteSqFt);
        
        if (double.TryParse(CostPerUnitEntry.Text, out double costPerUnit))
        {
            double totalCost = totalWithWaste * costPerUnit;
            string result = $"Total Area: {totalArea:F2} sq ft\nMaterial Needed (with waste): {totalWithWaste:F2} sq ft\nEstimated Total Cost: ${totalCost:F2}";
            await Clipboard.SetTextAsync(result);
            await DisplayAlert("Copied", "Results copied to clipboard!", "OK");
        }
        else
        {
            string result = $"Total Area: {totalArea:F2} sq ft\nMaterial Needed (with waste): {totalWithWaste:F2} sq ft";
            await Clipboard.SetTextAsync(result);
            await DisplayAlert("Copied", "Results copied to clipboard!", "OK");
        }
    }

    private void MaterialTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (WasteFactorEntry == null) return;

        int selectedIndex = MaterialTypePicker.SelectedIndex;
        switch (selectedIndex)
        {
            case 0:
            case 1:
            case 2:
                WasteFactorEntry.Text = "10";
                break;
            case 3:
            case 4:
            case 5:
                WasteFactorEntry.Text = "7";
                break;
            case 6:
                WasteFactorEntry.Text = "5";
                break;
            case 7:
                WasteFactorEntry.Text = "15";
                break;
            default:
                WasteFactorEntry.Text = "10";
                break;
        }
    }

    private void CostPerUnitEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isInitialized) return;
        UpdateTotals();
    }

    private void UpdateTotals()
    {
        if (TotalAreaLabel == null || MaterialNeededLabel == null || TotalCostLabel == null || CostPerUnitEntry == null)
            return;

        double totalArea = sections.Sum(s => s.AreaSqFt);
        double totalWithWaste = sections.Sum(s => s.AreaWithWasteSqFt);

        TotalAreaLabel.Text = $"Total Area: {totalArea:F2} sq ft";
        MaterialNeededLabel.Text = $"Material Needed (with waste): {totalWithWaste:F2} sq ft";

        if (double.TryParse(CostPerUnitEntry.Text, out double costPerUnit) && costPerUnit > 0)
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
        public double Length { get; set; }
        public double Width { get; set; }
        public string MaterialType { get; set; } = "";
        public double WasteFactor { get; set; }
        public double AreaSqFt { get; set; }
        public double AreaWithWasteSqFt { get; set; }
    }
}
