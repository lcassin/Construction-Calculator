using ConstructionCalculatorMAUI.Shared.Help;

namespace ConstructionCalculatorMAUI.Models;

public class CalculatorDescriptor
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public string IconFontFamily { get; set; } = "Segoe UI Emoji";
    public CalculatorKind HelpKind { get; set; }
    public string Route { get; set; } = "";
}
