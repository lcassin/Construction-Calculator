using ConstructionCalculatorMAUI.Shared.Help;

namespace ConstructionCalculatorMAUI.Pages;

[QueryProperty(nameof(KindParameter), "kind")]
public partial class HelpPage : ContentPage
{
    private string? _kindParameter;
    
    public string? KindParameter
    {
        get => _kindParameter;
        set
        {
            _kindParameter = value;
            if (!string.IsNullOrEmpty(value) && Enum.TryParse<CalculatorKind>(value, out var kind))
            {
                LoadHelp(kind);
            }
        }
    }

    public HelpPage()
    {
        InitializeComponent();
    }

    public HelpPage(CalculatorKind kind) : this()
    {
        LoadHelp(kind);
    }

    private void LoadHelp(CalculatorKind kind)
    {
        var helpTopic = HelpContentProvider.GetHelpTopic(kind);

        // Set title and summary
        TitleLabel.Text = helpTopic.Title;
        SummaryLabel.Text = helpTopic.Summary;

        // Load expected inputs
        ExpectedInputsList.Children.Clear();
        if (helpTopic.ExpectedInputs.Count > 0)
        {
            foreach (var input in helpTopic.ExpectedInputs)
            {
                ExpectedInputsList.Children.Add(new Label
                {
                    Text = $"• {input}",
                    FontSize = 13
                });
            }
        }
        else
        {
            ExpectedInputsHeader.IsVisible = false;
        }

        // Load shortcuts
        ShortcutsList.Children.Clear();
        if (helpTopic.Shortcuts.Count > 0)
        {
            foreach (var shortcut in helpTopic.Shortcuts)
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(120) },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                grid.Add(new Label
                {
                    Text = shortcut.Key,
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold
                }, 0, 0);

                grid.Add(new Label
                {
                    Text = shortcut.Description,
                    FontSize = 13
                }, 1, 0);

                ShortcutsList.Children.Add(grid);
            }
        }
        else
        {
            ShortcutsHeader.IsVisible = false;
        }

        // Load examples
        ExamplesList.Children.Clear();
        if (helpTopic.Examples.Count > 0)
        {
            foreach (var example in helpTopic.Examples)
            {
                var exampleStack = new VerticalStackLayout { Spacing = 2 };

                exampleStack.Children.Add(new Label
                {
                    Text = $"Input: {example.Input}",
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold
                });

                exampleStack.Children.Add(new Label
                {
                    Text = $"Output: {example.Output}",
                    FontSize = 13,
                    TextColor = Colors.Green
                });

                if (!string.IsNullOrEmpty(example.Description))
                {
                    exampleStack.Children.Add(new Label
                    {
                        Text = example.Description,
                        FontSize = 12,
                        TextColor = Color.FromArgb("#808080")
                    });
                }

                ExamplesList.Children.Add(exampleStack);
            }
        }
        else
        {
            ExamplesHeader.IsVisible = false;
        }

        // Load tips
        TipsList.Children.Clear();
        if (helpTopic.Tips.Count > 0)
        {
            foreach (var tip in helpTopic.Tips)
            {
                // Check if this is a section header (ends with colon and no bullet)
                if (tip.EndsWith(":") && !tip.StartsWith("•"))
                {
                    TipsList.Children.Add(new Label
                    {
                        Text = tip,
                        FontSize = 13,
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 10, 0, 3)
                    });
                }
                else if (string.IsNullOrWhiteSpace(tip))
                {
                    // Add spacing for empty lines
                    TipsList.Children.Add(new BoxView
                    {
                        HeightRequest = 5,
                        Color = Colors.Transparent
                    });
                }
                else
                {
                    TipsList.Children.Add(new Label
                    {
                        Text = tip.StartsWith("•") ? tip : $"• {tip}",
                        FontSize = 13
                    });
                }
            }
        }
        else
        {
            TipsHeader.IsVisible = false;
        }
    }
}
