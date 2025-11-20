using System.Collections.Generic;

namespace ConstructionCalculator.WPF;

public static class HelpContentProvider
{
    public static HelpTopic GetHelpTopic(CalculatorKind kind)
    {
        return kind switch
        {
            CalculatorKind.Main => GetMainCalculatorHelp(),
            CalculatorKind.UnitConverter => GetUnitConverterHelp(),
            CalculatorKind.Concrete => GetConcreteCalculatorHelp(),
            CalculatorKind.Angle => GetAngleCalculatorHelp(),
            CalculatorKind.Stair => GetStairCalculatorHelp(),
            CalculatorKind.Ramp => GetRampCalculatorHelp(),
            CalculatorKind.Survey => GetSurveyCalculatorHelp(),
            CalculatorKind.SeatingLayout => GetSeatingLayoutCalculatorHelp(),
            CalculatorKind.Area => GetAreaCalculatorHelp(),
            CalculatorKind.Roofing => GetRoofingCalculatorHelp(),
            CalculatorKind.Paint => GetPaintCalculatorHelp(),
            CalculatorKind.BoardFeet => GetBoardFeetCalculatorHelp(),
            CalculatorKind.Drywall => GetDrywallCalculatorHelp(),
            CalculatorKind.Grading => GetGradingCalculatorHelp(),
            _ => new HelpTopic { Title = "Help", Summary = "No help available for this calculator." }
        };
    }

