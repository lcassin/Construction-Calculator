using System.Collections.Generic;

namespace ConstructionCalculator.WPF;

public class HelpTopic
{
    public string Title { get; set; } = "";
    public string IconGlyph { get; set; } = "";
    public string IconFontFamily { get; set; } = "Segoe UI Emoji";
    public string Summary { get; set; } = "";
    public List<string> ExpectedInputs { get; set; } = new();
    public List<ShortcutItem> Shortcuts { get; set; } = new();
    public List<ExampleItem> Examples { get; set; } = new();
    public List<string> Tips { get; set; } = new();
}

public class ShortcutItem
{
    public string Keys { get; set; } = "";
    public string Action { get; set; } = "";
    
    public ShortcutItem(string keys, string action)
    {
        Keys = keys;
        Action = action;
    }
}

public class ExampleItem
{
    public string Input { get; set; } = "";
    public string Result { get; set; } = "";
    public string? Notes { get; set; }
    
    public ExampleItem(string input, string result, string? notes = null)
    {
        Input = input;
        Result = result;
        Notes = notes;
    }
}

public enum CalculatorKind
{
    Main,
    Angle,
    Stair,
    Survey,
    SeatingLayout,
    Area,
    UnitConverter,
    Concrete,
    Roofing,
    Paint,
    BoardFeet,
    Drywall,
    Grading
}
