namespace ConstructionCalculatorMAUI.Pages.Calculators.Plumbing;

public partial class PlumbingCalculatorPage : ContentPage
{
    public PlumbingCalculatorPage()
    {
        InitializeComponent();
    }

    private async void CalculatePipeSizeButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!double.TryParse(FixtureUnitsEntry.Text, out double fixtureUnits) || fixtureUnits <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter valid fixture units.", "OK");
                return;
            }

            if (!double.TryParse(PipeLengthEntry.Text, out double pipeLength) || pipeLength <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid pipe length.", "OK");
                return;
            }

            if (!double.TryParse(WaterPressureEntry.Text, out double pressure) || pressure <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid water pressure.", "OK");
                return;
            }

            double flowRate = Math.Sqrt(fixtureUnits) * 4.0;

            double frictionFactor = PipeMaterialPicker.SelectedIndex switch
            {
                0 => 0.85,
                1 => 0.75,
                2 => 0.90,
                3 => 1.2,
                _ => 1.0
            };

            string recommendedSize;
            double velocity;
            double pressureLoss;

            if (flowRate <= 4)
            {
                recommendedSize = "1/2\"";
                velocity = flowRate / 0.196;
                pressureLoss = (pipeLength / 100) * 8.0 * frictionFactor;
            }
            else if (flowRate <= 8)
            {
                recommendedSize = "3/4\"";
                velocity = flowRate / 0.442;
                pressureLoss = (pipeLength / 100) * 4.5 * frictionFactor;
            }
            else if (flowRate <= 15)
            {
                recommendedSize = "1\"";
                velocity = flowRate / 0.785;
                pressureLoss = (pipeLength / 100) * 2.5 * frictionFactor;
            }
            else if (flowRate <= 25)
            {
                recommendedSize = "1-1/4\"";
                velocity = flowRate / 1.227;
                pressureLoss = (pipeLength / 100) * 1.5 * frictionFactor;
            }
            else if (flowRate <= 40)
            {
                recommendedSize = "1-1/2\"";
                velocity = flowRate / 1.767;
                pressureLoss = (pipeLength / 100) * 1.0 * frictionFactor;
            }
            else if (flowRate <= 75)
            {
                recommendedSize = "2\"";
                velocity = flowRate / 3.142;
                pressureLoss = (pipeLength / 100) * 0.5 * frictionFactor;
            }
            else
            {
                recommendedSize = "2-1/2\" or larger";
                velocity = flowRate / 4.909;
                pressureLoss = (pipeLength / 100) * 0.3 * frictionFactor;
            }

            FlowRateLabel.Text = $"Required Flow Rate: {flowRate:F1} GPM";
            RecommendedPipeSizeLabel.Text = $"Recommended Pipe Size: {recommendedSize}";
            VelocityLabel.Text = $"Flow Velocity: {velocity:F1} ft/s (optimal: 5-8 ft/s)";
            PressureLossLabel.Text = $"Estimated Pressure Loss: {pressureLoss:F1} PSI";

            if (velocity > 8)
            {
                await DisplayAlert("High Velocity Warning", 
                    "Warning: Flow velocity exceeds 8 ft/s. Consider using larger pipe to reduce noise and wear.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Calculation Error: {ex.Message}", "OK");
        }
    }

    private async void CalculateDrainSizeButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!double.TryParse(DFUEntry.Text, out double dfu) || dfu <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter valid drainage fixture units.", "OK");
                return;
            }

            string drainType = DrainTypePicker.SelectedItem?.ToString() ?? "";
            int slopeIndex = SlopePicker.SelectedIndex;

            string drainSize;
            double capacity;
            string ventSize;

            if (drainType == "Horizontal Branch")
            {
                if (dfu <= 3)
                {
                    drainSize = "1-1/2\"";
                    capacity = 3;
                    ventSize = "1-1/4\"";
                }
                else if (dfu <= 6)
                {
                    drainSize = "2\"";
                    capacity = 6;
                    ventSize = "1-1/2\"";
                }
                else if (dfu <= 12)
                {
                    drainSize = "2-1/2\"";
                    capacity = 12;
                    ventSize = "2\"";
                }
                else if (dfu <= 20)
                {
                    drainSize = "3\"";
                    capacity = 20;
                    ventSize = "2\"";
                }
                else if (dfu <= 160)
                {
                    drainSize = "4\"";
                    capacity = 160;
                    ventSize = "3\"";
                }
                else
                {
                    drainSize = "6\" or larger";
                    capacity = 620;
                    ventSize = "4\"";
                }
            }
            else if (drainType == "Vertical Stack")
            {
                if (dfu <= 4)
                {
                    drainSize = "1-1/2\"";
                    capacity = 4;
                    ventSize = "1-1/4\"";
                }
                else if (dfu <= 10)
                {
                    drainSize = "2\"";
                    capacity = 10;
                    ventSize = "1-1/2\"";
                }
                else if (dfu <= 30)
                {
                    drainSize = "2-1/2\"";
                    capacity = 30;
                    ventSize = "2\"";
                }
                else if (dfu <= 240)
                {
                    drainSize = "3\"";
                    capacity = 240;
                    ventSize = "2\"";
                }
                else if (dfu <= 540)
                {
                    drainSize = "4\"";
                    capacity = 540;
                    ventSize = "3\"";
                }
                else
                {
                    drainSize = "6\" or larger";
                    capacity = 1400;
                    ventSize = "4\"";
                }
            }
            else
            {
                if (dfu <= 21)
                {
                    drainSize = "2\"";
                    capacity = 21;
                    ventSize = "1-1/2\"";
                }
                else if (dfu <= 24)
                {
                    drainSize = "2-1/2\"";
                    capacity = 24;
                    ventSize = "2\"";
                }
                else if (dfu <= 42)
                {
                    drainSize = "3\"";
                    capacity = 42;
                    ventSize = "2\"";
                }
                else if (dfu <= 216)
                {
                    drainSize = "4\"";
                    capacity = 216;
                    ventSize = "3\"";
                }
                else if (dfu <= 480)
                {
                    drainSize = "5\"";
                    capacity = 480;
                    ventSize = "4\"";
                }
                else
                {
                    drainSize = "6\" or larger";
                    capacity = 840;
                    ventSize = "4\"";
                }
            }

            if (slopeIndex == 0)
            {
                capacity *= 0.85;
            }
            else if (slopeIndex == 2)
            {
                capacity *= 1.15;
            }

            DrainSizeLabel.Text = $"Recommended Drain Size: {drainSize}";
            VentSizeLabel.Text = $"Recommended Vent Size: {ventSize}";
            DrainCapacityLabel.Text = $"Drain Capacity: {capacity:F0} DFU (you have {dfu:F0} DFU)";

            if (dfu > capacity * 0.9)
            {
                await DisplayAlert("Capacity Warning", 
                    "Warning: DFU is close to or exceeds drain capacity. Consider using larger pipe.", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Calculation Error: {ex.Message}", "OK");
        }
    }

    private async void CalculateFlowRateButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!double.TryParse(FlowPressureEntry.Text, out double pressure) || pressure <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid pressure.", "OK");
                return;
            }

            if (!double.TryParse(FlowPipeLengthEntry.Text, out double length) || length <= 0)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid pipe length.", "OK");
                return;
            }

            string sizeStr = FlowPipeDiameterPicker.SelectedItem?.ToString() ?? "";
            
            double diameter = sizeStr switch
            {
                "1/2\"" => 0.5,
                "3/4\"" => 0.75,
                "1\"" => 1.0,
                "1-1/4\"" => 1.25,
                "1-1/2\"" => 1.5,
                "2\"" => 2.0,
                "3\"" => 3.0,
                "4\"" => 4.0,
                _ => 1.0
            };

            double area = Math.PI * Math.Pow(diameter / 2, 2) / 144.0;

            double frictionLoss = length / 100.0 * (diameter <= 1 ? 5.0 : diameter <= 2 ? 2.5 : 1.5);
            double effectivePressure = Math.Max(0, pressure - frictionLoss);

            double velocity = Math.Sqrt(2 * 32.2 * effectivePressure * 144 / 62.4);

            double flowRate = velocity * area * 60 * 7.48;

            double pressureLoss = frictionLoss;

            CalculatedFlowRateLabel.Text = $"Flow Rate: {flowRate:F1} GPM";
            FlowVelocityLabel.Text = $"Flow Velocity: {velocity:F1} ft/s";
            FlowPressureLossLabel.Text = $"Pressure Loss: {pressureLoss:F1} PSI";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Calculation Error: {ex.Message}", "OK");
        }
    }

    private async void CopyResultsButton_Clicked(object sender, EventArgs e)
    {
        string results = "";

        if (FlowRateLabel.Text != "Required Flow Rate: 0 GPM")
        {
            results += $"{FlowRateLabel.Text}\n{RecommendedPipeSizeLabel.Text}\n{VelocityLabel.Text}\n{PressureLossLabel.Text}\n";
        }

        if (DrainSizeLabel.Text != "Recommended Drain Size: Not calculated")
        {
            results += $"{DrainSizeLabel.Text}\n{VentSizeLabel.Text}\n{DrainCapacityLabel.Text}\n";
        }

        if (CalculatedFlowRateLabel.Text != "Flow Rate: 0 GPM")
        {
            results += $"{CalculatedFlowRateLabel.Text}\n{FlowVelocityLabel.Text}\n{FlowPressureLossLabel.Text}";
        }

        if (!string.IsNullOrWhiteSpace(results))
        {
            await Clipboard.SetTextAsync(results.Trim());
            await DisplayAlert("Copied", "Results copied to clipboard!", "OK");
        }
        else
        {
            await DisplayAlert("No Results", "No results to copy. Please perform a calculation first.", "OK");
        }
    }
}
