namespace ConstructionCalculatorMAUI.Pages.Calculators.UnitConverter;

public partial class UnitConverterPage : ContentPage
{
    private Dictionary<string, Dictionary<string, double>> conversionFactors = new();

    public UnitConverterPage()
    {
        InitializeComponent();
        InitializeConversionFactors();
        UpdateUnitPickers();
        UpdateReferenceText();
    }

    private void InitializeConversionFactors()
    {
        conversionFactors["Measurement Format"] = new Dictionary<string, double>
        {
            { "Feet/Inches/Fractions", 1.0 },
            { "Decimal Inches", 1.0 },
            { "Decimal Feet", 1.0 }
        };

        conversionFactors["Length"] = new Dictionary<string, double>
        {
            { "Inches", 1.0 },
            { "Feet", 12.0 },
            { "Yards", 36.0 },
            { "Miles", 63360.0 },
            { "Millimeters", 0.0393701 },
            { "Centimeters", 0.393701 },
            { "Meters", 39.3701 }
        };

        conversionFactors["Area"] = new Dictionary<string, double>
        {
            { "Square Inches", 1.0 },
            { "Square Feet", 144.0 },
            { "Square Yards", 1296.0 },
            { "Acres", 6272640.0 },
            { "Square Meters", 1550.0031 }
        };

        conversionFactors["Volume"] = new Dictionary<string, double>
        {
            { "Cubic Inches", 1.0 },
            { "Cubic Feet", 1728.0 },
            { "Cubic Yards", 46656.0 },
            { "Gallons (US)", 231.0 },
            { "Liters", 61.0237 },
            { "Cubic Meters", 61023.7 }
        };

        conversionFactors["Weight"] = new Dictionary<string, double>
        {
            { "Ounces", 1.0 },
            { "Pounds", 16.0 },
            { "Tons (US)", 32000.0 },
            { "Grams", 0.035274 },
            { "Kilograms", 35.274 }
        };

        conversionFactors["Temperature"] = new Dictionary<string, double>
        {
            { "Fahrenheit", 1.0 },
            { "Celsius", 1.0 },
            { "Kelvin", 1.0 }
        };
    }

    private void UpdateUnitPickers()
    {
        if (ConversionTypePicker.SelectedItem == null)
            return;

        string selectedType = ConversionTypePicker.SelectedItem.ToString() ?? "Length";

        FromUnitPicker.Items.Clear();
        ToUnitPicker.Items.Clear();

        if (conversionFactors.ContainsKey(selectedType))
        {
            foreach (var unit in conversionFactors[selectedType].Keys)
            {
                FromUnitPicker.Items.Add(unit);
                ToUnitPicker.Items.Add(unit);
            }

            if (FromUnitPicker.Items.Count > 0)
            {
                FromUnitPicker.SelectedIndex = 0;
                ToUnitPicker.SelectedIndex = FromUnitPicker.Items.Count > 1 ? 1 : 0;
            }
        }
    }

    private void UpdateReferenceText()
    {
        if (ConversionTypePicker.SelectedItem == null)
            return;

        string selectedType = ConversionTypePicker.SelectedItem.ToString() ?? "Length";

        string referenceText = selectedType switch
        {
            "Measurement Format" => "Feet/Inches/Fractions: 6' 3 1/2\"\nDecimal Inches: 75.5\nDecimal Feet: 6.2917\n\nExamples:\n6' 3 1/2\" = 75.5 inches = 6.2917 feet\n100 inches = 8' 4\" = 8.3333 feet",
            "Length" => "1 foot = 12 inches\n1 yard = 3 feet = 36 inches\n1 mile = 5,280 feet\n1 meter = 39.37 inches\n1 inch = 2.54 centimeters",
            "Area" => "1 square foot = 144 square inches\n1 square yard = 9 square feet\n1 acre = 43,560 square feet\n1 square meter = 10.764 square feet",
            "Volume" => "1 cubic foot = 1,728 cubic inches\n1 cubic yard = 27 cubic feet\n1 gallon = 231 cubic inches\n1 cubic meter = 35.315 cubic feet",
            "Weight" => "1 pound = 16 ounces\n1 ton = 2,000 pounds\n1 kilogram = 2.205 pounds\n1 ounce = 28.35 grams",
            "Temperature" => "°F = (°C × 9/5) + 32\n°C = (°F - 32) × 5/9\nK = °C + 273.15",
            _ => ""
        };

        ReferenceLabel.Text = referenceText;
    }

