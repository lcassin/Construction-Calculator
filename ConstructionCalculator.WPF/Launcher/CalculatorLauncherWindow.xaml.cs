using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConstructionCalculator.WPF.Calculators.UnitConverter;
using ConstructionCalculator.WPF.Calculators.Construction.Concrete;
using ConstructionCalculator.WPF.Calculators.Construction.Stair;
using ConstructionCalculator.WPF.Calculators.Construction.Ramp;
using ConstructionCalculator.WPF.Calculators.Construction.Drywall;
using ConstructionCalculator.WPF.Calculators.Construction.Grading;
using ConstructionCalculator.WPF.Calculators.Geometry.Area;
using ConstructionCalculator.WPF.Calculators.Geometry.Angle;
using ConstructionCalculator.WPF.Calculators.Geometry.Roofing;
using ConstructionCalculator.WPF.Calculators.Materials.Paint;
using ConstructionCalculator.WPF.Calculators.Materials.BoardFeet;
using ConstructionCalculator.WPF.Calculators.Survey;
using ConstructionCalculator.WPF.Calculators.SeatingLayout;
using ConstructionCalculator.WPF.Shared.HelpSystem;

namespace ConstructionCalculator.WPF.Launcher;

public partial class CalculatorLauncherWindow : Window
{
    private List<CalculatorDescriptor> allCalculators;
    private CalculatorDescriptor? _hoveredCalculator;
    
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
                Icon = "⇄",
                HelpKind = CalculatorKind.UnitConverter,
                OpenAction = () => new UnitConverterWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Area Calculator",
                Category = "Common",
                Description = "Calculate areas of multiple sections and sum totals",
                Icon = "⬛",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Area,
                OpenAction = () => new AreaCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Concrete Calculator",
                Category = "Construction",
                Description = "Calculate cubic yards for slabs, footings, and columns",
                Icon = "🏗",
                HelpKind = CalculatorKind.Concrete,
                OpenAction = () => new ConcreteCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Angle Calculator",
                Category = "Measurement & Layout",
                Description = "Calculate angles from rise/run and solve right triangles",
                Icon = "∠",
                HelpKind = CalculatorKind.Angle,
                OpenAction = () => new AngleCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Survey Calculator",
                Category = "Measurement & Layout",
                Description = "Convert bearings/azimuths and calculate coordinates",
                Icon = "🧭",
                HelpKind = CalculatorKind.Survey,
                OpenAction = () => new SurveyCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Seating Layout",
                Category = "Measurement & Layout",
                Description = "Calculate seating arrangements and spacing",
                Icon = "💺",
                HelpKind = CalculatorKind.SeatingLayout,
                OpenAction = () => new SeatingLayoutCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Stair Calculator",
                Category = "Construction",
                Description = "Calculate stair dimensions, rise, run, and stringers",
                Icon = "⇧",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Stair,
                OpenAction = () => new StairCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Accessibility Ramp",
                Category = "Construction",
                Description = "Calculate ADA-compliant ramp dimensions and landings",
                Icon = "♿",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Ramp,
                OpenAction = () => new RampCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Roofing Calculator",
                Category = "Construction",
                Description = "Calculate roof area, pitch, and shingle requirements",
                Icon = "🏠",
                HelpKind = CalculatorKind.Roofing,
                OpenAction = () => new RoofingCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Paint/Coverage",
                Category = "Construction",
                Description = "Calculate paint needed for walls and ceilings",
                Icon = "🎨",
                HelpKind = CalculatorKind.Paint,
                OpenAction = () => new PaintCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Board Feet",
                Category = "Construction",
                Description = "Calculate board feet for lumber",
                Icon = "📏",
                IconFontFamily = "Segoe UI Emoji",
                HelpKind = CalculatorKind.BoardFeet,
                OpenAction = () => new BoardFeetCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Drywall Calculator",
                Category = "Construction",
                Description = "Calculate drywall sheets needed for rooms",
                Icon = "🧱",
                HelpKind = CalculatorKind.Drywall,
                OpenAction = () => new DrywallCalculatorWindow { Owner = Application.Current.MainWindow }.Show()
            },
            new CalculatorDescriptor
            {
                Name = "Grading/Slope",
                Category = "Construction",
                Description = "Convert between rise/run, percent, and angle",
                Icon = "⛰",
                HelpKind = CalculatorKind.Grading,
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

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true; // Prevent the tile button from also firing
        
        if (sender is Button button && button.Tag is CalculatorDescriptor descriptor)
        {
            var helpWindow = new HelpWindow(descriptor.HelpKind) { Owner = this };
            helpWindow.Show();
        }
    }

    private void Tile_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is CalculatorDescriptor descriptor)
        {
            _hoveredCalculator = descriptor;
        }
    }

    private void Tile_MouseLeave(object sender, MouseEventArgs e)
    {
        _hoveredCalculator = null;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            e.Handled = true;
            
            if (_hoveredCalculator != null)
            {
                var helpWindow = new HelpWindow(_hoveredCalculator.HelpKind) { Owner = this };
                helpWindow.Show();
            }
        }
    }
}

public class CalculatorDescriptor
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public string IconFontFamily { get; set; } = "Segoe UI Emoji";
    public CalculatorKind HelpKind { get; set; }
    public System.Action? OpenAction { get; set; }
}
