using ConstructionCalculatorMAUI.Shared.Help;
using ConstructionCalculatorMAUI.Models;
using System.Collections.ObjectModel;

namespace ConstructionCalculatorMAUI.Pages;

public partial class CalculatorHubPage : ContentPage
{
    public ObservableCollection<CalculatorDescriptor> Calculators { get; set; }

    public CalculatorHubPage()
    {
        InitializeComponent();
        InitializeCalculators();
        BindingContext = this;
    }

    private void InitializeCalculators()
    {
        Calculators = new ObservableCollection<CalculatorDescriptor>
        {
            // Common Tools
            new CalculatorDescriptor
            {
                Name = "Unit Converter",
                Category = "Common Tools",
                Description = "Convert between units: length, area, volume, weight, temperature",
                Icon = "⇄",
                HelpKind = CalculatorKind.UnitConverter,
                Route = "UnitConverter"
            },
            new CalculatorDescriptor
            {
                Name = "Area Calculator",
                Category = "Common Tools",
                Description = "Calculate areas of multiple sections and sum totals",
                Icon = "⬛",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Area,
                Route = "AreaCalculator"
            },
            
            // Geometry & Site
            new CalculatorDescriptor
            {
                Name = "Angle Calculator",
                Category = "Geometry & Site",
                Description = "Calculate angles from rise/run and solve right triangles",
                Icon = "∠",
                HelpKind = CalculatorKind.Angle,
                Route = "AngleCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Survey Calculator",
                Category = "Geometry & Site",
                Description = "Convert bearings/azimuths and calculate coordinates",
                Icon = "🧭",
                HelpKind = CalculatorKind.Survey,
                Route = "SurveyCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Grading/Slope",
                Category = "Geometry & Site",
                Description = "Convert between rise/run, percent, and angle",
                Icon = "⛰",
                HelpKind = CalculatorKind.Grading,
                Route = "GradingCalculator"
            },
            
            // Building Elements & Layout
            new CalculatorDescriptor
            {
                Name = "Stair Calculator",
                Category = "Building Elements & Layout",
                Description = "Calculate stair dimensions, rise, run, and stringers",
                Icon = "⇧",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Stair,
                Route = "StairCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Accessibility Ramp",
                Category = "Building Elements & Layout",
                Description = "Calculate ADA-compliant ramp dimensions and landings",
                Icon = "♿",
                IconFontFamily = "Segoe UI Symbol",
                HelpKind = CalculatorKind.Ramp,
                Route = "RampCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Seating Layout",
                Category = "Building Elements & Layout",
                Description = "Calculate seating arrangements and spacing",
                Icon = "💺",
                HelpKind = CalculatorKind.SeatingLayout,
                Route = "SeatingLayout"
            },
            
            // Materials & Estimating
            new CalculatorDescriptor
            {
                Name = "Concrete Calculator",
                Category = "Materials & Estimating",
                Description = "Calculate cubic yards for slabs, footings, and columns",
                Icon = "🏗",
                HelpKind = CalculatorKind.Concrete,
                Route = "ConcreteCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Roofing Calculator",
                Category = "Materials & Estimating",
                Description = "Calculate roof area, pitch, and shingle requirements",
                Icon = "🏠",
                HelpKind = CalculatorKind.Roofing,
                Route = "RoofingCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Drywall Calculator",
                Category = "Materials & Estimating",
                Description = "Calculate drywall sheets needed for rooms",
                Icon = "🧱",
                HelpKind = CalculatorKind.Drywall,
                Route = "DrywallCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Paint Calculator",
                Category = "Materials & Estimating",
                Description = "Calculate paint needed for walls and ceilings",
                Icon = "🎨",
                HelpKind = CalculatorKind.Paint,
                Route = "PaintCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Flooring & Countertops",
                Category = "Materials & Estimating",
                Description = "Calculate material quantities with waste for flooring and counters",
                Icon = "🔲",
                HelpKind = CalculatorKind.Flooring,
                Route = "FlooringCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Board Feet",
                Category = "Materials & Estimating",
                Description = "Calculate board feet for lumber",
                Icon = "📏",
                IconFontFamily = "Segoe UI Emoji",
                HelpKind = CalculatorKind.BoardFeet,
                Route = "BoardFeetCalculator"
            },
            
            // Systems
            new CalculatorDescriptor
            {
                Name = "HVAC Calculator",
                Category = "Systems (HVAC & Plumbing)",
                Description = "Calculate BTU, tonnage, CFM, and duct sizing",
                Icon = "❄",
                HelpKind = CalculatorKind.HVAC,
                Route = "HVACCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Plumbing Calculator",
                Category = "Systems (HVAC & Plumbing)",
                Description = "Calculate pipe sizing, flow rates, and drain capacity",
                Icon = "🚰",
                HelpKind = CalculatorKind.Plumbing,
                Route = "PlumbingCalculator"
            },
            new CalculatorDescriptor
            {
                Name = "Electrical Load Calculator",
                Category = "Systems (HVAC & Plumbing)",
                Description = "Calculate panel load, wire sizing, and circuit requirements per NEC",
                Icon = "⚡",
                HelpKind = CalculatorKind.Electrical,
                Route = "ElectricalCalculator"
            }
        };
    }

    private async void OnCalculatorTileTapped(object sender, EventArgs e)
    {
        if (sender is Border border && border.BindingContext is CalculatorDescriptor descriptor)
        {
            await Shell.Current.GoToAsync($"//{descriptor.Route}");
        }
    }

    private async void OnHelpButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CalculatorDescriptor descriptor)
        {
            // Navigate to help page using Navigation.PushAsync (matching existing pattern in MainPage)
            await Navigation.PushAsync(new HelpPage(descriptor.HelpKind));
        }
    }
}
