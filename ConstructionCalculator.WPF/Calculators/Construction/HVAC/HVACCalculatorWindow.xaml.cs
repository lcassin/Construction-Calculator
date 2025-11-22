using System.Windows;

namespace ConstructionCalculator.WPF.Calculators.Construction.HVAC;

public partial class HVACCalculatorWindow : Window
{
    public HVACCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
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

            double roomVolume = length * width * height;
            double squareFeet = length * width;

            double baseBTUPerSqFt = 20.0;

            double insulationMultiplier = InsulationComboBox.SelectedIndex switch
            {
                0 => 1.3,
                1 => 1.0,
                2 => 0.85,
                3 => 0.7,
                _ => 1.0
            };

            double sunExposureMultiplier = SunExposureComboBox.SelectedIndex switch
            {
                0 => 0.9,
                1 => 1.0,
                2 => 1.15,
                _ => 1.0
            };

            double climateMultiplier = ClimateZoneComboBox.SelectedIndex switch
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

            if (KitchenCheckBox.IsChecked == true)
            {
                coolingBTU += 4000;
                heatingBTU += 2000;
            }

            double tonnage = coolingBTU / 12000.0;

            double cfm = coolingBTU / 30.0;

            RoomVolumeLabel.Text = $"Room Volume: {roomVolume:F0} cubic feet ({squareFeet:F0} sq ft)";
            BTUHeatingLabel.Text = $"Heating BTU Required: {heatingBTU:F0} BTU/hr";
            BTUCoolingLabel.Text = $"Cooling BTU Required: {coolingBTU:F0} BTU/hr";
            TonnageLabel.Text = $"AC Tonnage Required: {tonnage:F2} tons (recommend {Math.Ceiling(tonnage * 2) / 2:F1} ton unit)";
            CFMLabel.Text = $"Recommended CFM: {cfm:F0} CFM";

            DuctCFMTextBox.Text = cfm.ToString("F0");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateDuctButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!double.TryParse(DuctCFMTextBox.Text, out double cfm) || cfm <= 0)
            {
                MessageBox.Show("Please enter a valid CFM value.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(VelocityTextBox.Text, out double velocity) || velocity <= 0)
            {
                MessageBox.Show("Please enter a valid velocity (typical: 600-900 FPM).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CopyResultsButton_Click(object sender, RoutedEventArgs e)
    {
        string results = $"{RoomVolumeLabel.Text}\n{BTUHeatingLabel.Text}\n{BTUCoolingLabel.Text}\n{TonnageLabel.Text}\n{CFMLabel.Text}";
        
        if (DuctSizeLabel.Text != "Duct Size: Not calculated")
        {
            results += $"\n{DuctSizeLabel.Text}";
        }

        Clipboard.SetText(results);
        MessageBox.Show("Results copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
