using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ConstructionCalculator.WPF;

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
                    if (string.IsNullOrWhiteSpace(LandingDepthTextBox.Text))
                    {
                        StepsAfterLandingLabel.Text = "Enter landing depth";
                        TotalRunWithLandingLabel.Text = "";
                    }
                    else
                    {
                        Measurement landingDepth = Measurement.Parse(LandingDepthTextBox.Text);
                        double landingDepthInches = landingDepth.ToTotalInches();

                        if (landingDepthInches < 36.0)
                        {
                            MessageBox.Show("Landing depth should be at least 36\" per typical building codes.",
                                "Validation Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        if (!int.TryParse(StepsBeforeLandingTextBox.Text, out int stepsBeforeLanding) || stepsBeforeLanding < 1)
                        {
                            StepsAfterLandingLabel.Text = "Invalid steps before landing";
                            TotalRunWithLandingLabel.Text = "";
                        }
                        else if (stepsBeforeLanding >= numberOfSteps)
                        {
                            StepsAfterLandingLabel.Text = "Steps before landing must be less than total steps";
                            TotalRunWithLandingLabel.Text = "";
                        }
                        else
                        {
                            int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;
                            double runBeforeLanding = (stepsBeforeLanding - 1) * treadDepthInches;
                            double runAfterLanding = (stepsAfterLanding - 1) * treadDepthInches;
                            double totalRunWithLanding = runBeforeLanding + landingDepthInches + runAfterLanding;

                            StepsAfterLandingLabel.Text = $"Steps After Landing: {stepsAfterLanding}";
                            Measurement totalRunWithLandingMeasurement = Measurement.FromDecimalInches(totalRunWithLanding);
                            TotalRunWithLandingLabel.Text = $"Total Run with Landing: {totalRunWithLandingMeasurement.ToFractionString()}";
                        }
                    }
                }
                catch (Exception)
                {
                    StepsAfterLandingLabel.Text = "Invalid landing depth";
                    TotalRunWithLandingLabel.Text = "";
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
        LandingDepthPanel.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        StepsBeforeLandingPanel.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        StepsAfterLandingBorder.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;
        TotalRunWithLandingBorder.Visibility = showLanding ? Visibility.Visible : Visibility.Collapsed;

        if (showLanding && !string.IsNullOrWhiteSpace(TotalRiseTextBox.Text))
        {
            CalculateButton_Click(sender, e);
        }
    }

    private void LandingDepthTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true)
        {
            CalculateButton_Click(sender, e);
        }
    }

    private void StepsBeforeLandingTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked == true)
        {
            CalculateButton_Click(sender, e);
        }
    }
}
