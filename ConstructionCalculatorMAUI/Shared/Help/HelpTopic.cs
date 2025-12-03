namespace ConstructionCalculatorMAUI.Shared.Help;

public class HelpTopic
{
    public string Title { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = string.Empty;
    public string IconFontFamily { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> ExpectedInputs { get; set; } = new();
    public List<ShortcutItem> Shortcuts { get; set; } = new();
    public List<ExampleItem> Examples { get; set; } = new();
    public List<string> Tips { get; set; } = new();
}

public class ShortcutItem
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ShortcutItem() { }

    public ShortcutItem(string key, string description)
    {
        Key = key;
        Description = description;
    }
}

public class ExampleItem
{
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ExampleItem() { }

    public ExampleItem(string input, string output, string description)
    {
        Input = input;
        Output = output;
        Description = description;
    }
}
