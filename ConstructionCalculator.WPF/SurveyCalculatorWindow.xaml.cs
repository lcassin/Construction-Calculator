using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF;

public partial class SurveyCalculatorWindow : Window
{
    private TextBox? focusedTextBox;

    public SurveyCalculatorWindow()
    {
        InitializeComponent();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            focusedTextBox = textBox;
        }
    }

    private void DegreeButton_Click(object sender, RoutedEventArgs e)
    {
        InsertSymbol("°");
    }

    private void MinuteButton_Click(object sender, RoutedEventArgs e)
    {
        InsertSymbol("'");
    }

    private void SecondButton_Click(object sender, RoutedEventArgs e)
    {
        InsertSymbol("\"");
    }

    private void InsertSymbol(string symbol)
    {
        if (focusedTextBox == null)
        {
            MessageBox.Show("Please click in a text field first to position the cursor.", "No Field Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        int cursorPosition = focusedTextBox.CaretIndex;
        string currentText = focusedTextBox.Text;

        focusedTextBox.Text = currentText.Insert(cursorPosition, symbol);

        focusedTextBox.CaretIndex = cursorPosition + symbol.Length;
        focusedTextBox.Focus();
    }

    private void BearingToAzimuthButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string bearingStr = BearingTextBox.Text.Trim();
            var result = TryParseBearing(bearingStr);

            int azDegrees = (int)result.azimuth;
            double azMinutesDecimal = (result.azimuth - azDegrees) * 60;
            int azMinutes = (int)azMinutesDecimal;
            double azSeconds = (azMinutesDecimal - azMinutes) * 60;

            int angleDegrees = (int)result.angle;
            double angleMinutesDecimal = (result.angle - angleDegrees) * 60;
            int angleMinutes = (int)angleMinutesDecimal;
            double angleSeconds = (angleMinutesDecimal - angleMinutes) * 60;

            ConversionResultLabel.Text = $"Azimuth: {azDegrees:D3}° {azMinutes:D2}' {azSeconds:F1}\"\n" +
                                        $"({result.azimuth:F4}°)\n\n" +
                                        $"Bearing Angle: {angleDegrees:D2}° {angleMinutes:D2}' {angleSeconds:F1}\"\n" +
                                        $"({result.angle:F4}°)";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AzimuthToBearingButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double azimuth = ParseAzimuth(AzimuthTextBox.Text.Trim());
            string bearing = AzimuthToBearing(azimuth);

            ConversionResultLabel.Text = $"Bearing: {bearing}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CalculateEndPointButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double startNorthing = double.Parse(StartNorthingTextBox.Text.Trim());
            double startEasting = double.Parse(StartEastingTextBox.Text.Trim());

            double distanceFeet;
            string distanceStr = DistanceTextBox.Text.Trim();
            if (distanceStr.Contains("'") || distanceStr.Contains("\""))
            {
                Measurement distMeasurement = Measurement.Parse(distanceStr);
                distanceFeet = distMeasurement.ToTotalInches() / 12.0;
            }
            else
            {
                distanceFeet = double.Parse(distanceStr);
            }

            string directionStr = DirectionTextBox.Text.Trim();
            double azimuth;
            if (directionStr.ToUpper().Contains("N") || directionStr.ToUpper().Contains("S") ||
                directionStr.ToUpper().Contains("E") || directionStr.ToUpper().Contains("W"))
            {
                azimuth = ParseBearingToAzimuth(directionStr.ToUpper());
            }
            else
            {
                azimuth = ParseAzimuth(directionStr);
            }

            double azimuthRadians = azimuth * (Math.PI / 180.0);

            double deltaNorthing = distanceFeet * Math.Cos(azimuthRadians);
            double deltaEasting = distanceFeet * Math.Sin(azimuthRadians);

            double endNorthing = startNorthing + deltaNorthing;
            double endEasting = startEasting + deltaEasting;

            CoordinateResultLabel.Text = $"End Point:\nNorthing: {endNorthing:F2}\nEasting: {endEasting:F2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private double ParseBearingToAzimuth(string bearing)
    {
        var result = TryParseBearing(bearing);
        return result.azimuth;
    }

    private (char ns, char ew, double angle, double azimuth) TryParseBearing(string bearing)
    {
        bearing = bearing.ToUpper().Trim();

        if (string.IsNullOrWhiteSpace(bearing))
        {
            throw new FormatException("Bearing cannot be empty.\n\nExamples:\n  N 57° 00' E\n  S 21° 30' W\n  N45E\n  N 57.5° E");
        }

        char firstDir = '\0';
        int firstDirIndex = -1;
        for (int i = 0; i < bearing.Length; i++)
        {
            if (bearing[i] == 'N' || bearing[i] == 'S')
            {
                firstDir = bearing[i];
                firstDirIndex = i;
                break;
            }
        }

        char lastDir = '\0';
        int lastDirIndex = -1;
        for (int i = bearing.Length - 1; i >= 0; i--)
        {
            if (bearing[i] == 'E' || bearing[i] == 'W')
            {
                lastDir = bearing[i];
                lastDirIndex = i;
                break;
            }
        }

        if (firstDir == '\0' || lastDir == '\0' || firstDirIndex >= lastDirIndex)
        {
            throw new FormatException("Bearing must start with N or S and end with E or W.\n\nExamples:\n  N 57° 00' E\n  S 21° 30' W\n  N45E\n  N 57.5° E");
        }

        string numberPart = bearing.Substring(firstDirIndex + 1, lastDirIndex - firstDirIndex - 1).Trim();

        numberPart = numberPart.Replace("°", " ").Replace("'", " ").Replace("\"", " ")
                               .Replace("D", " ").Replace("M", " ").Replace("S", " ")
                               .Replace(":", " ").Replace("-", " ");

        string[] parts = numberPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            throw new FormatException("No angle value found in bearing.\n\nExamples:\n  N 57° 00' E\n  S 21° 30' W\n  N45E");
        }

        double degrees = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
        double minutes = 0;
        double seconds = 0;

        if (parts.Length > 1)
        {
            minutes = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
            if (minutes < 0 || minutes >= 60)
            {
                throw new ArgumentException("Minutes must be between 0 and 59");
            }
        }
        if (parts.Length > 2)
        {
            seconds = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
            if (seconds < 0 || seconds >= 60)
            {
                throw new ArgumentException("Seconds must be between 0 and 59");
            }
        }

        double angle = degrees + (minutes / 60.0) + (seconds / 3600.0);

        if (angle < 0 || angle > 90)
        {
            throw new ArgumentException($"Bearing angle must be between 0° and 90° (got {angle:F2}°)");
        }

        double azimuth;
        if (firstDir == 'N' && lastDir == 'E')
        {
            azimuth = angle;
        }
        else if (firstDir == 'S' && lastDir == 'E')
        {
            azimuth = 180 - angle;
        }
        else if (firstDir == 'S' && lastDir == 'W')
        {
            azimuth = 180 + angle;
        }
        else // N and W
        {
            azimuth = 360 - angle;
        }

        return (firstDir, lastDir, angle, azimuth);
    }

    private string AzimuthToBearing(double azimuth)
    {
        azimuth = azimuth % 360;
        if (azimuth < 0) azimuth += 360;

        int degrees;
        int minutes;
        double seconds;
        string quadrant;
        double bearingAngle;

        if (azimuth >= 0 && azimuth <= 90)
        {
            quadrant = "N{0}E";
            bearingAngle = azimuth;
        }
        else if (azimuth > 90 && azimuth <= 180)
        {
            quadrant = "S{0}E";
            bearingAngle = 180 - azimuth;
        }
        else if (azimuth > 180 && azimuth <= 270)
        {
            quadrant = "S{0}W";
            bearingAngle = azimuth - 180;
        }
        else
        {
            quadrant = "N{0}W";
            bearingAngle = 360 - azimuth;
        }

        degrees = (int)bearingAngle;
        double minutesDecimal = (bearingAngle - degrees) * 60;
        minutes = (int)minutesDecimal;
        seconds = (minutesDecimal - minutes) * 60;

        string angleStr = $"{degrees:D2}° {minutes:D2}' {seconds:F1}\"";
        return string.Format(quadrant, angleStr);
    }

    private double ParseAzimuth(string azimuth)
    {
        azimuth = azimuth.Replace("°", "").Replace("'", "").Replace("\"", "").Trim();

        string[] parts = azimuth.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        double degrees = double.Parse(parts[0]);
        double minutes = 0;
        double seconds = 0;

        if (parts.Length > 1)
        {
            minutes = double.Parse(parts[1]);
        }
        if (parts.Length > 2)
        {
            seconds = double.Parse(parts[2]);
        }

        double result = degrees + (minutes / 60.0) + (seconds / 3600.0);

        if (result < 0 || result >= 360)
        {
            throw new ArgumentException("Azimuth must be between 0 and 360 degrees");
        }

        return result;
    }
}
