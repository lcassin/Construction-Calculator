using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ConstructionCalculator.WPF;

public enum LandingType
{
    Straight,
    RightAngle,
    FullReturn
}

public partial class StairCalculatorWindow : Window
{
    public StairCalculatorWindow()
    {
        InitializeComponent();
    }

    private void AutoCalculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Measurement totalRise = Measurement.Parse(TotalRiseTextBox.Text);
            double totalRiseInches = totalRise.ToTotalInches();

            double targetRiserHeight = 7.375;
            int optimalSteps = (int)Math.Round(totalRiseInches / targetRiserHeight);

            if (optimalSteps < 2) optimalSteps = 2;

            double resultingRiserHeight = totalRiseInches / optimalSteps;

            if (resultingRiserHeight > 7.75 && totalRiseInches > 15.5)
            {
                optimalSteps++;
            }
            else if (resultingRiserHeight < 7.0 && optimalSteps > 2)
            {
                optimalSteps--;
            }

            NumberOfStepsTextBox.Text = optimalSteps.ToString();

            CalculateButton_Click(sender, e);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Measurement totalRise = Measurement.Parse(TotalRiseTextBox.Text);
            if (!int.TryParse(NumberOfStepsTextBox.Text, out int numberOfSteps) || numberOfSteps < 1)
            {
                MessageBox.Show("Please enter a valid number of steps (1 or greater).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double totalRiseInches = totalRise.ToTotalInches();
            double riserHeightInches = totalRiseInches / numberOfSteps;

            Measurement riserHeight = Measurement.FromDecimalInches(riserHeightInches);
            RiserHeightLabel.Text = $"Riser Height: {riserHeight.ToFractionString()}";

            double treadDepthInches = 25.0 - (2.0 * riserHeightInches);
            Measurement treadDepth = Measurement.FromDecimalInches(treadDepthInches);
            TreadDepthLabel.Text = $"Tread Depth: {treadDepth.ToFractionString()}";

            double totalRunInches = (numberOfSteps - 1) * treadDepthInches;
            Measurement totalRun = Measurement.FromDecimalInches(totalRunInches);
            RunWidthLabel.Text = $"Total Run Width: {totalRun.ToFractionString()}";

            if (IncludeLandingCheckBox.IsChecked == true)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(LandingDepthTextBox.Text) || string.IsNullOrWhiteSpace(StairWidthTextBox.Text))
                    {
                        StepsAfterLandingLabel.Text = "Enter landing depth and stair width";
                        OverallLengthLabel.Text = "";
                        OverallWidthLabel.Text = "";
                    }
                    else
                    {
                        Measurement landingDepth = Measurement.Parse(LandingDepthTextBox.Text);
                        double landingDepthInches = landingDepth.ToTotalInches();
                        
                        Measurement stairWidth = Measurement.Parse(StairWidthTextBox.Text);
                        double stairWidthInches = stairWidth.ToTotalInches();

                        if (!int.TryParse(StepsBeforeLandingTextBox.Text, out int stepsBeforeLanding) || stepsBeforeLanding < 1)
                        {
                            StepsAfterLandingLabel.Text = "Invalid steps before landing";
                            OverallLengthLabel.Text = "";
                            OverallWidthLabel.Text = "";
                        }
                        else if (stepsBeforeLanding >= numberOfSteps)
                        {
                            StepsAfterLandingLabel.Text = "Steps before landing must be less than total steps";
                            OverallLengthLabel.Text = "";
                            OverallWidthLabel.Text = "";
                        }
                        else
                        {
                            int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;
                            double flightARun = (stepsBeforeLanding - 1) * treadDepthInches;
                            double flightBRun = (stepsAfterLanding - 1) * treadDepthInches;

                            StepsAfterLandingLabel.Text = $"Steps After Landing: {stepsAfterLanding}";
                            
                            StringBuilder codeWarnings = new StringBuilder();
                            
                            if (landingDepthInches < 36.0)
                            {
                                codeWarnings.AppendLine("⚠ Landing depth should be at least 36\" per IBC/IRC");
                            }
                            
                            if (landingDepthInches < stairWidthInches)
                            {
                                codeWarnings.AppendLine("⚠ Landing depth must be at least as wide as stair width per code");
                            }
                            
                            double riseBeforeLanding = riserHeightInches * stepsBeforeLanding;
                            double riseAfterLanding = riserHeightInches * stepsAfterLanding;
                            
                            if (riseBeforeLanding > 144.0)
                            {
                                codeWarnings.AppendLine($"⚠ Rise before landing ({riseBeforeLanding:F1}\") exceeds 12' maximum per code");
                            }
                            
                            if (riseAfterLanding > 144.0)
                            {
                                codeWarnings.AppendLine($"⚠ Rise after landing ({riseAfterLanding:F1}\") exceeds 12' maximum per code");
                            }
                            
                            LandingType landingType = (LandingType)LandingTypeComboBox.SelectedIndex;
                            
                            if (landingType == LandingType.FullReturn)
                            {
                                if (landingDepthInches < stairWidthInches * 2)
                                {
                                    codeWarnings.AppendLine("⚠ U-shaped landing should be at least 2x stair width for code compliance");
                                }
                            }
                            
                            if (codeWarnings.Length > 0)
                            {
                                MessageBox.Show(codeWarnings.ToString().Trim(),
                                    "Code Requirements", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            
                            double overallLength, overallWidth;
                            
                            switch (landingType)
                            {
                                case LandingType.Straight:
                                    overallLength = flightARun + landingDepthInches + flightBRun;
                                    overallWidth = stairWidthInches;
                                    break;
                                    
                                case LandingType.RightAngle:
                                    overallLength = flightARun + stairWidthInches;
                                    overallWidth = Math.Max(landingDepthInches, stairWidthInches) + flightBRun;
                                    break;
                                    
                                case LandingType.FullReturn:
                                    overallLength = Math.Max(flightARun, flightBRun);
                                    overallWidth = (2 * stairWidthInches) + landingDepthInches;
                                    break;
                                    
                                default:
                                    overallLength = flightARun + landingDepthInches + flightBRun;
                                    overallWidth = stairWidthInches;
                                    break;
                            }
                            
                            Measurement overallLengthMeasurement = Measurement.FromDecimalInches(overallLength);
                            Measurement overallWidthMeasurement = Measurement.FromDecimalInches(overallWidth);
                            
                            OverallLengthLabel.Text = $"Overall Length: {overallLengthMeasurement.ToFractionString()}";
                            OverallWidthLabel.Text = $"Overall Width: {overallWidthMeasurement.ToFractionString()}";
                            
                            CheckSpaceFit();
                        }
                    }
                }
                catch (Exception)
                {
                    StepsAfterLandingLabel.Text = "Invalid landing depth or stair width";
                    OverallLengthLabel.Text = "";
                    OverallWidthLabel.Text = "";
                }
            }

            StringBuilder complianceMessage = new StringBuilder();
            bool riserCompliant = false;
            bool treadCompliant = false;

            if (riserHeightInches >= 7.0 && riserHeightInches <= 7.75)
            {
                complianceMessage.AppendLine("✓ Riser height within typical residential code (7-7.75\")");
                riserCompliant = true;
            }
            else if (riserHeightInches >= 6.0 && riserHeightInches <= 8.0)
            {
                complianceMessage.AppendLine("⚠ Riser height outside typical range but may be acceptable");
            }
            else
            {
                complianceMessage.AppendLine("⚠ Riser height outside typical code range");
            }

            if (treadDepthInches >= 10.0 && treadDepthInches <= 11.0)
            {
                complianceMessage.AppendLine("✓ Tread depth within typical code (10-11\")");
                treadCompliant = true;
            }
            else if (treadDepthInches >= 9.0 && treadDepthInches <= 12.0)
            {
                complianceMessage.AppendLine("⚠ Tread depth outside typical range but may be acceptable");
            }
            else
            {
                complianceMessage.AppendLine("⚠ Tread depth outside typical code range");
            }

            if (!riserCompliant || !treadCompliant)
            {
                complianceMessage.AppendLine("Check local building codes");
            }

            ComplianceLabel.Text = complianceMessage.ToString().Trim();

            if (riserCompliant && treadCompliant)
            {
                ComplianceLabel.Foreground = Brushes.Green;
            }
            else if ((riserHeightInches >= 6.0 && riserHeightInches <= 8.0) &&
                     (treadDepthInches >= 9.0 && treadDepthInches <= 12.0))
            {
                ComplianceLabel.Foreground = Brushes.Orange;
            }
            else
            {
                ComplianceLabel.Foreground = Brushes.Red;
            }

            GenerateVisualDiagram(totalRise, numberOfSteps, riserHeight, treadDepth);
            
            if (IncludeLandingCheckBox.IsChecked == true)
            {
                CheckSpaceFit();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void GenerateVisualDiagram(Measurement totalRise, int numberOfSteps, Measurement riserHeight, Measurement treadDepth)
    {
        StringBuilder diagram = new StringBuilder();

        diagram.AppendLine($"Total Rise: {totalRise.ToFractionString()}");
        diagram.AppendLine($"Number of Steps: {numberOfSteps}");
        diagram.AppendLine($"Riser Height: {riserHeight.ToFractionString()} ({riserHeight.ToTotalInches():F2}\")");
        diagram.AppendLine($"Tread Depth: {treadDepth.ToFractionString()} ({treadDepth.ToTotalInches():F2}\")");

        if (IncludeLandingCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(LandingDepthTextBox.Text))
        {
            try
            {
                Measurement landingDepth = Measurement.Parse(LandingDepthTextBox.Text);
                if (int.TryParse(StepsBeforeLandingTextBox.Text, out int stepsBeforeLanding))
                {
                    int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;

                    diagram.AppendLine();
                    diagram.AppendLine("WITH LANDING:");
                    diagram.AppendLine($"Steps Before Landing: {stepsBeforeLanding}");
                    diagram.AppendLine($"Landing Depth: {landingDepth.ToFractionString()}");
                    diagram.AppendLine($"Steps After Landing: {stepsAfterLanding}");
                }
            }
            catch
            {
            }
        }

        diagram.AppendLine();
        diagram.AppendLine("Stair Profile:");
        diagram.AppendLine();

        if (IncludeLandingCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(LandingDepthTextBox.Text))
        {
            try
            {
                if (int.TryParse(StepsBeforeLandingTextBox.Text, out int stepsBeforeLanding))
                {
                    int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;

                    int stepsToShowBefore = Math.Min(3, stepsBeforeLanding);
                    for (int i = 0; i < stepsToShowBefore; i++)
                    {
                        diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                        if (i < stepsToShowBefore - 1)
                        {
                            diagram.AppendLine($"└──────────┐");
                        }
                        else
                        {
                            diagram.AppendLine($"└──────────");
                        }
                    }

                    if (stepsBeforeLanding > stepsToShowBefore)
                    {
                        diagram.AppendLine($"  (... {stepsBeforeLanding - stepsToShowBefore} more steps)");
                    }

                    diagram.AppendLine();
                    diagram.AppendLine($"  ═══════════════════ LANDING ═══════════════════");
                    diagram.AppendLine();

                    int stepsToShowAfter = Math.Min(3, stepsAfterLanding);
                    for (int i = 0; i < stepsToShowAfter; i++)
                    {
                        diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                        if (i < stepsToShowAfter - 1)
                        {
                            diagram.AppendLine($"└──────────┐");
                        }
                        else
                        {
                            diagram.AppendLine($"└──────────");
                        }
                    }

                    if (stepsAfterLanding > stepsToShowAfter)
                    {
                        diagram.AppendLine($"  (... {stepsAfterLanding - stepsToShowAfter} more steps)");
                    }
                }
            }
            catch
            {
                int stepsToShow = Math.Min(4, numberOfSteps);
                for (int i = 0; i < stepsToShow; i++)
                {
                    diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                    if (i < stepsToShow - 1)
                    {
                        diagram.AppendLine($"└──────────┐");
                    }
                    else
                    {
                        diagram.AppendLine($"└──────────");
                    }
                }
                if (numberOfSteps > stepsToShow)
                {
                    diagram.AppendLine();
                    diagram.AppendLine($"(showing {stepsToShow} of {numberOfSteps} total steps)");
                }
            }
        }
        else
        {
            int stepsToShow = Math.Min(4, numberOfSteps);
            for (int i = 0; i < stepsToShow; i++)
            {
                diagram.AppendLine($"│          │ {riserHeight.ToFractionString()}");
                if (i < stepsToShow - 1)
                {
                    diagram.AppendLine($"└──────────┐");
                }
                else
                {
                    diagram.AppendLine($"└──────────");
                }
            }

            diagram.AppendLine($"  {treadDepth.ToFractionString()}");

            if (numberOfSteps > stepsToShow)
            {
                diagram.AppendLine();
                diagram.AppendLine($"(showing {stepsToShow} of {numberOfSteps} total steps)");
            }
        }

        VisualDiagramTextBox.Text = diagram.ToString();
    }

    private void IncludeLandingCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        bool showLanding = IncludeLandingCheckBox.IsChecked == true;
        LandingTypePanel.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        LandingDepthPanel.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        StepsBeforeLandingPanel.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        StepsAfterLandingBorder.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        OverallDimensionsBorder.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;

        if (showLanding && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }
    
    private void LandingTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }

    private void LandingDepthTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
    }
    
    private void LandingDepth_LostFocus(object sender, RoutedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }

    private void StepsBeforeLandingTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
    }
    
    private void StepsBeforeLanding_LostFocus(object sender, RoutedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }
    
    private void SpaceConstraint_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
    }
    
    private void AvailableLength_LostFocus(object sender, RoutedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true)
        {
            CheckSpaceFit();
        }
    }
    
    private void AvailableWidth_LostFocus(object sender, RoutedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true)
        {
            CheckSpaceFit();
        }
    }
    
    private void CheckSpaceFit()
    {
        if (string.IsNullOrWhiteSpace(AvailableLengthTextBox.Text) && string.IsNullOrWhiteSpace(AvailableWidthTextBox.Text))
        {
            SpaceFitBorder.Visibility = Visibility.Collapsed;
            return;
        }
        
        if (IncludeLandingCheckBox.IsChecked != true || string.IsNullOrWhiteSpace(OverallLengthLabel.Text))
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
                fitMessage.AppendLine("\nTry different landing type or consider spiral staircase");
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
