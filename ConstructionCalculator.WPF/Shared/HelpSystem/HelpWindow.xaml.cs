using System.Windows;

namespace ConstructionCalculator.WPF.Shared.HelpSystem;

public partial class HelpWindow : Window
{
    public HelpWindow(CalculatorKind calculatorKind)
    {
        InitializeComponent();
        LoadHelpContent(calculatorKind);
    }

    private void LoadHelpContent(CalculatorKind calculatorKind)
    {
        var helpTopic = HelpContentProvider.GetHelpTopic(calculatorKind);

        IconTextBlock.Text = helpTopic.IconGlyph;
        IconTextBlock.FontFamily = new System.Windows.Media.FontFamily(helpTopic.IconFontFamily);
        TitleTextBlock.Text = helpTopic.Title;
        Title = $"Help - {helpTopic.Title}";

        SummaryTextBlock.Text = helpTopic.Summary;

        if (helpTopic.ExpectedInputs.Count > 0)
        {
            ExpectedInputsHeader.Visibility = Visibility.Visible;
            ExpectedInputsItemsControl.ItemsSource = helpTopic.ExpectedInputs;
            ExpectedInputsItemsControl.Visibility = Visibility.Visible;
        }
        else
        {
            ExpectedInputsHeader.Visibility = Visibility.Collapsed;
            ExpectedInputsItemsControl.Visibility = Visibility.Collapsed;
        }

        if (helpTopic.Shortcuts.Count > 0)
        {
            ShortcutsHeader.Visibility = Visibility.Visible;
            ShortcutsItemsControl.ItemsSource = helpTopic.Shortcuts;
            ShortcutsItemsControl.Visibility = Visibility.Visible;
        }
        else
        {
            ShortcutsHeader.Visibility = Visibility.Collapsed;
            ShortcutsItemsControl.Visibility = Visibility.Collapsed;
        }

        if (helpTopic.Examples.Count > 0)
        {
            ExamplesHeader.Visibility = Visibility.Visible;
            ExamplesItemsControl.ItemsSource = helpTopic.Examples;
            ExamplesItemsControl.Visibility = Visibility.Visible;
        }
        else
        {
            ExamplesHeader.Visibility = Visibility.Collapsed;
            ExamplesItemsControl.Visibility = Visibility.Collapsed;
        }

        if (helpTopic.Tips.Count > 0)
        {
            TipsHeader.Visibility = Visibility.Visible;
            TipsItemsControl.ItemsSource = helpTopic.Tips;
            TipsItemsControl.Visibility = Visibility.Visible;
        }
        else
        {
            TipsHeader.Visibility = Visibility.Collapsed;
            TipsItemsControl.Visibility = Visibility.Collapsed;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
