namespace ConstructionCalculatorMAUI.Pages;

public partial class CalculatorHubPage : ContentPage
{
    public CalculatorHubPage()
    {
        InitializeComponent();
    }

    private async void OnCalculatorClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string route)
        {
            await Shell.Current.GoToAsync($"//{route}");
        }
    }
}
