namespace ConstructionCalculatorMAUI.Pages;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(800);

        Application.Current!.MainPage = new AppShell();
    }
}