    private void ConversionTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        FromValueEntry.Text = "";
        ToValueEntry.Text = "";
        UpdateUnitPickers();
        UpdateReferenceText();
    }

    private void FromValueEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        PerformConversion();
    }

    private void ConversionChanged(object sender, EventArgs e)
    {
        PerformConversion();
    }

    private void PerformConversion()
    {
        if (FromUnitPicker.SelectedItem == null || ToUnitPicker.SelectedItem == null)
            return;

        string selectedType = ConversionTypePicker.SelectedItem?.ToString() ?? "Length";
        string fromUnit = FromUnitPicker.SelectedItem.ToString() ?? "";
        string toUnit = ToUnitPicker.SelectedItem.ToString() ?? "";

        if (selectedType == "Measurement Format")
        {
            ConvertMeasurementFormat(FromValueEntry.Text, fromUnit, toUnit);
            return;
        }

        if (!double.TryParse(FromValueEntry.Text, out double fromValue))
        {
            ToValueEntry.Text = "";
            return;
        }

        double result = 0;

        if (selectedType == "Temperature")
        {
            result = ConvertTemperature(fromValue, fromUnit, toUnit);
        }
        else if (conversionFactors.ContainsKey(selectedType))
        {
            var factors = conversionFactors[selectedType];
            if (factors.ContainsKey(fromUnit) && factors.ContainsKey(toUnit))
            {
                double baseValue = fromValue * factors[fromUnit];
                result = baseValue / factors[toUnit];
            }
        }

        ToValueEntry.Text = result.ToString("F6").TrimEnd('0').TrimEnd('.');
    }

    private void ConvertMeasurementFormat(string inputText, string fromUnit, string toUnit)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ToValueEntry.Text = "";
                return;
            }

            double totalInches = 0;

            if (fromUnit == "Feet/Inches/Fractions")
            {
                totalInches = ParseFeetInches(inputText);
            }
            else if (fromUnit == "Decimal Inches")
            {
                if (!double.TryParse(inputText, out totalInches))
                {
                    ToValueEntry.Text = "Invalid input";
                    return;
                }
            }
            else if (fromUnit == "Decimal Feet")
            {
                if (!double.TryParse(inputText, out double feet))
                {
                    ToValueEntry.Text = "Invalid input";
                    return;
                }
                totalInches = feet * 12.0;
            }

            if (toUnit == "Feet/Inches/Fractions")
            {
                ToValueEntry.Text = FormatFeetInches(totalInches);
            }
            else if (toUnit == "Decimal Inches")
            {
                ToValueEntry.Text = totalInches.ToString("F4").TrimEnd('0').TrimEnd('.');
            }
            else if (toUnit == "Decimal Feet")
            {
                double feet = totalInches / 12.0;
                ToValueEntry.Text = feet.ToString("F6").TrimEnd('0').TrimEnd('.');
            }
        }
        catch (Exception ex)
        {
            ToValueEntry.Text = $"Error: {ex.Message}";
        }
    }

    private double ParseFeetInches(string input)
    {
        input = input.Trim();
        double totalInches = 0;

        string[] parts = input.Split('\'');
        
        if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0]))
        {
            if (double.TryParse(parts[0].Trim(), out double feet))
            {
                totalInches += feet * 12;
            }
        }

        if (parts.Length > 1)
        {
            string inchPart = parts[1].Replace("\"", "").Trim();
            
            if (inchPart.Contains('/'))
            {
                string[] inchFraction = inchPart.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (inchFraction.Length > 0 && double.TryParse(inchFraction[0], out double wholeInches))
                {
                    totalInches += wholeInches;
                }
                
                if (inchFraction.Length > 1 && inchFraction[1].Contains('/'))
                {
                    string[] fraction = inchFraction[1].Split('/');
                    if (fraction.Length == 2 && 
                        double.TryParse(fraction[0], out double numerator) && 
                        double.TryParse(fraction[1], out double denominator) && 
                        denominator != 0)
                    {
                        totalInches += numerator / denominator;
                    }
                }
            }
            else if (double.TryParse(inchPart, out double inches))
            {
                totalInches += inches;
            }
        }

        return totalInches;
    }

    private string FormatFeetInches(double totalInches)
    {
        int feet = (int)(totalInches / 12);
        double remainingInches = totalInches - (feet * 12);
        
        int wholeInches = (int)remainingInches;
        double fraction = remainingInches - wholeInches;
        
        if (fraction < 0.03125)
        {
            return wholeInches == 0 ? $"{feet}'" : $"{feet}' {wholeInches}\"";
        }
        
        int[] denominators = { 16, 8, 4, 2 };
        foreach (int denom in denominators)
        {
            int numerator = (int)Math.Round(fraction * denom);
            if (numerator > 0 && numerator < denom)
            {
                while (numerator % 2 == 0 && denom % 2 == 0)
                {
                    numerator /= 2;
                    denom /= 2;
                }
                
                if (wholeInches > 0)
                    return $"{feet}' {wholeInches} {numerator}/{denom}\"";
                else
                    return $"{feet}' {numerator}/{denom}\"";
            }
        }
        
        return wholeInches == 0 ? $"{feet}'" : $"{feet}' {wholeInches}\"";
    }

    private double ConvertTemperature(double value, string fromUnit, string toUnit)
    {
        if (fromUnit == toUnit)
            return value;

        double celsius = fromUnit switch
        {
            "Fahrenheit" => (value - 32) * 5.0 / 9.0,
            "Kelvin" => value - 273.15,
            _ => value
        };

        return toUnit switch
        {
            "Fahrenheit" => (celsius * 9.0 / 5.0) + 32,
            "Kelvin" => celsius + 273.15,
            _ => celsius
        };
    }

    private async void CopyResult_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ToValueEntry.Text))
        {
            await Clipboard.SetTextAsync(ToValueEntry.Text);
            await DisplayAlert("Success", "Result copied to clipboard!", "OK");
        }
    }

    private void Clear_Clicked(object sender, EventArgs e)
    {
        FromValueEntry.Text = "";
        ToValueEntry.Text = "";
        FromValueEntry.Focus();
    }
}
