using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ConstructionCalculator.WPF;

public enum RampLandingType
{
    Straight,
    RightAngle,
    FullReturn
}

public partial class RampCalculatorWindow : Window
{
    public RampCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Measurement totalRise = Measurement.Parse(TotalRiseTextBox.Text);
            if (!double.TryParse(SlopeRatioTextBox.Text, out double slopeRatio) || slopeRatio < 1)
            {
                MessageBox.Show("Please enter a valid slope ratio (e.g., 12 for 1:12).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double totalRiseInches = totalRise.ToTotalInches();
            double requiredRunInches = totalRiseInches * slopeRatio;
            
            Measurement requiredRun = Measurement.FromDecimalInches(requiredRunInches);
            RequiredRunLabel.Text = $"Required Run: {requiredRun.ToFractionString()}";

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
                ComplianceLabel.Foreground = Brushes.Green;
            }
            else if (slopeRatio >= 10.0)
            {
                ComplianceLabel.Foreground = Brushes.Orange;
            }
            else
            {
                ComplianceLabel.Foreground = Brushes.Red;
            }

            if (IncludeLandingsCheckBox.IsChecked == true)
            {
                CalculateLandingConfiguration(totalRiseInches, requiredRunInches, slopeRatio, landingsRequired);
            }

            GenerateVisualDiagram(totalRise, requiredRun, slopeRatio, landingsRequired);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateLandingConfiguration(double totalRiseInches, double requiredRunInches, double slopeRatio, int landingsRequired)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LandingDepthTextBox.Text) || string.IsNullOrWhiteSpace(RampWidthTextBox.Text))
            {
                OverallLengthLabel.Text = "Enter landing depth and ramp width";
                OverallWidthLabel.Text = "";
                return;
            }

            Measurement landingDepth = Measurement.Parse(LandingDepthTextBox.Text);
            double landingDepthInches = landingDepth.ToTotalInches();
            
            Measurement rampWidth = Measurement.Parse(RampWidthTextBox.Text);
            double rampWidthInches = rampWidth.ToTotalInches();

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

            RampLandingType landingType = (RampLandingType)LandingTypeComboBox.SelectedIndex;
            
            if (landingType == RampLandingType.FullReturn)
            {
                if (landingDepthInches < rampWidthInches * 2)
                {
                    codeWarnings.AppendLine("⚠ U-shaped landing should be at least 2x ramp width for code compliance");
                }
            }
            
            if (codeWarnings.Length > 0)
            {
                MessageBox.Show(codeWarnings.ToString().Trim(),
                    "Code Requirements", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    overallLength = Math.Max(runPerSegment, runPerSegment);
                    overallWidth = landingDepthInches + rampWidthInches;
                    break;
                    
                default:
                    overallLength = requiredRunInches + (landingsRequired * landingDepthInches);
                    overallWidth = rampWidthInches;
                    break;
            }
            
            Measurement overallLengthMeasurement = Measurement.FromDecimalInches(overallLength);
            Measurement overallWidthMeasurement = Measurement.FromDecimalInches(overallWidth);
            
            OverallLengthLabel.Text = $"Overall Length: {overallLengthMeasurement.ToFractionString()}";
            OverallWidthLabel.Text = $"Overall Width: {overallWidthMeasurement.ToFractionString()}";
            
            CheckSpaceFit();
        }
        catch (Exception)
        {
            OverallLengthLabel.Text = "Invalid landing depth or ramp width";
            OverallWidthLabel.Text = "";
        }
    }

    private void GenerateVisualDiagram(Measurement totalRise, Measurement requiredRun, double slopeRatio, int landingsRequired)
    {
        StringBuilder diagram = new StringBuilder();

        diagram.AppendLine($"Total Rise: {totalRise.ToFractionString()}");
        diagram.AppendLine($"Slope Ratio: 1:{slopeRatio}");
        diagram.AppendLine($"Required Run: {requiredRun.ToFractionString()} ({requiredRun.ToTotalInches():F2}\")");
        diagram.AppendLine($"Landings Required: {landingsRequired}");

        if (IncludeLandingsCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(LandingDepthTextBox.Text))
        {
            try
            {
                Measurement landingDepth = Measurement.Parse(LandingDepthTextBox.Text);
                diagram.AppendLine();
                diagram.AppendLine("WITH LANDINGS:");
                diagram.AppendLine($"Landing Depth: {landingDepth.ToFractionString()}");
                diagram.AppendLine($"Landing Type: {LandingTypeComboBox.Text}");
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
        diagram.AppendLine(" /  │ " + totalRise.ToFractionString());
        diagram.AppendLine("/   │");
        diagram.AppendLine("────┘");
        diagram.AppendLine("  " + requiredRun.ToFractionString());

        if (landingsRequired > 0)
        {
            diagram.AppendLine();
            diagram.AppendLine($"Note: {landingsRequired} landing(s) required every 30' per ADA");
        }

        VisualDiagramTextBox.Text = diagram.ToString();
    }

    private void IncludeLandingsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        bool showLandings = IncludeLandingsCheckBox.IsChecked == true;
        LandingTypePanel.Visibility = showLandings ? Visibility.Visible : Visibility.Collapsed;
        LandingDepthPanel.Visibility = showLandings ? Visibility.Visible : Visibility.Collapsed;
        OverallDimensionsBorder.Visibility = showLandings ? Visibility.Visible : Visibility.Collapsed;

        if (showLandings && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }
    
    private void LandingTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }

    private void LandingDepthTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (IncludeLandingsCheckBox.IsChecked == true)
        {
            CalculateButton_Click(sender, e);
        }
    }
    
    private void SpaceConstraint_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        CheckSpaceFit();
    }
    
    private void CheckSpaceFit()
    {
        if (string.IsNullOrWhiteSpace(AvailableLengthTextBox.Text) && string.IsNullOrWhiteSpace(AvailableWidthTextBox.Text))
        {
            SpaceFitBorder.Visibility = Visibility.Collapsed;
            return;
        }
        
        if (IncludeLandingsCheckBox.IsChecked != true || string.IsNullOrWhiteSpace(OverallLengthLabel.Text))
        {
            SpaceFitBorder.Visibility = Visibility.Collapsed;
            return;
        }
        
        try
        {
            double? availableLength = null;
            double? availableWidth = null;
            
            if (!string.IsNullOrWhiteSpace(AvailableLengthTextBox.Text))
            {
                Measurement lengthMeasurement = Measurement.Parse(AvailableLengthTextBox.Text);
                availableLength = lengthMeasurement.ToTotalInches();
            }
            
            if (!string.IsNullOrWhiteSpace(AvailableWidthTextBox.Text))
            {
                Measurement widthMeasurement = Measurement.Parse(AvailableWidthTextBox.Text);
                availableWidth = widthMeasurement.ToTotalInches();
            }
            
            string lengthText = OverallLengthLabel.Text.Replace("Overall Length: ", "");
            string widthText = OverallWidthLabel.Text.Replace("Overall Width: ", "");
            
            Measurement overallLength = Measurement.Parse(lengthText);
            Measurement overallWidth = Measurement.Parse(widthText);
            
            double requiredLength = overallLength.ToTotalInches();
            double requiredWidth = overallWidth.ToTotalInches();
            
            bool fits = true;
            StringBuilder fitMessage = new StringBuilder();
            
            if (availableLength.HasValue && requiredLength > availableLength.Value)
            {
                fits = false;
                fitMessage.AppendLine($"⚠ Length exceeds available space by {Measurement.FromDecimalInches(requiredLength - availableLength.Value).ToFractionString()}");
            }
            
            if (availableWidth.HasValue && requiredWidth > availableWidth.Value)
            {
                fits = false;
                fitMessage.AppendLine($"⚠ Width exceeds available space by {Measurement.FromDecimalInches(requiredWidth - availableWidth.Value).ToFractionString()}");
            }
            
            if (fits)
            {
                fitMessage.AppendLine("✓ Configuration fits in available space");
                SpaceFitLabel.Foreground = Brushes.Green;
            }
            else
            {
                fitMessage.AppendLine("\nTry different landing type or adjust slope ratio");
                SpaceFitLabel.Foreground = Brushes.Red;
            }
            
            SpaceFitLabel.Text = fitMessage.ToString().Trim();
            SpaceFitBorder.Visibility = Visibility.Visible;
        }
        catch
        {
            SpaceFitBorder.Visibility = Visibility.Collapsed;
        }
    }
}
