namespace ConstructionCalculatorMAUI.Shared.Help;

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
            CalculatorKind.HVAC => GetHVACCalculatorHelp(),
            CalculatorKind.Plumbing => GetPlumbingCalculatorHelp(),
            CalculatorKind.Electrical => GetElectricalCalculatorHelp(),
            CalculatorKind.Flooring => GetFlooringCalculatorHelp(),
            _ => new HelpTopic { Title = "Help", Summary = "No help available for this calculator." }
        };
    }

    private static HelpTopic GetMainCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Construction Calculator",
            IconGlyph = "🔢",
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
                new("= Button", "Calculate / Evaluate expression"),
                new("Copy Button", "Copy result to clipboard"),
                new("Mode Button", "Toggle between Feet/Inches and Decimal mode"),
                new("C Button", "Clear all"),
                new("CE Button", "Clear entry"),
                new("Help Button", "Show this help")
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
                "Percentage operations: A + B% adds B% of A, A - B% subtracts B% of A, A × B% multiplies by B%, A ÷ B% divides by B%",
                "Press Mode or Ctrl+M to switch between feet/inches and decimal display",
                "",
                "USING MEMORY KEYS (MC, MR, M+, M-):",
                "Memory keys help you handle grouped calculations like (3+6+5)×3 + (8+2)×4",
                "When memory is active, you'll see 'Memory: [value]' displayed above the calculator in green",
                "The memory indicator shows the exact value stored, in the same format as your display (feet/inches or decimal)",
                "",
                "MC (Memory Clear): Clears the memory to zero. The green memory indicator disappears.",
                "MR (Memory Recall): Displays the value stored in memory. Use this to see your accumulated total.",
                "M+ (Memory Add): Adds the current display value to memory. Use this to accumulate subtotals.",
                "M- (Memory Subtract): Subtracts the current display value from memory.",
                "",
                "EXAMPLE: Calculate (3+6+5)×3 + (8+2)×4",
                "1. Enter: 3 + 6 + 5 = 14",
                "2. Enter: × 3 = 42",
                "3. Press M+ (stores 42, you'll see 'Memory: 42' in green)",
                "4. Enter: 8 + 2 = 10",
                "5. Enter: × 4 = 40",
                "6. Press M+ (adds 40 to memory, now shows 'Memory: 82')",
                "7. Press MR to see the final result: 82",
                "8. Press MC when done to clear memory",
                "",
                "Memory works with feet/inches measurements too! Example: (12' 6\" + 8' 3\")×2 + (5' 9\" + 3' 4\")×3",
                "The memory indicator always shows the stored value so you don't have to remember it"
            }
        };
    }

    private static HelpTopic GetUnitConverterHelp()
    {
        return new HelpTopic
        {
            Title = "Unit Converter",
            IconGlyph = "🔄",
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
                new("Help Button", "Show this help")
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
            Summary = "Calculate the amount of concrete needed for slabs, footings, walls, and other concrete pours. Accounts for waste percentage and includes cost estimation.",
            ExpectedInputs = new List<string>
            {
                "Length: in feet (e.g., 20)",
                "Width: in feet (e.g., 10)",
                "Depth/Thickness: in inches (e.g., 4)",
                "Waste %: percentage for overage (default: 10%)",
                "Cost per Cubic Yard: optional, for cost estimation",
                "All fields accept decimal values"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Help Button", "Show this help")
            },
            Examples = new List<ExampleItem>
            {
                new("20' × 10' × 4\" slab", "2.47 cubic yards", "With 10% waste"),
                new("30' × 3' × 12\" footing", "3.33 cubic yards", "With 10% waste"),
                new("100' × 8\" × 8\" wall", "19.75 cubic yards", "With 10% waste"),
                new("3 cubic yards at $150/yard", "$450 total cost", "Cost estimation")
            },
            Tips = new List<string>
            {
                "Default waste percentage is 10% - adjust based on your needs",
                "Depth/thickness is always in inches, length and width in feet",
                "Result is shown in cubic yards (standard concrete ordering unit)",
                "Enter cost per cubic yard to see estimated total cost",
                "Cost updates automatically as you type",
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
            Summary = "Calculate roofing materials needed including shingles and underlayment with cost estimation.",
            ExpectedInputs = new List<string> { "Roof length and width", "Roof pitch (e.g., 6:12)", "Waste percentage", "Cost per Bundle: optional, for cost estimation" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("30' × 40' roof, 6:12 pitch", "13.42 squares", "With 10% waste"), new("40 bundles at $35/bundle", "$1,400 total cost", "Cost estimation") },
            Tips = new List<string> { "1 square = 100 square feet", "Steeper pitches require more material", "Enter cost per bundle to see estimated total cost", "Cost updates automatically as you type" }
        };
    }

    private static HelpTopic GetPaintCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Paint Calculator",
            IconGlyph = "🎨",
            Summary = "Calculate paint needed for walls, ceilings, and trim with cost estimation.",
            ExpectedInputs = new List<string> { "Wall/ceiling dimensions", "Number of coats", "Coverage rate", "Cost per Gallon: optional, for cost estimation" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("12' × 10' × 8' room, 2 coats", "~3 gallons", "Standard room"), new("3 gallons at $40/gallon", "$120 total cost", "Cost estimation") },
            Tips = new List<string> { "Standard coverage: 350-400 sq ft per gallon", "Enter cost per gallon to see estimated total cost", "Cost updates automatically as you type", "Always buy slightly more" }
        };
    }

    private static HelpTopic GetBoardFeetCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Board Feet Calculator",
            IconGlyph = "🪵",
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
            Summary = "Calculate drywall sheets needed for walls and ceilings with cost estimation.",
            ExpectedInputs = new List<string> { "Wall/ceiling dimensions", "Sheet size (4×8, 4×10, 4×12)", "Waste percentage", "Cost per Sheet: optional, for cost estimation" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("12' × 8' wall, 4×8 sheets", "3 sheets", "With 10% waste"), new("15 sheets at $12/sheet", "$180 total cost", "Cost estimation") },
            Tips = new List<string> { "Standard sheet: 4' × 8' (32 sq ft)", "Add 10-15% for waste", "Enter cost per sheet to see estimated total cost", "Cost updates automatically as you type" }
        };
    }

    private static HelpTopic GetGradingCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Grading Calculator",
            IconGlyph = "⛰",
            Summary = "Calculate slope, grade percentage, and cut/fill volumes for site grading.",
            ExpectedInputs = new List<string> { "Horizontal distance (run)", "Vertical distance (rise)" },
            Shortcuts = new List<ShortcutItem> { new("F1", "Show this help") },
            Examples = new List<ExampleItem> { new("Run: 100', Rise: 2'", "2% grade", "Minimum drainage slope") },
            Tips = new List<string> { "Minimum grade for drainage: 2%", "1% grade = 1 foot rise per 100 feet run" }
        };
    }

    private static HelpTopic GetHVACCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "HVAC Multi-Zone Calculator",
            IconGlyph = "❄",
            Summary = "Calculate heating and cooling requirements for multiple zones including BTU, tonnage, CFM, and duct sizing. Add multiple zones with different conditions to determine total system requirements with optional diversity factor.",
            ExpectedInputs = new List<string>
            {
                "Zone Name (optional, e.g., 'Living Room', 'Bedroom 1')",
                "Room Length (feet)",
                "Room Width (feet)",
                "Room Height (feet, typically 8-10')",
                "Insulation Level (Poor/Average/Good/Excellent)",
                "Sun Exposure (Shaded/Moderate/High)",
                "Climate Zone (Cold/Moderate/Hot)",
                "Number of Windows",
                "Number of Occupants",
                "Kitchen/Appliances (Yes/No checkbox)",
                "Diversity Factor (optional, typically 80-90% for multi-zone)",
                "For Duct Sizing: CFM and Air Velocity"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Help Button", "Show this help"),
                new("Tab", "Navigate between fields"),
                new("Enter", "Add zone (when in Add Zone section)")
            },
            Examples = new List<ExampleItem>
            {
                new("Living Room: 20' × 15' × 8', Average insulation, Moderate sun, 2 windows, 2 occupants", "Heating: 7,200 BTU, Cooling: 8,400 BTU, 280 CFM", "Standard living room zone"),
                new("Kitchen: 12' × 10' × 8', Good insulation, High sun, 1 window, 2 occupants, Kitchen=Yes", "Heating: 5,040 BTU, Cooling: 8,280 BTU, 276 CFM", "Kitchen zone with cooking load"),
                new("3 zones totaling 1,200 sq ft with 85% diversity", "Total: 28,800 BTU cooling → 24,480 BTU with diversity (2.04 tons)", "Multi-zone system with diversity"),
                new("Duct Sizing: 800 CFM at 800 FPM", "10×14\" rectangular duct", "Main trunk duct sizing")
            },
            Tips = new List<string>
            {
                "Add multiple zones: Enter each zone's details and click 'Add Zone'",
                "Zone names help identify rooms in results (optional, auto-numbered if blank)",
                "Base calculation: 20 BTU per square foot",
                "Insulation multipliers: Poor (1.3×), Average (1.0×), Good (0.85×), Excellent (0.7×)",
                "Sun exposure multipliers: Shaded (0.9×), Moderate (1.0×), High (1.15×)",
                "Climate multipliers: Cold (0.9×), Moderate (1.0×), Hot (1.2×)",
                "Add 1,000 BTU per window",
                "Add 400 BTU per occupant (heating), 600 BTU per occupant (cooling)",
                "Add 2,000 BTU (heating) or 4,000 BTU (cooling) for kitchens",
                "Diversity factor: Not all zones peak simultaneously (typically 80-90%)",
                "Apply diversity factor for more realistic multi-zone system sizing",
                "Tonnage = Cooling BTU ÷ 12,000",
                "CFM = Cooling BTU ÷ 30",
                "Total CFM auto-fills duct sizing calculator",
                "Standard air velocity: 700-900 FPM for supply ducts, 500-700 FPM for return ducts",
                "Always round up equipment size to next available unit",
                "Remove zones individually or clear all at once",
                "Copy results includes all zones and totals for easy sharing"
            }
        };
    }

    private static HelpTopic GetPlumbingCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Plumbing Calculator",
            IconGlyph = "🚰",
            Summary = "Calculate pipe sizing, flow rates, pressure loss, and drain/vent sizing for plumbing systems. Includes three calculation modes: Pipe Sizing, DWV (Drain-Waste-Vent), and Flow Rate.",
            ExpectedInputs = new List<string>
            {
                "Pipe Sizing Tab: Fixture Units, Pipe Length, Material, Pressure",
                "DWV Tab: Drainage Fixture Units (DFU), Drain Type, Slope",
                "Flow Rate Tab: Pipe Diameter, Pressure, Pipe Length"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Help Button", "Show this help"),
                new("Tab", "Navigate between fields"),
                new("Ctrl+Tab", "Switch between tabs")
            },
            Examples = new List<ExampleItem>
            {
                new("Pipe Sizing: 10 fixture units, 50' copper pipe, 60 PSI", "3/4\" pipe, 12.6 GPM, 5.2 ft/s, 8.5 PSI loss", "Residential branch line"),
                new("DWV: 20 DFU, Horizontal Branch, 1/4\" per foot slope", "3\" drain, 2\" vent, 42 DFU capacity", "Bathroom group drain"),
                new("Flow Rate: 1\" pipe, 60 PSI, 100' length", "15.8 GPM, 5.1 ft/s, 12.3 PSI loss", "Supply line flow")
            },
            Tips = new List<string>
            {
                "Fixture Units: Standardized load values (toilet=4 FU, sink=1 FU, shower=2 FU)",
                "Flow rate formula: √(Fixture Units) × 4.0 GPM",
                "Pipe sizing: 1/2\" (≤4 GPM), 3/4\" (≤10 GPM), 1\" (≤20 GPM), 1-1/4\" (≤35 GPM), 1-1/2\" (≤50 GPM)",
                "Friction factors: Copper (0.85), PEX (0.75), CPVC (0.90), Galvanized (1.2)",
                "Velocity should be 5-8 ft/s for supply lines",
                "Maximum pressure loss: 5-10 PSI per 100 feet",
                "DFU (Drainage Fixture Units): Similar to supply fixture units but for drains",
                "Drain types: Horizontal Branch, Vertical Stack, Building Drain",
                "Minimum drain slope: 1/4\" per foot for 3\" and smaller, 1/8\" per foot for 4\" and larger",
                "Vent sizing: Typically 1/2 the drain size, minimum 1-1/4\"",
                "Horizontal branch: Short runs connecting fixtures to stacks",
                "Vertical stack: Main vertical drain pipe",
                "Building drain: Main horizontal drain below fixtures",
                "Always check local plumbing codes for specific requirements"
            }
        };
    }

    private static HelpTopic GetElectricalCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Electrical Load Calculator",
            IconGlyph = "⚡",
            Summary = "Calculate electrical panel load, circuit requirements, wire sizing, and breaker sizing based on NEC 2023. Add multiple circuits to determine total load, apply demand factors, and get panel sizing recommendations.",
            ExpectedInputs = new List<string>
            {
                "Circuit Type: General Lighting, Small Appliance, Laundry, Appliances, HVAC, EV Charger, etc.",
                "For Lighting: Square footage (NEC: 3 VA per sq ft)",
                "For Appliances: Load in watts/VA and voltage (120V or 240V)",
                "For Wire Sizing: Circuit load, voltage, wire length, wire type (copper/aluminum)"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Help Button", "Show this help"),
                new("Tab", "Navigate between fields"),
                new("Enter", "Add circuit")
            },
            Examples = new List<ExampleItem>
            {
                new("General Lighting: 2000 sq ft", "6000 VA load, 120V, 50A, 12 AWG wire, 15A breaker", "Whole house lighting"),
                new("Small Appliance: 2 circuits", "3000 VA load (1500 VA each), 120V, 12 AWG, 20A breaker", "Kitchen receptacles"),
                new("Electric Range: 12000W, 240V", "12000 VA, 50A, 6 AWG wire, 50A breaker", "Kitchen range"),
                new("EV Charger: 9600W, 240V", "9600 VA (min 7200 per NEC), 40A, 8 AWG, 40A breaker", "Level 2 EVSE"),
                new("Total Panel: Multiple circuits", "200A service panel recommended", "Based on demand load calculation")
            },
            Tips = new List<string>
            {
                "NEC 2023 compliance: All calculations follow current National Electrical Code",
                "Demand factors: Automatically applied per NEC Article 220",
                "General lighting: 100% first 3kVA, 35% next 117kVA (NEC 220.42)",
                "Small appliance circuits: Minimum 2 required for kitchen (NEC 220.52)",
                "Laundry circuit: Minimum 1500 VA required (NEC 220.52)",
                "EV Charger: Minimum 7200 VA or nameplate rating, whichever is larger (NEC 220.57)",
                "Electric range: 80% demand factor typically applied (NEC Table 220.55)",
                "Wire sizing: Based on NEC Table 310.16 ampacity ratings",
                "Continuous loads: 125% factor applied for wire and breaker sizing",
                "Voltage drop: Should not exceed 3% for branch circuits (NEC 210.19)",
                "Standard breaker sizes: 15A, 20A, 30A, 40A, 50A, 60A, 70A, 80A, 90A, 100A, etc.",
                "Copper vs Aluminum: Copper has better conductivity, aluminum requires larger wire",
                "Wire sizes: 14 AWG (15A max), 12 AWG (20A max), 10 AWG (30A max), 8 AWG (40A max)",
                "Panel sizing: 100A, 125A, 150A, 200A, 225A, 300A, 400A service panels",
                "Add all circuits: Calculator sums loads and applies NEC demand factors",
                "Individual wire sizing: Calculate wire size for any circuit based on load and length",
                "Always consult licensed electrician: Final design must be reviewed by professional",
                "Local codes: May have additional requirements beyond NEC",
                "Copy results: Export complete calculation summary to clipboard"
            }
        };
    }

    private static HelpTopic GetFlooringCalculatorHelp()
    {
        return new HelpTopic
        {
            Title = "Flooring & Counter Calculator",
            IconGlyph = "🔲",
            Summary = "Calculate material quantities needed for flooring and countertop installations including waste/overage factors and cost estimation. Supports multiple sections and various material types.",
            ExpectedInputs = new List<string>
            {
                "Length (feet and inches)",
                "Width (feet and inches)",
                "Material Type (Tile, Hardwood, Laminate, Vinyl, Carpet, Granite/Quartz, etc.)",
                "Waste Factor (percentage, auto-set based on material)",
                "Cost per Square Foot (optional, for cost estimation)"
            },
            Shortcuts = new List<ShortcutItem>
            {
                new("Help Button", "Show this help"),
                new("Tab", "Navigate between fields"),
                new("Enter", "Add section")
            },
            Examples = new List<ExampleItem>
            {
                new("Kitchen: 12' × 10', Tile 12×12, 10% waste", "120 sq ft area, 132 sq ft material needed", "Ceramic tile floor"),
                new("Living Room: 20' × 15', Hardwood, 7% waste", "300 sq ft area, 321 sq ft material needed", "Hardwood flooring"),
                new("Counter: 8' × 2', Granite, 15% waste", "16 sq ft area, 18.4 sq ft material needed", "Kitchen countertop"),
                new("Multiple sections: Kitchen 120 sq ft + Dining 150 sq ft", "270 sq ft total, 297 sq ft with 10% waste", "Combined areas")
            },
            Tips = new List<string>
            {
                "Smart waste defaults: Tile (10%), Hardwood/Laminate/Vinyl (7%), Carpet (5%), Granite/Quartz (15%)",
                "Tile waste accounts for cuts, breakage, and pattern matching",
                "Hardwood waste covers end cuts and board defects",
                "Carpet waste is lower due to large roll sizes",
                "Granite/Quartz waste is higher due to seams and fabrication",
                "Add multiple sections for rooms with different areas",
                "Material types: Tile 12×12, Tile 18×18, Tile 24×24, Hardwood, Laminate, Vinyl, Carpet, Granite/Quartz, Custom",
                "Always round up when ordering materials",
                "Consider extra material for future repairs",
                "Cost calculation: Total Material Needed × Cost per Sq Ft",
                "Remove sections individually or clear all at once",
                "Copy results to clipboard for easy sharing"
            }
        };
    }
}
