using System.Text;

namespace ConstructionCalculatorMAUI.Pages.Calculators.Geometry;

public enum RampLandingType
{
    Straight,
    RightAngle,
    FullReturn
}

public partial class RampCalculatorPage : ContentPage
{
    public RampCalculatorPage()
    {
        InitializeComponent();
    }

    private async void CalculateButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Parse total rise - for MAUI, we'll accept decimal feet for simplicity
            if (!double.TryParse(TotalRiseEntry.Text?.Replace("\"", "").Replace("'", ""), out double totalRiseFeet))
            {
                await DisplayAlert("Invalid Input", "Please enter a valid total rise (in feet or inches).", "OK");
                return;
            }
            
            double totalRiseInches = totalRiseFeet * 12; // Convert to inches
            
            if (!double.TryParse(SlopeRatioEntry.Text, out double slopeRatio) || slopeRatio < 1)
            {
                await DisplayAlert("Invalid Input", "Please enter a valid slope ratio (e.g., 12 for 1:12).", "OK");
                return;
            }

            double requiredRunInches = totalRiseInches * slopeRatio;
            double requiredRunFeet = requiredRunInches / 12.0;
            
            RequiredRunLabel.Text = $"Required Run: {requiredRunFeet:F2}'";

            int landingsRequired = (int)Math.Ceiling(requiredRunInches / 360.0) - 1;
            if (landingsRequired < 0) landingsRequired = 0;
            
            LandingsRequiredLabel.Text = $"Landings Required: {landingsRequired} (ADA: every 30')";

            StringBuilder complianceMessage = new StringBuilder();
            bool slopeCompliant = false;

            if (slopeRatio >= 12.0)
            {
                complianceMessage.AppendLine("✓ Slope compliant with ADA (1:12 maximum)");
                slopeCompliant = true;
            }
            else if (slopeRatio >= 10.0)
            {
                complianceMessage.AppendLine("⚠ Slope steeper than ADA standard but may be acceptable for short distances");
            }
            else
            {
                complianceMessage.AppendLine("⚠ Slope too steep for ADA compliance");
            }

            if (totalRiseInches > 30.0 && landingsRequired == 0)
            {
                complianceMessage.AppendLine("⚠ Rise exceeds 30\" - landing required per ADA");
            }

            if (!slopeCompliant)
            {
                complianceMessage.AppendLine("Check local building codes");
            }

            ComplianceLabel.Text = complianceMessage.ToString().Trim();

            if (slopeCompliant)
            {
                ComplianceLabel.TextColor = Colors.Green;
            }
            else if (slopeRatio >= 10.0)
            {
                ComplianceLabel.TextColor = Colors.Orange;
            }
            else
            {
                ComplianceLabel.TextColor = Colors.Red;
            }

            if (IncludeLandingsCheckBox.IsChecked)
            {
                CalculateLandingConfiguration(totalRiseInches, requiredRunInches, slopeRatio, landingsRequired);
            }

            GenerateVisualDiagram(totalRiseFeet, requiredRunFeet, slopeRatio, landingsRequired);
            