    private static HelpTopic GetMainCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Construction Calculator",
            IconGlyph = "🔢",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "A specialized calculator for construction measurements supporting feet, inches, and fractions. Perform calculations with measurements in feet/inches format or decimal mode.",
            ExpectedInputs = new List<string>
            {
                "Feet and inches: 12' 6\" or 12'6\" or 12' 6",
                "Inches with fractions: 4-1/2\" or 4 1/2\" (dash means 'and')",
                "Decimal inches: 75.5",
                "Plain numbers: 100",
                "Percentages: 30% (in expressions like '100 * 30%')",
                "Spaces are optional between feet and inches"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Enter", "Calculate / Evaluate expression"),
                new("Ctrl+C", "Copy result to clipboard"),
                new("Ctrl+M", "Toggle between Feet/Inches and Decimal mode"),
                new("Esc", "Clear all (C)"),
                new("Ctrl+Z", "Clear entry (CE)"),
                new("Ctrl++", "Addition"),
                new("Ctrl+-", "Subtraction"),
                new("Ctrl+Shift+8", "Multiplication"),
                new("Ctrl+/", "Division"),
                new("Ctrl+Shift+5", "Percentage (%)"),
                new("Ctrl+R", "Square root (√)"),
                new("Ctrl+Shift+2", "Squared (x²)"),
                new("Ctrl+N", "Toggle sign (+/-)"),
                new("F1", "Show this help")
            },
            Examples = new List<ExampleItem>
            {
                new("12' 6\" + 8' 3\"", "20' 9\"", "Add measurements"),
                new("16' 3 1/2\" * 30%", "4' 10-13/16\"", "Multiply by percentage"),
                new("100 + 10%", "110", "Add 10% of 100 to 100"),
                new("50 - 20%", "40", "Subtract 20% of 50 from 50"),
                new("144", "12 (press Ctrl+R)", "Square root"),
                new("12", "144 (press Ctrl+Shift+2)", "Squared"),
                new("4-1/2", "4.5 inches", "Dash means 'and' in fractions")
            },
            Tips = new List<string>
            {
                "Use the Space, /, ', \" buttons for easier input of measurements",
                "The / button is for fractions (1/2), not division. Use ÷ for division",
                "Memory buttons (MC, MR, M+, M-) work with feet/inches measurements",
                "Percentage operations: A + B% adds B% of A, A - B% subtracts B% of A, A × B% multiplies by B%, A ÷ B% divides by B%",
                "Press Mode or Ctrl+M to switch between feet/inches and decimal display"
            }
        };
    }

    private static HelpTopic GetUnitConverterHelp()
    {
        return new HelpTopic
        {
            Title = "Unit Converter",
            IconGlyph = "🔄",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Convert between different units of measurement commonly used in construction, including length, area, volume, weight, and temperature.",
            ExpectedInputs = new List<string>
            {
                "Numeric values in the 'From' field",
                "Select conversion type from dropdown",
                "Select 'From' and 'To' units",
                "For Measurement Format: enter feet/inches (12' 6\") or decimal values"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("F1", "Show this help")
            },
            Examples = new List<ExampleItem>
            {
                new("Length: 12 feet", "144 inches", "Convert feet to inches"),
                new("Area: 100 sq ft", "9.29 sq meters", "Convert square feet to square meters"),
                new("Volume: 1 cubic yard", "27 cubic feet", "Convert cubic yards to cubic feet"),
                new("Weight: 2000 pounds", "1 ton", "Convert pounds to tons"),
                new("Temperature: 32°F", "0°C", "Convert Fahrenheit to Celsius"),
                new("Measurement Format: 6' 3 1/2\"", "75.5 inches", "Convert feet/inches to decimal inches")
            },
            Tips = new List<string>
            {
                "Measurement Format conversion is useful for converting between feet/inches and decimal formats",
                "The converter automatically updates the 'To' field as you type",
                "Changing the conversion type clears both fields",
                "All conversions use standard construction industry conversion factors"
            }
        };
    }

    private static HelpTopic GetConcreteCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Concrete Calculator",
            IconGlyph = "🏗",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate the amount of concrete needed for slabs, footings, walls, and other concrete pours. Accounts for waste percentage.",
            ExpectedInputs = new List<string>
            {
                "Length: in feet (e.g., 20)",
                "Width: in feet (e.g., 10)",
                "Depth/Thickness: in inches (e.g., 4)",
                "Waste %: percentage for overage (default: 10%)",
                "All fields accept decimal values"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("F1", "Show this help")
            },
            Examples = new List<ExampleItem>
            {
                new("20' × 10' × 4\" slab", "2.47 cubic yards", "With 10% waste"),
                new("30' × 3' × 12\" footing", "3.33 cubic yards", "With 10% waste"),
                new("100' × 8\" × 8\" wall", "19.75 cubic yards", "With 10% waste")
            },
            Tips = new List<string>
            {
                "Default waste percentage is 10% - adjust based on your needs",
                "Depth/thickness is always in inches, length and width in feet",
                "Result is shown in cubic yards (standard concrete ordering unit)",
                "Always round up when ordering concrete",
                "Consider adding extra for spillage and uneven subgrade"
            }
        };
    }

    private static HelpTopic GetAngleCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Angle Calculator",
            IconGlyph = "📐",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate angles, rise, run, and pitch for roofs, ramps, and stairs.",
            ExpectedInputs = new List<string> { "Rise, Run, or Angle values" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("Rise: 4, Run: 12", "Angle: 18.43°", "4:12 roof pitch") },
            Tips = new List<string> { "Common roof pitches: 4:12, 6:12, 8:12, 12:12" }
        };
    }

    private static HelpTopic GetStairCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Stair Calculator",
            IconGlyph = "🪜",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate stair dimensions including rise, run, and number of steps. Supports complex configurations with landings and multiple stair types (straight, L-shaped, U-shaped).",
            ExpectedInputs = new List<string> 
            { 
                "Total Rise (height)", 
                "Number of Risers", 
                "Stair Width (typically 36\" minimum)",
                "Landing Type (Straight/Right Angle/Full Return)",
                "Landing Depth (when using landings)",
                "Steps Before Landing (when using landings)"
            },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> 
            { 
                new("Total Rise: 108\", Risers: 14, Width: 36\"", "Riser: 7.71\", Run: 130\"", "Standard straight run"),
                new("108\" rise, 7 steps before landing, Straight", "Two 7-step flights", "Split with mid-landing"),
                new("108\" rise, L-shaped landing", "Length: 65\", Width: 101\"", "Right angle turn saves space"),
                new("108\" rise, U-shaped landing", "Length: 65\", Width: 72\"", "Switchback configuration")
            },
            Tips = new List<string> 
            { 
                "Ideal riser height: 7-7.75 inches for residential (IBC/IRC)",
                "Ideal tread depth: 10-11 inches minimum",
                "Commercial code: 4-7\" rise, 11\" minimum tread",
                "Stair width: 36\" minimum for residential, 44\" for commercial",
                "Landing Depth: The dimension you enter - how far the landing extends in the direction of travel",
                "Landing Width: Automatically equals the stair width (entered at top)",
                "Landing depth: Must be at least as wide as stair width (36\" minimum per code)",
                "Maximum rise between landings: 12 feet (144\") per IBC/IRC",
                "Straight landing: Both flights continue in same direction - landing adds to overall length",
                "Right Angle (L-shaped): Second flight turns 90° - saves space in corners",
                "Full Return (U-shaped): Flights parallel but opposite - best for tight vertical spaces",
                "U-shaped landing depth: Should be at least 2× stair width for safe 180° turning radius",
                "U-shaped geometry: Two parallel stair runs are separated by the landing depth",
                "Overall Length: Total horizontal run including all stair treads and landing depth",
                "Overall Width: Total perpendicular footprint dimension",
                "Overall Width for Straight: Equals stair width (landing doesn't add width)",
                "Overall Width for L-shaped: Landing depth + second flight run (perpendicular configuration)",
                "Overall Width for U-shaped: 2× stair width + landing depth (both parallel runs + landing between)",
                "Entry/Exit landings: NOT included in calculations - add separately to your space planning",
                "Code compliance: Calculator validates against IBC/IRC requirements",
                "Space planning: Use landing type to fit stairs in available footprint",
                "Space constraints: Enter available length/width to check if configuration fits",
                "Spiral staircase: Consider when no standard configuration fits available space",
                "Spiral code: 26\" clear width minimum, 7.5\" tread depth at 12\" from narrow end, 9.5\" max riser",
                "Auto-calculate: Automatically determines optimal number of steps for ideal riser height"
            }
        };
    }

    private static HelpTopic GetRampCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Accessibility Ramp Calculator",
            IconGlyph = "♿",
            IconFontFamily = "Segoe UI Symbol",
            Summary = "Calculate ADA-compliant accessibility ramp dimensions including slope, run length, and landing requirements. Supports complex configurations with landings for long runs.",
            ExpectedInputs = new List<string>
            {
                "Total Rise (vertical height to overcome)",
                "Slope Ratio (1:X format, default 1:12 for ADA)",
                "Ramp Width (typically 36\" minimum for ADA)",
                "Landing Type (Straight/Right Angle/Full Return)",
                "Available Space (optional, for fit checking)"
            },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem>
            {
                new("Rise: 24\", Slope: 1:12", "Run: 24', 0 landings", "ADA compliant, under 30' max"),
                new("Rise: 48\", Slope: 1:12", "Run: 48', 1 landing", "Requires landing at 30'"),
                new("Rise: 72\", Slope: 1:12, L-shaped", "Length: 36', Width: 96'", "Right angle saves space"),
                new("Rise: 96\", Slope: 1:12, U-shaped", "Length: 36', Width: 72'", "Switchback configuration")
            },
            Tips = new List<string>
            {
                "ADA compliance: Maximum slope 1:12 (1 inch rise per 12 inches run)",
                "Maximum run: 30 feet before landing required (ADA)",
                "Landing Depth: The dimension you enter - how far the landing extends in the direction of travel",
                "Landing Width: Automatically equals the ramp width (entered at top)",
                "Landing depth: 60\" minimum per ADA",
                "Landing width: Must be at least as wide as ramp (36\" minimum)",
                "Ramp width: 36\" minimum clear width for ADA compliance",
                "Less steep slopes: Can use 1:16 or 1:20 for easier access",
                "Straight landing: Both ramp segments continue in same direction - landing adds to overall length",
                "Right Angle (L-shaped): Second segment turns 90° - saves space in corners",
                "Full Return (U-shaped): Segments parallel but opposite - best for tight spaces",
                "U-shaped landing depth: Should be at least 2× ramp width for safe 180° turning radius",
                "U-shaped geometry: Two parallel ramp runs are separated by the landing depth",
                "Overall Length: Total horizontal run including all ramp segments and landing depth",
                "Overall Width: Total perpendicular footprint dimension",
                "Overall Width for Straight: Equals ramp width (landing doesn't add width)",
                "Overall Width for L-shaped: Landing depth + second segment run (perpendicular configuration)",
                "Overall Width for U-shaped: 2× ramp width + landing depth (both parallel runs + landing between)",
                "Entry/Exit landings: NOT included in calculations - add separately to your space planning",
                "Space constraints: Enter available length/width to check if configuration fits",
                "Handrails: Required on both sides for ramps over 6\" rise or 72\" run",
                "Edge protection: Required when drop-off exceeds 6\"",
                "Surface: Must be slip-resistant and stable"
            }
        };
    }

    private static HelpTopic GetSurveyCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Survey Calculator",
            IconGlyph = "🧭",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate distances, bearings, and coordinates for surveying and site layout.",
            ExpectedInputs = new List<string> { "Bearing: N 57° 00' E", "Distance in feet", "Northing/Easting coordinates" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("Bearing: N 45° E, Distance: 100'", "North: 70.71', East: 70.71'", "") },
            Tips = new List<string> { "Northing = Y coordinate", "Easting = X coordinate", "Bearings use quadrant notation: N/S angle E/W" }
        };
    }

    private static HelpTopic GetSeatingLayoutCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Seating Layout Calculator",
            IconGlyph = "💺",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate seating arrangements for events, classrooms, or meeting spaces.",
            ExpectedInputs = new List<string> { "Room dimensions", "Seat spacing" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("30' × 40' room, 3' spacing", "~133 seats", "Theater-style") },
            Tips = new List<string> { "Theater seating: 2-3 feet per person", "Classroom: 6-8 feet per person" }
        };
    }

    private static HelpTopic GetAreaCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Area Calculator",
            IconGlyph = "📏",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate areas for rectangles, triangles, and circles.",
            ExpectedInputs = new List<string> { "Rectangle: length and width", "Triangle: base and height", "Circle: radius" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("Rectangle: 20' × 15'", "300 sq ft", "Room area") },
            Tips = new List<string> { "Add 10% for waste when ordering materials", "1 square of roofing = 100 square feet" }
        };
    }

    private static HelpTopic GetRoofingCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Roofing Calculator",
            IconGlyph = "🏠",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate roofing materials needed including shingles and underlayment.",
            ExpectedInputs = new List<string> { "Roof length and width", "Roof pitch (e.g., 6:12)", "Waste percentage" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("30' × 40' roof, 6:12 pitch", "13.42 squares", "With 10% waste") },
            Tips = new List<string> { "1 square = 100 square feet", "Steeper pitches require more material" }
        };
    }

    private static HelpTopic GetPaintCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Paint Calculator",
            IconGlyph = "🎨",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate paint needed for walls, ceilings, and trim.",
            ExpectedInputs = new List<string> { "Wall/ceiling dimensions", "Number of coats", "Coverage rate" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("12' × 10' × 8' room, 2 coats", "~3 gallons", "Standard room") },
            Tips = new List<string> { "Standard coverage: 350-400 sq ft per gallon", "Always buy slightly more" }
        };
    }

    private static HelpTopic GetBoardFeetCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Board Feet Calculator",
            IconGlyph = "🪵",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate board feet for lumber pricing and ordering.",
            ExpectedInputs = new List<string> { "Thickness (inches)", "Width (inches)", "Length (feet)", "Quantity" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("2\" × 4\" × 8', qty: 10", "53.33 board feet", "Ten 2×4×8 studs") },
            Tips = new List<string> { "Formula: (Thickness × Width × Length) ÷ 12 × Quantity", "Use nominal dimensions" }
        };
    }

    private static HelpTopic GetDrywallCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Drywall Calculator",
            IconGlyph = "🧱",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate drywall sheets needed for walls and ceilings.",
            ExpectedInputs = new List<string> { "Wall/ceiling dimensions", "Sheet size (4×8, 4×10, 4×12)", "Waste percentage" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("12' × 8' wall, 4×8 sheets", "3 sheets", "With 10% waste") },
            Tips = new List<string> { "Standard sheet: 4' × 8' (32 sq ft)", "Add 10-15% for waste" }
        };
    }

    private static HelpTopic GetGradingCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Grading Calculator",
            IconGlyph = "⛰",
            IconFontFamily = "Segoe UI Emoji",
            Summary = "Calculate slope, grade percentage, and cut/fill volumes for site grading.",
            ExpectedInputs = new List<string> { "Horizontal distance (run)", "Vertical distance (rise)" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("Run: 100', Rise: 2'", "2% grade", "Minimum drainage slope") },
            Tips = new List<string> { "Minimum grade for drainage: 2%", "1% grade = 1 foot rise per 100 feet run" }
        };
    }
}
