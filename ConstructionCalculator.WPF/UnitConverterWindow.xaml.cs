using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF;

public partial class UnitConverterWindow : Window
{
    private Dictionary<string, Dictionary<string, double>> conversionFactors = new();
    
    public UnitConverterWindow()
    {
        InitializeComponent();
        InitializeConversionFactors();
        ConversionTypeComboBox.SelectedIndex = 0;
        UpdateUnitComboBoxes();
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
    
    private void UpdateUnitComboBoxes()
    {
        if (ConversionTypeComboBox.SelectedItem == null || FromUnitComboBox == null || ToUnitComboBox == null)
            return;
            
        string selectedType = ((ComboBoxItem)ConversionTypeComboBox.SelectedItem).Content.ToString() ?? "Length";
        
        FromUnitComboBox.Items.Clear();
        ToUnitComboBox.Items.Clear();
        
        if (conversionFactors.ContainsKey(selectedType))
        {
            foreach (var unit in conversionFactors[selectedType].Keys)
            {
                FromUnitComboBox.Items.Add(unit);
                ToUnitComboBox.Items.Add(unit);
            }
            
            if (FromUnitComboBox.Items.Count > 0)
            {
                FromUnitComboBox.SelectedIndex = 0;
                ToUnitComboBox.SelectedIndex = FromUnitComboBox.Items.Count > 1 ? 1 : 0;
            }
        }
    }
    
    private void UpdateReferenceText()
    {
        if (ConversionTypeComboBox.SelectedItem == null || ReferenceTextBlock == null)
            return;
            
        string selectedType = ((ComboBoxItem)ConversionTypeComboBox.SelectedItem).Content.ToString() ?? "Length";
        
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
        
        ReferenceTextBlock.Text = referenceText;
    }
    
    private void ConversionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FromValueTextBox != null && ToValueTextBox != null)
        {
            FromValueTextBox.Text = "";
            ToValueTextBox.Text = "";
        }
        UpdateUnitComboBoxes();
        UpdateReferenceText();
    }
    
    private void FromValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        PerformConversion();
    }
    
    private void ConversionChanged(object sender, SelectionChangedEventArgs e)
    {
        PerformConversion();
    }
    
    private void PerformConversion()
    {
        if (FromValueTextBox == null || ToValueTextBox == null || 
            FromUnitComboBox.SelectedItem == null || ToUnitComboBox.SelectedItem == null)
            return;
        
        string selectedType = ((ComboBoxItem)ConversionTypeComboBox.SelectedItem).Content.ToString() ?? "Length";
        string fromUnit = FromUnitComboBox.SelectedItem.ToString() ?? "";
        string toUnit = ToUnitComboBox.SelectedItem.ToString() ?? "";
        
        if (selectedType == "Measurement Format")
        {
            ConvertMeasurementFormat(FromValueTextBox.Text, fromUnit, toUnit);
            return;
        }
        
        if (!double.TryParse(FromValueTextBox.Text, out double fromValue))
        {
            ToValueTextBox.Text = "";
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
        
        ToValueTextBox.Text = result.ToString("F6").TrimEnd('0').TrimEnd('.');
    }
    
    private void ConvertMeasurementFormat(string inputText, string fromUnit, string toUnit)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ToValueTextBox.Text = "";
                return;
            }
            
            double totalInches = 0;
            
            if (fromUnit == "Feet/Inches/Fractions")
            {
                Measurement m = Measurement.Parse(inputText);
                totalInches = m.ToTotalInches();
            }
            else if (fromUnit == "Decimal Inches")
            {
                if (!double.TryParse(inputText, out totalInches))
                {
                    ToValueTextBox.Text = "Invalid input";
                    return;
                }
            }
            else if (fromUnit == "Decimal Feet")
            {
                if (!double.TryParse(inputText, out double feet))
                {
                    ToValueTextBox.Text = "Invalid input";
                    return;
                }
                totalInches = feet * 12.0;
            }
            
            if (toUnit == "Feet/Inches/Fractions")
            {
                Measurement result = Measurement.FromDecimalInches(totalInches);
                ToValueTextBox.Text = result.ToFractionString();
            }
            else if (toUnit == "Decimal Inches")
            {
                ToValueTextBox.Text = totalInches.ToString("F4").TrimEnd('0').TrimEnd('.');
            }
            else if (toUnit == "Decimal Feet")
            {
                double feet = totalInches / 12.0;
                ToValueTextBox.Text = feet.ToString("F6").TrimEnd('0').TrimEnd('.');
            }
        }
        catch (Exception ex)
        {
            ToValueTextBox.Text = $"Error: {ex.Message}";
        }
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
    
    private void CopyResult_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ToValueTextBox.Text))
        {
            Clipboard.SetText(ToValueTextBox.Text);
            MessageBox.Show("Result copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        FromValueTextBox.Text = "";
        ToValueTextBox.Text = "";
        FromValueTextBox.Focus();
    }
}