            if (IncludeLandingsCheckBox.IsChecked)
            {
                CheckSpaceFit();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void CalculateLandingConfiguration(double totalRiseInches, double requiredRunInches, double slopeRatio, int landingsRequired)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LandingDepthEntry.Text) || string.IsNullOrWhiteSpace(RampWidthEntry.Text))
            {
                OverallLengthLabel.Text = "Enter landing depth and ramp width";
                OverallWidthLabel.Text = "";
                return;
            }

            if (!double.TryParse(LandingDepthEntry.Text?.Replace("\"", "").Replace("'", ""), out double landingDepthFeet))
            {
                OverallLengthLabel.Text = "Invalid landing depth";
                return;
            }
            double landingDepthInches = landingDepthFeet * 12;
            
            if (!double.TryParse(RampWidthEntry.Text?.Replace("\"", "").Replace("'", ""), out double rampWidthFeet))
            {
                OverallLengthLabel.Text = "Invalid ramp width";
                return;
            }
            double rampWidthInches = rampWidthFeet * 12;

            StringBuilder codeWarnings = new StringBuilder();
            
            if (landingDepthInches < 60.0)
            {
                codeWarnings.AppendLine("⚠ Landing depth should be at least 60\" per ADA");
            }
            
            if (landingDepthInches < rampWidthInches)
            {
                codeWarnings.AppendLine("⚠ Landing depth must be at least as wide as ramp width per code");
            }
            
            if (rampWidthInches < 36.0)
            {
                codeWarnings.AppendLine("⚠ Ramp width should be at least 36\" per ADA");
            }

            RampLandingType landingType = (RampLandingType)LandingTypePicker.SelectedIndex;
            
            if (landingType == RampLandingType.FullReturn)
            {
                if (landingDepthInches < rampWidthInches * 2)
                {
                    codeWarnings.AppendLine("⚠ U-shaped landing should be at least 2x ramp width for code compliance");
                }
            }
            
            if (codeWarnings.Length > 0)
            {
                await DisplayAlert("Code Requirements", codeWarnings.ToString().Trim(), "OK");
            }

            int segments = landingsRequired + 1;
            double runPerSegment = requiredRunInches / segments;
            
            double overallLength, overallWidth;
            
            switch (landingType)
            {
                case RampLandingType.Straight:
                    overallLength = requiredRunInches + (landingsRequired * landingDepthInches);
                    overallWidth = rampWidthInches;
                    break;
                    
                case RampLandingType.RightAngle:
                    if (landingsRequired == 1)
                    {
                        overallLength = runPerSegment + rampWidthInches;
                        overallWidth = Math.Max(landingDepthInches, rampWidthInches) + runPerSegment;
                    }
                    else
                    {
                        overallLength = runPerSegment * Math.Ceiling(segments / 2.0) + rampWidthInches;
                        overallWidth = runPerSegment * Math.Floor(segments / 2.0) + landingDepthInches;
                    }
                    break;
                    
                case RampLandingType.FullReturn:
                    overallLength = runPerSegment;
                    overallWidth = (2 * rampWidthInches) + landingDepthInches;
                    break;
                    
                default:
                    overallLength = requiredRunInches + (landingsRequired * landingDepthInches);
                    overallWidth = rampWidthInches;
                    break;
            }
            
            double overallLengthFeet = overallLength / 12.0;
            double overallWidthFeet = overallWidth / 12.0;
            
            OverallLengthLabel.Text = $"Overall Length: {overallLengthFeet:F2}'";
            OverallWidthLabel.Text = $"Overall Width: {overallWidthFeet:F2}'";
            
            CheckSpaceFit();
        }
        catch (Exception)
        {
            OverallLengthLabel.Text = "Invalid landing depth or ramp width";
            OverallWidthLabel.Text = "";
        }
    }

    private void GenerateVisualDiagram(double totalRiseFeet, double requiredRunFeet, double slopeRatio, int landingsRequired)
    {
        StringBuilder diagram = new StringBuilder();

        diagram.AppendLine($"Total Rise: {totalRiseFeet:F2}'");
        diagram.AppendLine($"Slope Ratio: 1:{slopeRatio}");
        diagram.AppendLine($"Required Run: {requiredRunFeet:F2}' ({totalRiseFeet * 12:F2}\")");
        diagram.AppendLine($"Landings Required: {landingsRequired}");

        if (IncludeLandingsCheckBox.IsChecked && !string.IsNullOrWhiteSpace(LandingDepthEntry.Text))
        {
            try
            {
                if (double.TryParse(LandingDepthEntry.Text?.Replace("\"", "").Replace("'", ""), out double landingDepthFeet))
                {
                    diagram.AppendLine();
                    diagram.AppendLine("WITH LANDINGS:");
                    diagram.AppendLine($"Landing Depth: {landingDepthFeet:F2}'");
                    diagram.AppendLine($"Landing Type: {LandingTypePicker.SelectedItem}");
                }
            }
            catch
            {
            }
        }

        diagram.AppendLine();
        diagram.AppendLine("Ramp Profile:");
        diagram.AppendLine();
        diagram.AppendLine("    ┌─────────────────────────────────");
        diagram.AppendLine("   /│");
        diagram.AppendLine("  / │");
        diagram.AppendLine($" /  │ {totalRiseFeet:F2}'");
        diagram.AppendLine("/   │");
        diagram.AppendLine("────┘");
        diagram.AppendLine($"  {requiredRunFeet:F2}'");

        if (landingsRequired > 0)
        {
            diagram.AppendLine();
            diagram.AppendLine($"Note: {landingsRequired} landing(s) required every 30' per ADA");
        }

        VisualDiagramLabel.Text = diagram.ToString();
    }

    private void IncludeLandingsCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool showLandings = IncludeLandingsCheckBox.IsChecked;
        LandingTypePanel.IsVisible = showLandings;
        OverallDimensionsFrame.IsVisible = showLandings;

        if (showLandings && !string.IsNullOrWhiteSpace(TotalRiseEntry.Text))
        {
            CalculateButton_Clicked(sender, e);
        }
    }
    
    private void LandingTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked && !string.IsNullOrWhiteSpace(TotalRiseEntry.Text))
        {
            CalculateButton_Clicked(sender, e);
        }
    }
    
    private void LandingDepth_Unfocused(object sender, FocusEventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked && !string.IsNullOrWhiteSpace(TotalRiseEntry.Text))
        {
            CalculateButton_Clicked(sender, e);
        }
    }
    
    private void AvailableLength_Unfocused(object sender, FocusEventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked)
        {
            CheckSpaceFit();
        }
    }
    
    private void AvailableWidth_Unfocused(object sender, FocusEventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked)
        {
            CheckSpaceFit();
        }
    }
    
    private void CheckSpaceFit()
    {
        if (string.IsNullOrWhiteSpace(AvailableLengthEntry.Text) && string.IsNullOrWhiteSpace(AvailableWidthEntry.Text))
        {
            SpaceFitFrame.IsVisible = false;
            return;
        }
        
        if (!IncludeLandingsCheckBox.IsChecked || string.IsNullOrWhiteSpace(OverallLengthLabel.Text))
        {
            SpaceFitFrame.IsVisible = false;
            return;
        }
        
        try
        {
            double? availableLength = null;
            double? availableWidth = null;
            
            if (!string.IsNullOrWhiteSpace(AvailableLengthEntry.Text))
            {
                if (double.TryParse(AvailableLengthEntry.Text?.Replace("\"", "").Replace("'", ""), out double lengthFeet))
                {
                    availableLength = lengthFeet * 12;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(AvailableWidthEntry.Text))
            {
                if (double.TryParse(AvailableWidthEntry.Text?.Replace("\"", "").Replace("'", ""), out double widthFeet))
                {
                    availableWidth = widthFeet * 12;
                }
            }
            
            string lengthText = OverallLengthLabel.Text.Replace("Overall Length: ", "").Replace("'", "");
            string widthText = OverallWidthLabel.Text.Replace("Overall Width: ", "").Replace("'", "");
            
            if (!double.TryParse(lengthText, out double requiredLengthFeet) || 
                !double.TryParse(widthText, out double requiredWidthFeet))
            {
                SpaceFitFrame.IsVisible = false;
                return;
            }
            
            double requiredLength = requiredLengthFeet * 12;
            double requiredWidth = requiredWidthFeet * 12;
            
            bool fits = true;
            StringBuilder fitMessage = new StringBuilder();
            
            if (availableLength.HasValue && requiredLength > availableLength.Value)
            {
                fits = false;
                double excessFeet = (requiredLength - availableLength.Value) / 12.0;
                fitMessage.AppendLine($"⚠ Length exceeds available space by {excessFeet:F2}'");
            }
            
            if (availableWidth.HasValue && requiredWidth > availableWidth.Value)
            {
                fits = false;
                double excessFeet = (requiredWidth - availableWidth.Value) / 12.0;
                fitMessage.AppendLine($"⚠ Width exceeds available space by {excessFeet:F2}'");
            }
            
            if (fits)
            {
                fitMessage.AppendLine("✓ Configuration fits in available space");
                SpaceFitLabel.TextColor = Colors.Green;
            }
            else
            {
                fitMessage.AppendLine("\nTry different landing type or adjust slope ratio");
                SpaceFitLabel.TextColor = Colors.Red;
            }
            
            SpaceFitLabel.Text = fitMessage.ToString().Trim();
            SpaceFitFrame.IsVisible = true;
        }
        catch
        {
            SpaceFitFrame.IsVisible = false;
        }
    }
}
