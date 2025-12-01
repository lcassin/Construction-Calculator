using ConstructionCalculator;
using ConstructionCalculatorMAUI.Shared.Help;
using System.Collections.ObjectModel;

namespace ConstructionCalculatorMAUI.Pages;

public partial class AreaCalculatorPage : ContentPage
{
    private readonly ObservableCollection<string> _sectionDisplays = new();

    private readonly List<(Measurement length, Measurement width, double sqft)> _sections = new();

    public AreaCalculatorPage()
    {
        InitializeComponent();
        SectionsCollection.ItemsSource = _sectionDisplays;
    }

    private void OnLengthCompleted(object sender, EventArgs e)
    {
        WidthEntry.Focus();
    }

    private async void OnAddSectionClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LengthEntry.Text) || string.IsNullOrWhiteSpace(WidthEntry.Text))
            {
                await DisplayAlert("Input Required", "Please enter both length and width.", "OK");
                return;
            }

            Measurement length = Measurement.Parse(LengthEntry.Text);
            Measurement width = Measurement.Parse(WidthEntry.Text);

            double lengthFeet = length.ToTotalInches() / 12.0;
            double widthFeet = width.ToTotalInches() / 12.0;
            double sqft = lengthFeet * widthFeet;

            _sections.Add((length, width, sqft));

            string displayText = $"{length.ToFractionString()} × {width.ToFractionString()} = {sqft:F2} sq ft";
            _sectionDisplays.Add(displayText);

            UpdateTotal();

            LengthEntry.Text = "";
            WidthEntry.Text = "";
            LengthEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnRemoveSectionClicked(object sender, EventArgs e)
    {
        var selectedItem = SectionsCollection.SelectedItem as string;
        if (selectedItem != null)
        {
            int index = _sectionDisplays.IndexOf(selectedItem);
            if (index >= 0)
            {
                _sections.RemoveAt(index);
                _sectionDisplays.RemoveAt(index);
                UpdateTotal();
            }
        }
        else
        {
            await DisplayAlert("Selection Required", "Please select a section to remove.", "OK");
        }
    }

    private void OnClearAllClicked(object sender, EventArgs e)
    {
        _sections.Clear();
        _sectionDisplays.Clear();
        UpdateTotal();
        LengthEntry.Text = "";
        WidthEntry.Text = "";
    }

    private async void OnCopyTotalClicked(object sender, EventArgs e)
    {
        double total = _sections.Sum(s => s.sqft);
        await Clipboard.SetTextAsync($"{total:F2}");
        await DisplayAlert("Copied", $"Copied: {total:F2} sq ft", "OK");
    }

    private void UpdateTotal()
    {
        double total = _sections.Sum(s => s.sqft);
        TotalAreaLabel.Text = $"Total Area: {total:F2} sq ft";
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPage(CalculatorKind.Area));
    }
}
