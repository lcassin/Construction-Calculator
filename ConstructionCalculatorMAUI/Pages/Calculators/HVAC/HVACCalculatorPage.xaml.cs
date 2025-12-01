using System.Collections.ObjectModel;
using ConstructionCalculatorMAUI.Shared.Help;

namespace ConstructionCalculatorMAUI.Pages.Calculators.HVAC;

public partial class HVACCalculatorPage : ContentPage
{
    private readonly List<ZoneData> zones = new();
    private readonly ObservableCollection<string> zoneDisplayList = new();

    public HVACCalculatorPage()
    {
        InitializeComponent();
        ZonesCollectionView.ItemsSource = zoneDisplayList;
    }

    private async void AddZoneButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!double.TryParse(LengthEntry.Text, out double length) || length <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid length.", "OK");
                return;
            }

            if (!double.TryParse(WidthEntry.Text, out double width) || width <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid width.", "OK");
                return;
            }

            if (!double.TryParse(HeightEntry.Text, out double height) || height <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid ceiling height.", "OK");
                return;
            }

            if (!int.TryParse(WindowsEntry.Text, out int windows) || windows < 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number of windows.", "OK");
                return;
            }

            if (!int.TryParse(OccupantsEntry.Text, out int occupants) || occupants < 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid number of occupants.", "OK");
                return;
            }

            string zoneName = string.IsNullOrWhiteSpace(ZoneNameEntry.Text) ? $"Zone {zones.Count + 1}" : ZoneNameEntry.Text;

            var zoneData = CalculateZoneLoad(zoneName, length, width, height, windows, occupants,
                InsulationPicker.SelectedIndex, SunExposurePicker.SelectedIndex,
                ClimateZonePicker.SelectedIndex, KitchenCheckBox.IsChecked);

            zones.Add(zoneData);

            string displayText = $"{zoneName}: {zoneData.SquareFeet:F0} sq ft | Heating: {zoneData.HeatingBTU:F0} BTU | Cooling: {zoneData.CoolingBTU:F0} BTU | {zoneData.CFM:F0} CFM";
            zoneDisplayList.Add(displayText);

            UpdateTotals();

            ZoneNameEntry.Text = "";
            LengthEntry.Text = "";
            WidthEntry.Text = "";
            WindowsEntry.Text = "2";
            OccupantsEntry.Text = "2";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Calculation Error: {ex.Message}", "OK");
        }
    }

    private async void RemoveZoneButton_Clicked(object sender, EventArgs e)
    {
        var selectedItem = ZonesCollectionView.SelectedItem as string;
        if (selectedItem != null)
        {
            int index = zoneDisplayList.IndexOf(selectedItem);
            if (index >= 0)
            {
                zones.RemoveAt(index);
                zoneDisplayList.RemoveAt(index);
                UpdateTotals();
            }
        }
        else
        {
            await DisplayAlert("Selection Required", "Please select a zone to remove.", "OK");
        }
    }

    private void ClearAllZonesButton_Clicked(object sender, EventArgs e)
    {
        zones.Clear();
        zoneDisplayList.Clear();
        UpdateTotals();
    }

    private ZoneData CalculateZoneLoad(string name, double length, double width, double height, int windows, int occupants,
        int insulationIndex, int sunExposureIndex, int climateIndex, bool hasKitchen)
    {
        double roomVolume = length * width * height;
        double squareFeet = length * width;

        double baseBTUPerSqFt = 20.0;

        double insulationMultiplier = insulationIndex switch
        {
            0 => 1.3,
            1 => 1.0,
            2 => 0.85,
            3 => 0.7,
            _ => 1.0
        };

        double sunExposureMultiplier = sunExposureIndex switch
        {
            0 => 0.9,
            1 => 1.0,
            2 => 1.15,
            _ => 1.0
        };

        double climateMultiplier = climateIndex switch
        {
            0 => 0.9,
            1 => 1.0,
            2 => 1.2,
            _ => 1.0
        };

        double heatingBTU = squareFeet * baseBTUPerSqFt * insulationMultiplier * climateMultiplier;
        heatingBTU += windows * 1000;
        heatingBTU += occupants * 400;

        double coolingBTU = squareFeet * baseBTUPerSqFt * insulationMultiplier * sunExposureMultiplier * climateMultiplier;
        coolingBTU += windows * 1000;
        coolingBTU += occupants * 600;

        if (hasKitchen)
        {
            coolingBTU += 4000;
            heatingBTU += 2000;
        }

        double cfm = coolingBTU / 30.0;

        return new ZoneData
        {
            Name = name,
            Length = length,
            Width = width,
            Height = height,
            RoomVolume = roomVolume,
            SquareFeet = squareFeet,
            HeatingBTU = heatingBTU,
            CoolingBTU = coolingBTU,
            CFM = cfm
        };
    }

    private void UpdateTotals()
    {
        if (zones.Count == 0)
        {
            TotalSquareFeetLabel.Text = "Total Area: 0 sq ft";
            TotalHeatingBTULabel.Text = "Total Heating BTU: 0 BTU/hr";
            TotalCoolingBTULabel.Text = "Total Cooling BTU: 0 BTU/hr";
            TotalTonnageLabel.Text = "Total AC Tonnage: 0.0 tons";
            TotalCFMLabel.Text = "Total CFM: 0 CFM";
            DiversityHeatingLabel.Text = "";
            DiversityCoolingLabel.Text = "";
            DiversityTonnageLabel.Text = "";
            return;
        }

        double totalSquareFeet = zones.Sum(z => z.SquareFeet);
        double totalHeatingBTU = zones.Sum(z => z.HeatingBTU);
        double totalCoolingBTU = zones.Sum(z => z.CoolingBTU);
        double totalCFM = zones.Sum(z => z.CFM);
        double totalTonnage = totalCoolingBTU / 12000.0;

        TotalSquareFeetLabel.Text = $"Total Area: {totalSquareFeet:F0} sq ft ({zones.Count} zones)";
        TotalHeatingBTULabel.Text = $"Total Heating BTU: {totalHeatingBTU:F0} BTU/hr";
        TotalCoolingBTULabel.Text = $"Total Cooling BTU: {totalCoolingBTU:F0} BTU/hr";
        TotalTonnageLabel.Text = $"Total AC Tonnage: {totalTonnage:F2} tons (recommend {Math.Ceiling(totalTonnage * 2) / 2:F1} ton unit)";
        TotalCFMLabel.Text = $"Total CFM: {totalCFM:F0} CFM";

        if (DiversityFactorCheckBox?.IsChecked == true && double.TryParse(DiversityFactorEntry?.Text, out double diversityFactor))
        {
            double factor = diversityFactor / 100.0;
            double diversityHeatingBTU = totalHeatingBTU * factor;
            double diversityCoolingBTU = totalCoolingBTU * factor;
            double diversityTonnage = diversityCoolingBTU / 12000.0;

            DiversityHeatingLabel.Text = $"With {diversityFactor}% diversity: {diversityHeatingBTU:F0} BTU/hr heating";
            DiversityCoolingLabel.Text = $"With {diversityFactor}% diversity: {diversityCoolingBTU:F0} BTU/hr cooling";
            DiversityTonnageLabel.Text = $"With {diversityFactor}% diversity: {diversityTonnage:F2} tons (recommend {Math.Ceiling(diversityTonnage * 2) / 2:F1} ton unit)";
        }
        else
        {
            if (DiversityHeatingLabel != null) DiversityHeatingLabel.Text = "";
            if (DiversityCoolingLabel != null) DiversityCoolingLabel.Text = "";
            if (DiversityTonnageLabel != null) DiversityTonnageLabel.Text = "";
        }

        if (DuctCFMEntry != null)
            DuctCFMEntry.Text = totalCFM.ToString("F0");
    }

    private void DiversityFactorCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateTotals();
    }

    private void DiversityFactorEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DiversityFactorCheckBox?.IsChecked == true)
        {
            UpdateTotals();
        }
    }

    private async void CalculateDuctButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!double.TryParse(DuctCFMEntry.Text, out double cfm) || cfm <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid CFM value.", "OK");
                return;
            }

            if (!double.TryParse(VelocityEntry.Text, out double velocity) || velocity <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid velocity (typical: 600-900 FPM).", "OK");
                return;
            }

            double requiredArea = (cfm / velocity) * 144;

            double aspectRatio = 2.0;
            double height = Math.Sqrt(requiredArea / aspectRatio);
            double width = height * aspectRatio;

            int heightInches = (int)Math.Ceiling(height);
            int widthInches = (int)Math.Ceiling(width);

            int[] standardSizes = { 4, 6, 8, 10, 12, 14, 16, 18, 20, 24, 30, 36 };
            heightInches = standardSizes.FirstOrDefault(s => s >= heightInches, heightInches);
            widthInches = standardSizes.FirstOrDefault(s => s >= widthInches, widthInches);

            double actualArea = (heightInches * widthInches) / 144.0;
            double actualVelocity = cfm / actualArea;

            DuctSizeLabel.Text = $"Duct Size: {heightInches}\" × {widthInches}\" (actual velocity: {actualVelocity:F0} FPM)";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Calculation Error: {ex.Message}", "OK");
        }
    }

    private async void CopyResultsButton_Clicked(object sender, EventArgs e)
    {
        if (zones.Count == 0)
        {
            await DisplayAlert("No Zones", "Please add at least one zone first.", "OK");
            return;
        }

        string results = "HVAC Multi-Zone Calculation Results\n";
        results += "=====================================\n\n";

        foreach (var zone in zones)
        {
            results += $"{zone.Name}:\n";
            results += $"  Area: {zone.SquareFeet:F0} sq ft\n";
            results += $"  Heating: {zone.HeatingBTU:F0} BTU/hr\n";
            results += $"  Cooling: {zone.CoolingBTU:F0} BTU/hr\n";
            results += $"  CFM: {zone.CFM:F0}\n\n";
        }

        results += "TOTALS:\n";
        results += $"{TotalSquareFeetLabel.Text}\n";
        results += $"{TotalHeatingBTULabel.Text}\n";
        results += $"{TotalCoolingBTULabel.Text}\n";
        results += $"{TotalTonnageLabel.Text}\n";
        results += $"{TotalCFMLabel.Text}\n";

        if (!string.IsNullOrEmpty(DiversityHeatingLabel.Text))
        {
            results += $"\n{DiversityHeatingLabel.Text}\n";
            results += $"{DiversityCoolingLabel.Text}\n";
            results += $"{DiversityTonnageLabel.Text}\n";
        }

        if (DuctSizeLabel.Text != "Duct Size: Not calculated")
        {
            results += $"\n{DuctSizeLabel.Text}";
        }

        await Clipboard.SetTextAsync(results);
        await DisplayAlert("Copied", "Results copied to clipboard!", "OK");
    }

    private class ZoneData
    {
        public string Name { get; set; } = "";
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RoomVolume { get; set; }
        public double SquareFeet { get; set; }
        public double HeatingBTU { get; set; }
        public double CoolingBTU { get; set; }
        public double CFM { get; set; }
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage(CalculatorKind.HVAC));
    }
}
