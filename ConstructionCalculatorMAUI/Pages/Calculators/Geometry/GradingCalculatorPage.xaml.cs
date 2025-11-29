namespace ConstructionCalculatorMAUI.Pages.Calculators.Geometry;

public partial class GradingCalculatorPage : ContentPage
{
    public GradingCalculatorPage()
    {
        InitializeComponent();
    }

    private async void Calculate_Clicked(object sender, EventArgs e)
    {
        try
        {
            double rise = 0, run = 0, percent = 0, angle = 0;
            int inputCount = 0;

            if (!string.IsNullOrWhiteSpace(RiseEntry.Text))
            {
                rise = double.Parse(RiseEntry.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(RunEntry.Text))
            {
                run = double.Parse(RunEntry.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(PercentEntry.Text))
            {
                percent = double.Parse(PercentEntry.Text);
                inputCount++;
            }

            if (!string.IsNullOrWhiteSpace(AngleEntry.Text))
            {
                angle = double.Parse(AngleEntry.Text);
                inputCount++;
            }

            if (inputCount == 0)
            {
                await DisplayAlert("Input Required", "Please enter at least one value.", "OK");
                return;
            }

            if (inputCount > 1)
            {
                await DisplayAlert("Too Many Inputs", "Please enter only ONE value. The calculator will compute the others.", "OK");
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

            RiseEntry.Text = rise.ToString("F4");
            RunEntry.Text = run.ToString("F4");
            PercentEntry.Text = percent.ToString("F4");
            AngleEntry.Text = angle.ToString("F4");

            string ratio = $"{rise:F2}:{run:F2}";
            if (run == 1) ratio = $"{rise:F2}:1";
            if (rise == 1) ratio = $"1:{run:F2}";

            ResultLabel.Text = $"Slope Conversions:\n\n" +
                              $"Ratio: {ratio}\n" +
                              $"Percent Grade: {percent:F2}%\n" +
                              $"Angle: {angle:F2}°\n\n" +
                              $"Rise: {rise:F4}\n" +
                              $"Run: {run:F4}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease check your inputs.", "OK");
        }
    }

    private void Clear_Clicked(object sender, EventArgs e)
    {
        RiseEntry.Text = "";
        RunEntry.Text = "";
        PercentEntry.Text = "";
        AngleEntry.Text = "";
        ResultLabel.Text = "";
        RiseEntry.Focus();
    }
}
