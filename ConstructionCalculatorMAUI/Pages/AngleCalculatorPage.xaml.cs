using ConstructionCalculator;
using System.Text;

namespace ConstructionCalculatorMAUI.Pages;

public partial class AngleCalculatorPage : ContentPage
{
    private bool _isRadianMode = false;

    public AngleCalculatorPage()
    {
        InitializeComponent();
    }

    private void OnRiseCompleted(object sender, EventArgs e)
    {
        RunEntry.Focus();
    }

    private async void OnCalculateAngleClicked(object sender, EventArgs e)
    {
        try
        {
            Measurement rise = Measurement.Parse(RiseEntry.Text);
            Measurement run = Measurement.Parse(RunEntry.Text);

            double riseInches = rise.ToTotalInches();
            double runInches = run.ToTotalInches();

            double angleRadians = Math.Atan2(riseInches, runInches);
            double angleDegrees = angleRadians * (180.0 / Math.PI);

            ResultLabel.Text = $"Angle: {angleDegrees:F2}° ({angleRadians:F4} rad)\nRatio: {riseInches:F2}:{runInches:F2}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private void OnModeToggleClicked(object sender, EventArgs e)
    {
        _isRadianMode = !_isRadianMode;
        ModeButton.Text = _isRadianMode ? "Radians" : "Degrees";
    }

    private async void OnCalculateRatioClicked(object sender, EventArgs e)
    {
        try
        {
            double angle = double.Parse(AngleEntry.Text);
            
            double angleRadians = _isRadianMode ? angle : angle * (Math.PI / 180.0);

            double runInches = 12.0;
            double riseInches = Math.Tan(angleRadians) * runInches;

            Measurement rise = Measurement.FromDecimalInches(riseInches);
            Measurement run = Measurement.FromDecimalInches(runInches);

            ResultLabel.Text = $"Rise: {rise.ToFractionString()}\nRun: {run.ToFractionString()}\n(for 12\" run)";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnSolveTriangleClicked(object sender, EventArgs e)
    {
        try
        {
            bool hasAngle = !string.IsNullOrWhiteSpace(SolverAngleEntry.Text);
            bool hasOpposite = !string.IsNullOrWhiteSpace(SolverOppositeEntry.Text);
            bool hasAdjacent = !string.IsNullOrWhiteSpace(SolverAdjacentEntry.Text);
            bool hasHypotenuse = !string.IsNullOrWhiteSpace(SolverHypotenuseEntry.Text);

            int inputCount = (hasAngle ? 1 : 0) + (hasOpposite ? 1 : 0) + (hasAdjacent ? 1 : 0) + (hasHypotenuse ? 1 : 0);

            if (inputCount != 2)
            {
                await DisplayAlert("Input Required", "Please enter exactly 2 values to calculate the rest.", "OK");
                return;
            }

            double? angle = null;
            double? opposite = null;
            double? adjacent = null;
            double? hypotenuse = null;

            StringBuilder result = new StringBuilder();
            StringBuilder formulas = new StringBuilder();

            if (hasAngle)
            {
                angle = double.Parse(SolverAngleEntry.Text);
                if (!_isRadianMode)
                {
                    angle = angle.Value * (Math.PI / 180.0);
                }
            }
            if (hasOpposite)
            {
                opposite = Measurement.Parse(SolverOppositeEntry.Text).ToTotalInches();
            }
            if (hasAdjacent)
            {
                adjacent = Measurement.Parse(SolverAdjacentEntry.Text).ToTotalInches();
            }
            if (hasHypotenuse)
            {
                hypotenuse = Measurement.Parse(SolverHypotenuseEntry.Text).ToTotalInches();
            }

            if (hasAngle && hasOpposite)
            {
                adjacent = opposite.Value / Math.Tan(angle.Value);
                hypotenuse = opposite.Value / Math.Sin(angle.Value);
                formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                formulas.AppendLine("      sin(θ) = opposite/hypotenuse");
            }
            else if (hasAngle && hasAdjacent)
            {
                opposite = adjacent.Value * Math.Tan(angle.Value);
                hypotenuse = adjacent.Value / Math.Cos(angle.Value);
                formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                formulas.AppendLine("      cos(θ) = adjacent/hypotenuse");
            }
            else if (hasAngle && hasHypotenuse)
            {
                opposite = hypotenuse.Value * Math.Sin(angle.Value);
                adjacent = hypotenuse.Value * Math.Cos(angle.Value);
                formulas.AppendLine("Used: sin(θ) = opposite/hypotenuse");
                formulas.AppendLine("      cos(θ) = adjacent/hypotenuse");
            }
            else if (hasOpposite && hasAdjacent)
            {
                angle = Math.Atan2(opposite.Value, adjacent.Value);
                hypotenuse = Math.Sqrt(opposite.Value * opposite.Value + adjacent.Value * adjacent.Value);
                formulas.AppendLine("Used: tan(θ) = opposite/adjacent");
                formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
            }
            else if (hasOpposite && hasHypotenuse)
            {
                angle = Math.Asin(opposite.Value / hypotenuse.Value);
                adjacent = Math.Sqrt(hypotenuse.Value * hypotenuse.Value - opposite.Value * opposite.Value);
                formulas.AppendLine("Used: sin(θ) = opposite/hypotenuse");
                formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
            }
            else if (hasAdjacent && hasHypotenuse)
            {
                angle = Math.Acos(adjacent.Value / hypotenuse.Value);
                opposite = Math.Sqrt(hypotenuse.Value * hypotenuse.Value - adjacent.Value * adjacent.Value);
                formulas.AppendLine("Used: cos(θ) = adjacent/hypotenuse");
                formulas.AppendLine("      Pythagorean theorem: a² + b² = c²");
            }

            double angleDegrees = angle!.Value * (180.0 / Math.PI);
            result.AppendLine("CALCULATED VALUES:");
            result.AppendLine($"Angle: {angleDegrees:F2}° ({angle.Value:F4} rad)");
            result.AppendLine($"Opposite (Rise): {Measurement.FromDecimalInches(opposite!.Value).ToFractionString()}");
            result.AppendLine($"Adjacent (Run): {Measurement.FromDecimalInches(adjacent!.Value).ToFractionString()}");
            result.AppendLine($"Hypotenuse: {Measurement.FromDecimalInches(hypotenuse!.Value).ToFractionString()}");
            result.AppendLine();
            result.Append(formulas.ToString());

            SolverResultLabel.Text = result.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private void OnSolverClearClicked(object sender, EventArgs e)
    {
        SolverAngleEntry.Text = "";
        SolverOppositeEntry.Text = "";
        SolverAdjacentEntry.Text = "";
        SolverHypotenuseEntry.Text = "";
        SolverResultLabel.Text = "";
    }
}
