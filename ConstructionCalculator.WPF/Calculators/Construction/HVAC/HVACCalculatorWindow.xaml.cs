using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Construction.HVAC;

public partial class HVACCalculatorWindow : Window
{
    private readonly List<ZoneData> zones = new();

    public HVACCalculatorWindow()
    {
        InitializeComponent();
    }

    private void AddZoneButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!double.TryParse(LengthTextBox.Text, out double length) || length <= 0)
            {
                MessageBox.Show("Please enter a valid length.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(WidthTextBox.Text, out double width) || width <= 0)
            {
                MessageBox.Show("Please enter a valid width.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(HeightTextBox.Text, out double height) || height <= 0)
            {
                MessageBox.Show("Please enter a valid ceiling height.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(WindowsTextBox.Text, out int windows) || windows < 0)
            {
                MessageBox.Show("Please enter a valid number of windows.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(OccupantsTextBox.Text, out int occupants) || occupants < 0)
            {
                MessageBox.Show("Please enter a valid number of occupants.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string zoneName = string.IsNullOrWhiteSpace(ZoneNameTextBox.Text) ? $"Zone {zones.Count + 1}" : ZoneNameTextBox.Text;

            var zoneData = CalculateZoneLoad(zoneName, length, width, height, windows, occupants,
                InsulationComboBox.SelectedIndex, SunExposureComboBox.SelectedIndex,
                ClimateZoneComboBox.SelectedIndex, KitchenCheckBox.IsChecked == true);

            zones.Add(zoneData);

            string displayText = $"{zoneName}: {zoneData.SquareFeet:F0} sq ft | Heating: {zoneData.HeatingBTU:F0} BTU | Cooling: {zoneData.CoolingBTU:F0} BTU | {zoneData.CFM:F0} CFM";
            ZonesListBox.Items.Add(displayText);

            UpdateTotals();

            ZoneNameTextBox.Clear();
            LengthTextBox.Clear();
            WidthTextBox.Clear();
            WindowsTextBox.Text = "2";
            OccupantsTextBox.Text = "2";
            ZoneNameTextBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RemoveZoneButton_Click(object sender, RoutedEventArgs e)
    {
        if (ZonesListBox.SelectedIndex >= 0)
        {
            int index = ZonesListBox.SelectedIndex;
            zones.RemoveAt(index);
            ZonesListBox.Items.RemoveAt(index);
            UpdateTotals();
        }
        else
        {
            MessageBox.Show("Please select a zone to remove.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearAllZonesButton_Click(object sender, RoutedEventArgs e)
    {
        zones.Clear();
        ZonesListBox.Items.Clear();
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

        if (DiversityFactorCheckBox?.IsChecked == true && double.TryParse(DiversityFactorTextBox?.Text, out double diversityFactor))
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

        if (DuctCFMTextBox != null)
            DuctCFMTextBox.Text = totalCFM.ToString("F0");
    }

    private void DiversityFactorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        UpdateTotals();
    }

    private void DiversityFactorTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DiversityFactorCheckBox?.IsChecked == true)
        {
            UpdateTotals();
        }
    }

    // Duct calculator state
    private double calculatedDiameter = 0;
    private double calculatedVelocity = 0;

    private void DuctTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DuctTypeComboBox == null || RoundDuctPanel == null || RectangularDuctPanel == null)
            return;

        bool isRound = DuctTypeComboBox.SelectedIndex == 0;
        RoundDuctPanel.Visibility = isRound ? Visibility.Visible : Visibility.Collapsed;
        RectangularDuctPanel.Visibility = isRound ? Visibility.Collapsed : Visibility.Visible;
    }

    private void UseTotalCFMButton_Click(object sender, RoutedEventArgs e)
    {
        if (zones.Count == 0)
        {
            MessageBox.Show("Please add zones first to calculate total CFM.", "No Zones", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        double totalCFM = zones.Sum(z => z.CFM);
        DuctCFMTextBox.Text = totalCFM.ToString("F0");
    }

    private void CalculateDuctSizeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!double.TryParse(DuctCFMTextBox.Text, out double cfm) || cfm <= 0)
            {
                MessageBox.Show("Please enter a valid CFM value.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(DuctVelocityTextBox.Text, out double velocity) || velocity <= 0)
            {
                MessageBox.Show("Please enter a valid velocity (typical: 600-1200 FPM).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isRound = DuctTypeComboBox.SelectedIndex == 0;

            if (isRound)
            {
                CalculateRoundDuct(cfm, velocity);
            }
            else
            {
                CalculateRectangularDuct(cfm, velocity);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateRoundDuct(double cfm, double targetVelocity)
    {
        // Calculate required area in square feet
        double areaFt2 = cfm / targetVelocity;

        // Calculate diameter in feet, then convert to inches
        double diameterFt = Math.Sqrt(4 * areaFt2 / Math.PI);
        double diameterIn = diameterFt * 12;

        // Round to nearest standard size
        int[] standardSizes = { 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36 };
        int standardDiameter = standardSizes.FirstOrDefault(s => s >= diameterIn, (int)Math.Ceiling(diameterIn));

        // Calculate actual velocity with standard size
        double actualAreaFt2 = Math.PI * Math.Pow(standardDiameter / 12.0, 2) / 4;
        double actualVelocity = cfm / actualAreaFt2;

        calculatedDiameter = standardDiameter;
        calculatedVelocity = actualVelocity;

        RoundDiameterLabel.Text = $"Diameter: {standardDiameter}\" (standard size)";
        RoundActualVelocityLabel.Text = $"Actual Velocity: {actualVelocity:F0} FPM";

        // Add velocity warning if needed
        if (actualVelocity > 1200)
        {
            RoundActualVelocityLabel.Text += " ⚠ High velocity - may cause noise";
        }
        else if (actualVelocity < 400)
        {
            RoundActualVelocityLabel.Text += " ⚠ Low velocity - may be oversized";
        }
    }

    private void CalculateRectangularDuct(double cfm, double targetVelocity)
    {
        if (!double.TryParse(AspectRatioTextBox.Text, out double aspectRatio) || aspectRatio <= 0)
        {
            MessageBox.Show("Please enter a valid aspect ratio (typical: 1.5-3.0).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Calculate required area in square feet
        double areaFt2 = cfm / targetVelocity;

        // Calculate dimensions based on aspect ratio
        // Area = W * H, AspectRatio = W / H
        // So: H = sqrt(Area / AspectRatio), W = AspectRatio * H
        double heightFt = Math.Sqrt(areaFt2 / aspectRatio);
        double widthFt = aspectRatio * heightFt;

        // Convert to inches
        double heightIn = heightFt * 12;
        double widthIn = widthFt * 12;

        // Round to nearest standard size
        int[] standardSizes = { 4, 6, 8, 10, 12, 14, 16, 18, 20, 24, 30, 36, 42, 48 };
        int standardHeight = standardSizes.FirstOrDefault(s => s >= heightIn, (int)Math.Ceiling(heightIn));
        int standardWidth = standardSizes.FirstOrDefault(s => s >= widthIn, (int)Math.Ceiling(widthIn));

        // Calculate actual velocity with standard sizes
        double actualAreaFt2 = (standardWidth * standardHeight) / 144.0;
        double actualVelocity = cfm / actualAreaFt2;

        // Calculate equivalent diameter using ASHRAE formula
        double equivalentDiameter = 1.30 * Math.Pow(standardWidth * standardHeight, 0.625) / Math.Pow(standardWidth + standardHeight, 0.25);

        calculatedDiameter = equivalentDiameter;
        calculatedVelocity = actualVelocity;

        RectangularSizeLabel.Text = $"Size: {standardWidth}\" × {standardHeight}\" (standard sizes)";
        RectangularActualVelocityLabel.Text = $"Actual Velocity: {actualVelocity:F0} FPM";
        EquivalentDiameterLabel.Text = $"Equivalent Diameter: {equivalentDiameter:F1}\" (for friction calculations)";

        // Add velocity warning if needed
        if (actualVelocity > 1200)
        {
            RectangularActualVelocityLabel.Text += " ⚠ High velocity - may cause noise";
        }
        else if (actualVelocity < 400)
        {
            RectangularActualVelocityLabel.Text += " ⚠ Low velocity - may be oversized";
        }
    }

    private void CalculateFrictionButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (calculatedDiameter == 0 || calculatedVelocity == 0)
            {
                MessageBox.Show("Please calculate duct size first.", "Duct Size Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!double.TryParse(StraightLengthTextBox.Text, out double straightLength) || straightLength < 0)
            {
                MessageBox.Show("Please enter a valid straight length.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(FrictionRateTextBox.Text, out double frictionRate) || frictionRate <= 0)
            {
                MessageBox.Show("Please enter a valid friction rate (typical: 0.05-0.15 in. w.g./100 ft).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse fitting counts
            int.TryParse(Elbow90TextBox.Text, out int elbow90Count);
            int.TryParse(Elbow45TextBox.Text, out int elbow45Count);
            int.TryParse(TransitionsTextBox.Text, out int transitionsCount);
            int.TryParse(BranchesTextBox.Text, out int branchesCount);
            int.TryParse(DampersTextBox.Text, out int dampersCount);

            // Calculate equivalent lengths for fittings (in feet)
            // These are typical values - actual values vary by duct size and construction
            double equiv90 = elbow90Count * 35;  // ~35 ft per 90° elbow
            double equiv45 = elbow45Count * 20;  // ~20 ft per 45° elbow
            double equivTransitions = transitionsCount * 15;  // ~15 ft per transition
            double equivBranches = branchesCount * 25;  // ~25 ft per branch takeoff
            double equivDampers = dampersCount * 10;  // ~10 ft per damper

            double totalEquivLength = equiv90 + equiv45 + equivTransitions + equivBranches + equivDampers;
            double effectiveLength = straightLength + totalEquivLength;

            // Calculate friction loss
            double frictionLoss = frictionRate * (effectiveLength / 100.0);

            // Calculate velocity pressure (in. w.g.)
            // Pv = (V/4005)^2 where V is in FPM
            double velocityPressure = Math.Pow(calculatedVelocity / 4005.0, 2);

            // Total pressure drop is friction loss (we're using equivalent length method, so fittings are already included)
            double totalPressure = frictionLoss;

            // Update labels
            TotalEquivalentLengthLabel.Text = $"Total Equivalent Length: {totalEquivLength:F1} ft";
            EffectiveLengthLabel.Text = $"Effective Length (straight + fittings): {effectiveLength:F1} ft";
            FrictionLossLabel.Text = $"Friction Loss: {frictionLoss:F3} in. w.g.";
            VelocityPressureLabel.Text = $"Velocity Pressure: {velocityPressure:F3} in. w.g.";
            TotalPressureLabel.Text = $"Total Pressure Drop: {totalPressure:F3} in. w.g.";

            // Add pressure warning if needed
            if (totalPressure > 0.5)
            {
                TotalPressureLabel.Text += " ⚠ High pressure drop";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CopyResultsButton_Click(object sender, RoutedEventArgs e)
    {
        if (zones.Count == 0)
        {
            MessageBox.Show("Please add at least one zone first.", "No Zones", MessageBoxButton.OK, MessageBoxImage.Information);
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


        Clipboard.SetText(results);
        MessageBox.Show("Results copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
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
}
