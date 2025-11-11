using ConstructionCalculator.Core;

namespace ConstructionCalculatorMAUI.Pages;

public partial class SurveyCalculatorPage : ContentPage
{
    private Entry? _focusedEntry;

    public SurveyCalculatorPage()
    {
        InitializeComponent();
    }

    #region Symbol Insert Handlers

    private void OnInsertDegreeClicked(object sender, EventArgs e) => InsertSymbol("°");

    private void OnInsertMinuteClicked(object sender, EventArgs e) => InsertSymbol("'");

    private void OnInsertSecondClicked(object sender, EventArgs e) => InsertSymbol("\"");

    private async void InsertSymbol(string symbol)
    {
        if (_focusedEntry == null)
        {
            await DisplayAlert("No Field Selected", "Please click in a text field first to position the cursor.", "OK");
            return;
        }

        int cursorPosition = _focusedEntry.CursorPosition;
        string currentText = _focusedEntry.Text ?? "";

        _focusedEntry.Text = currentText.Insert(cursorPosition, symbol);
        _focusedEntry.CursorPosition = cursorPosition + symbol.Length;
        _focusedEntry.Focus();
    }

    #endregion

    #region Focus Tracking

    private void OnBearingFocused(object sender, FocusEventArgs e) => _focusedEntry = BearingEntry;
    private void OnAzimuthFocused(object sender, FocusEventArgs e) => _focusedEntry = AzimuthEntry;
    private void OnStartNorthingFocused(object sender, FocusEventArgs e) => _focusedEntry = StartNorthingEntry;
    private void OnStartEastingFocused(object sender, FocusEventArgs e) => _focusedEntry = StartEastingEntry;
    private void OnDistanceFocused(object sender, FocusEventArgs e) => _focusedEntry = DistanceEntry;
    private void OnDirectionFocused(object sender, FocusEventArgs e) => _focusedEntry = DirectionEntry;

    #endregion

    #region Bearing/Azimuth Conversion

    private async void OnBearingToAzimuthClicked(object sender, EventArgs e)
    {
        try
        {
            string bearingStr = BearingEntry.Text.Trim().ToUpper();
            double azimuth = ParseBearingToAzimuth(bearingStr);

            int degrees = (int)azimuth;
            double minutesDecimal = (azimuth - degrees) * 60;
            int minutes = (int)minutesDecimal;
            double seconds = (minutesDecimal - minutes) * 60;

            ConversionResultLabel.Text = $"Azimuth: {degrees:D3}° {minutes:D2}' {seconds:F1}\"\n({azimuth:F4}°)";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Conversion Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnAzimuthToBearingClicked(object sender, EventArgs e)
    {
        try
        {
            double azimuth = ParseAzimuth(AzimuthEntry.Text.Trim());
            string bearing = AzimuthToBearing(azimuth);

            ConversionResultLabel.Text = $"Bearing: {bearing}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Conversion Error", $"Error: {ex.Message}", "OK");
        }
    }

    private double ParseBearingToAzimuth(string bearing)
    {
        char firstDir = bearing[0];
        char lastDir = bearing[^1];

        if ((firstDir != 'N' && firstDir != 'S') || (lastDir != 'E' && lastDir != 'W'))
        {
            throw new FormatException("Bearing must be in format like N45E, S30W, etc.");
        }

        string numberPart = bearing[1..^1];
        numberPart = numberPart.Replace("°", " ").Replace("'", " ").Replace("\"", " ").Trim();

        string[] parts = numberPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

        double angle = degrees + (minutes / 60.0) + (seconds / 3600.0);

        if (angle < 0 || angle > 90)
        {
            throw new ArgumentException("Bearing angle must be between 0 and 90 degrees");
        }

        double azimuth;
        if (firstDir == 'N' && lastDir == 'E')
        {
            azimuth = angle; // NE quadrant: 0-90°
        }
        else if (firstDir == 'S' && lastDir == 'E')
        {
            azimuth = 180 - angle; // SE quadrant: 90-180°
        }
        else if (firstDir == 'S' && lastDir == 'W')
        {
            azimuth = 180 + angle; // SW quadrant: 180-270°
        }
        else // N and W
        {
            azimuth = 360 - angle; // NW quadrant: 270-360°
        }

        return azimuth;
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

    #endregion

    #region Coordinate Geometry

    private async void OnCalculateEndPointClicked(object sender, EventArgs e)
    {
        try
        {
            double startNorthing = double.Parse(StartNorthingEntry.Text.Trim());
            double startEasting = double.Parse(StartEastingEntry.Text.Trim());

            double distanceFeet;
            string distanceStr = DistanceEntry.Text.Trim();
            if (distanceStr.Contains("'") || distanceStr.Contains("\""))
            {
                Measurement distMeasurement = Measurement.Parse(distanceStr);
                distanceFeet = distMeasurement.ToTotalInches() / 12.0;
            }
            else
            {
                distanceFeet = double.Parse(distanceStr);
            }

            string directionStr = DirectionEntry.Text.Trim();
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
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    #endregion
}
