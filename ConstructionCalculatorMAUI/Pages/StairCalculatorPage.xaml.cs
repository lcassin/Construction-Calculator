using ConstructionCalculator.Core;
using System.Text;

namespace ConstructionCalculatorMAUI.Pages;

public partial class StairCalculatorPage : ContentPage
{
    public StairCalculatorPage()
    {
        InitializeComponent();
    }

    private void OnTotalRiseCompleted(object sender, EventArgs e)
    {
        OnCalculateClicked(sender, e);
    }

    private void OnStepsChanged(object sender, ValueChangedEventArgs e)
    {
        int steps = (int)e.NewValue;
        StepsLabel.Text = $"{steps} steps";
    }

    private void OnStepsBeforeLandingChanged(object sender, ValueChangedEventArgs e)
    {
        int steps = (int)e.NewValue;
        StepsBeforeLandingLabel.Text = $"{steps} steps";
        
        if (IncludeLandingCheckBox.IsChecked)
        {
            OnCalculateClicked(sender, e);
        }
    }

    private void OnIncludeLandingChanged(object sender, CheckedChangedEventArgs e)
    {
        LandingSection.IsVisible = e.Value;
        
        if (e.Value && !string.IsNullOrWhiteSpace(TotalRiseEntry.Text))
        {
            OnCalculateClicked(sender, e);
        }
    }

    private void OnLandingDepthChanged(object sender, TextChangedEventArgs e)
    {
        if (IncludeLandingCheckBox.IsChecked)
        {
            OnCalculateClicked(sender, e);
        }
    }

    private async void OnAutoCalculateClicked(object sender, EventArgs e)
    {
        try
        {
            Measurement totalRise = Measurement.Parse(TotalRiseEntry.Text);
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

            NumberOfStepsStepper.Value = optimalSteps;
            StepsLabel.Text = $"{optimalSteps} steps";

            OnCalculateClicked(sender, e);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnCalculateClicked(object sender, EventArgs e)
    {
        try
        {
            Measurement totalRise = Measurement.Parse(TotalRiseEntry.Text);
            int numberOfSteps = (int)NumberOfStepsStepper.Value;

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

            if (IncludeLandingCheckBox.IsChecked)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(LandingDepthEntry.Text))
                    {
                        StepsAfterLandingLabel.Text = "Enter landing depth";
                        TotalRunWithLandingLabel.Text = "";
                    }
                    else
                    {
                        Measurement landingDepth = Measurement.Parse(LandingDepthEntry.Text);
                        double landingDepthInches = landingDepth.ToTotalInches();

                        if (landingDepthInches < 36.0)
                        {
                            await DisplayAlert("Validation Warning", 
                                "Landing depth should be at least 36\" per typical building codes.", "OK");
                        }

                        int stepsBeforeLanding = (int)StepsBeforeLandingStepper.Value;
                        if (stepsBeforeLanding >= numberOfSteps)
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
                ComplianceLabel.TextColor = Colors.Green;
            }
            else if ((riserHeightInches >= 6.0 && riserHeightInches <= 8.0) &&
                     (treadDepthInches >= 9.0 && treadDepthInches <= 12.0))
            {
                ComplianceLabel.TextColor = Colors.Orange;
            }
            else
            {
                ComplianceLabel.TextColor = Colors.Red;
            }

            GenerateVisualDiagram(totalRise, numberOfSteps, riserHeight, treadDepth);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private void GenerateVisualDiagram(Measurement totalRise, int numberOfSteps, Measurement riserHeight, Measurement treadDepth)
    {
        StringBuilder diagram = new StringBuilder();

        diagram.AppendLine($"Total Rise: {totalRise.ToFractionString()}");
        diagram.AppendLine($"Number of Steps: {numberOfSteps}");
        diagram.AppendLine($"Riser Height: {riserHeight.ToFractionString()} ({riserHeight.ToTotalInches():F2}\")");
        diagram.AppendLine($"Tread Depth: {treadDepth.ToFractionString()} ({treadDepth.ToTotalInches():F2}\")");

        if (IncludeLandingCheckBox.IsChecked && !string.IsNullOrWhiteSpace(LandingDepthEntry.Text))
        {
            try
            {
                Measurement landingDepth = Measurement.Parse(LandingDepthEntry.Text);
                int stepsBeforeLanding = (int)StepsBeforeLandingStepper.Value;
                int stepsAfterLanding = numberOfSteps - stepsBeforeLanding;

                diagram.AppendLine();
                diagram.AppendLine("WITH LANDING:");
                diagram.AppendLine($"Steps Before Landing: {stepsBeforeLanding}");
                diagram.AppendLine($"Landing Depth: {landingDepth.ToFractionString()}");
                diagram.AppendLine($"Steps After Landing: {stepsAfterLanding}");
            }
            catch
            {
            }
        }

        diagram.AppendLine();
        diagram.AppendLine("Stair Profile:");
        diagram.AppendLine();

        if (IncludeLandingCheckBox.IsChecked && !string.IsNullOrWhiteSpace(LandingDepthEntry.Text))
        {
            try
            {
                int stepsBeforeLanding = (int)StepsBeforeLandingStepper.Value;
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
            catch
            {
                GenerateSimpleDiagram(diagram, numberOfSteps, riserHeight, treadDepth);
            }
        }
        else
        {
            GenerateSimpleDiagram(diagram, numberOfSteps, riserHeight, treadDepth);
        }

        VisualDiagramLabel.Text = diagram.ToString();
    }

    private void GenerateSimpleDiagram(StringBuilder diagram, int numberOfSteps, Measurement riserHeight, Measurement treadDepth)
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
}
