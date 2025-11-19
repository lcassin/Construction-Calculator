using System;
using System.Windows;

namespace ConstructionCalculator.WPF;

public partial class GradingCalculatorWindow : Window
{
    public GradingCalculatorWindow()
    {
        InitializeComponent();
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double rise = 0, run = 0, percent = 0, angle = 0;
            int inputCount = 0;

            if (!string.IsNullOrWhiteSpace(RiseTextBox.Text))
            {
                rise = double.Parse(RiseTextBox.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(RunTextBox.Text))
            {
                run = double.Parse(RunTextBox.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(PercentTextBox.Text))
            {
                percent = double.Parse(PercentTextBox.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(AngleTextBox.Text))
            {
                angle = double.Parse(AngleTextBox.Text);
                inputCount++;
            }

            if (inputCount == 0)
            {
                MessageBox.Show("Please enter at least one value.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (inputCount > 1)
            {
                MessageBox.Show("Please enter only ONE value. The calculator will compute the others.", "Too Many Inputs", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rise > 0 && run == 0)
            {
                run = 1;
                percent = (rise / run) * 100;
                angle = Math.Atan(rise / run) * (180.0 / Math.PI);
            }
            else if (run > 0 && rise == 0)
            {
                rise = 1;
                percent = (rise / run) * 100;
                angle = Math.Atan(rise / run) * (180.0 / Math.PI);
            }
            else if (percent > 0)
            {
                run = 100;
                rise = percent;
                angle = Math.Atan(rise / run) * (180.0 / Math.PI);
            }
            else if (angle > 0)
            {
                double radians = angle * (Math.PI / 180.0);
                rise = Math.Tan(radians);
                run = 1;
                percent = (rise / run) * 100;
            }

            RiseTextBox.Text = rise.ToString("F4");
            RunTextBox.Text = run.ToString("F4");
            PercentTextBox.Text = percent.ToString("F4");
            AngleTextBox.Text = angle.ToString("F4");

            string ratio = $"{rise:F2}:{run:F2}";
            if (run == 1) ratio = $"{rise:F2}:1";
            if (rise == 1) ratio = $"1:{run:F2}";

            ResultTextBlock.Text = $"Slope Conversions:\n\n" +
                                  $"Ratio: {ratio}\n" +
                                  $"Percent Grade: {percent:F2}%\n" +
                                  $"Angle: {angle:F2}°\n\n" +
                                  $"Rise: {rise:F4}\n" +
                                  $"Run: {run:F4}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nPlease check your inputs.", 
                          "Calculation Error", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        RiseTextBox.Clear();
        RunTextBox.Clear();
        PercentTextBox.Clear();
        AngleTextBox.Clear();
        ResultTextBlock.Text = "";
        RiseTextBox.Focus();
    }
}
