using System.Text;
using System.Windows;

namespace ConstructionCalculator.WPF.Calculators.Geometry.Angle;

public partial class AngleCalculatorWindow : Window
{
    private bool isRadianMode = false;

    public AngleCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CalculateAngleButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Measurement rise = Measurement.Parse(RiseTextBox.Text);
            Measurement run = Measurement.Parse(RunTextBox.Text);

            double riseInches = rise.ToTotalInches();
            double runInches = run.ToTotalInches();

            double angleRadians = Math.Atan2(riseInches, runInches);
            double angleDegrees = angleRadians * (180.0 / Math.PI);

            ResultLabel.Text = $"Angle: {angleDegrees:F2}° ({angleRadians:F4} rad)\nRatio: {riseInches:F2}:{runInches:F2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateRatioButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double angle = double.Parse(AngleTextBox.Text);
            double angleRadians = isRadianMode ? angle : angle * (Math.PI / 180.0);

            double runInches = 12.0;
            double riseInches = Math.Tan(angleRadians) * runInches;

            Measurement rise = Measurement.FromDecimalInches(riseInches);
            Measurement run = Measurement.FromDecimalInches(runInches);

            ResultLabel.Text = $"Rise: {rise.ToFractionString()}\nRun: {run.ToFractionString()}\n(for 12\" run)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ModeButton_Click(object sender, RoutedEventArgs e)
    {
        isRadianMode = !isRadianMode;
        ModeButton.Content = isRadianMode ? "Radians" : "Degrees";
    }

    private void SolverCalculateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool hasAngle = !string.IsNullOrWhiteSpace(SolverAngleTextBox.Text);
            bool hasOpposite = !string.IsNullOrWhiteSpace(SolverOppositeTextBox.Text);
            bool hasAdjacent = !string.IsNullOrWhiteSpace(SolverAdjacentTextBox.Text);
            bool hasHypotenuse = !string.IsNullOrWhiteSpace(SolverHypotenuseTextBox.Text);

            int inputCount = (hasAngle ? 1 : 0) + (hasOpposite ? 1 : 0) + (hasAdjacent ? 1 : 0) + (hasHypotenuse ? 1 : 0);

            if (inputCount != 2)
            {
                MessageBox.Show("Please enter exactly 2 values to calculate the rest.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
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
                angle = double.Parse(SolverAngleTextBox.Text);
                if (!isRadianMode)
                {
                    angle = angle.Value * (Math.PI / 180.0);
                }
            }
            if (hasOpposite)
            {
                opposite = Measurement.Parse(SolverOppositeTextBox.Text).ToTotalInches();
            }
            if (hasAdjacent)
            {
                adjacent = Measurement.Parse(SolverAdjacentTextBox.Text).ToTotalInches();
            }
            if (hasHypotenuse)
            {
                hypotenuse = Measurement.Parse(SolverHypotenuseTextBox.Text).ToTotalInches();
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

            double angleDegrees = angle.Value * (180.0 / Math.PI);
            result.AppendLine("CALCULATED VALUES:");
            result.AppendLine($"Angle: {angleDegrees:F2}° ({angle.Value:F4} rad)");
            result.AppendLine($"Opposite (Rise): {Measurement.FromDecimalInches(opposite.Value).ToFractionString()}");
            result.AppendLine($"Adjacent (Run): {Measurement.FromDecimalInches(adjacent.Value).ToFractionString()}");
            result.AppendLine($"Hypotenuse: {Measurement.FromDecimalInches(hypotenuse.Value).ToFractionString()}");
            result.AppendLine();
            result.Append(formulas.ToString());

            SolverResultTextBox.Text = result.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SolverClearButton_Click(object sender, RoutedEventArgs e)
    {
        SolverAngleTextBox.Clear();
        SolverOppositeTextBox.Clear();
        SolverAdjacentTextBox.Clear();
        SolverHypotenuseTextBox.Clear();
        SolverResultTextBox.Clear();
    }
}
