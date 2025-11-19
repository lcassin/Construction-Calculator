using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF;

public partial class CalculatorLauncherWindow : Window
{
    private List<CalculatorDescriptor> allCalculators;
    
    public CalculatorLauncherWindow()
    {
        InitializeComponent();
        InitializeCalculators();
        UpdateDisplay();
    }
    
    private void InitializeCalculators()
    {
        allCalculators = new List<CalculatorDescriptor>
        {
            new CalculatorDescriptor
            {
                Name = "Unit Converter",
                Category = "Common",
                Description = "Convert between units: length, area, volume, weight, temperature",
                OpenAction = () => new UnitConverterWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Area Calculator",
                Category = "Common",
                Description = "Calculate areas of multiple sections and sum totals",
                OpenAction = () => new AreaCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Concrete Calculator",
                Category = "Construction",
                Description = "Calculate cubic yards for slabs, footings, and columns",
                OpenAction = () => new ConcreteCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Angle Calculator",
                Category = "Measurement & Layout",
                Description = "Calculate angles from rise/run and solve right triangles",
                OpenAction = () => new AngleCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Survey Calculator",
                Category = "Measurement & Layout",
                Description = "Convert bearings/azimuths and calculate coordinates",
                OpenAction = () => new SurveyCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Seating Layout",
                Category = "Measurement & Layout",
                Description = "Calculate seating arrangements and spacing",
                OpenAction = () => new SeatingLayoutCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Stair Calculator",
                Category = "Construction",
                Description = "Calculate stair dimensions, rise, run, and stringers",
                OpenAction = () => new StairCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Roofing Calculator",
                Category = "Construction",
                Description = "Calculate roof area, pitch, and shingle requirements",
                OpenAction = () => new RoofingCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Paint/Coverage",
                Category = "Construction",
                Description = "Calculate paint needed for walls and ceilings",
                OpenAction = () => new PaintCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Board Feet",
                Category = "Construction",
                Description = "Calculate board feet for lumber",
                OpenAction = () => new BoardFeetCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Drywall Calculator",
                Category = "Construction",
                Description = "Calculate drywall sheets needed for rooms",
                OpenAction = () => new DrywallCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Grading/Slope",
                Category = "Construction",
                Description = "Convert between rise/run, percent, and angle",
                OpenAction = () => new GradingCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            }
        };
    }
    
    private void UpdateDisplay()
    {
        string searchText = SearchTextBox.Text.ToLower();
        
        var filtered = string.IsNullOrWhiteSpace(searchText)
            ? allCalculators
            : allCalculators.Where(c => 
                c.Name.ToLower().Contains(searchText) || 
                c.Description.ToLower().Contains(searchText) ||
                c.Category.ToLower().Contains(searchText)).ToList();
        
        var grouped = filtered
            .GroupBy(c => c.Category)
            .OrderBy(g => g.Key == "Common" ? 0 : g.Key == "Measurement & Layout" ? 1 : 2)
            .SelectMany(g => g.OrderBy(c => c.Name))
            .ToList();
        
        CalculatorItemsControl.ItemsSource = grouped;
    }
    
    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateDisplay();
    }
    
    private void CalculatorTile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is CalculatorDescriptor descriptor)
        {
            descriptor.OpenAction?.Invoke();
        }
    }
}

public class CalculatorDescriptor
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public System.Action? OpenAction { get; set; }
}
