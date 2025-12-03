namespace ConstructionCalculatorMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        LoadThemePreference();
        ApplyThemeResources(RequestedTheme);

        RequestedThemeChanged += (s, e) => ApplyThemeResources(e.RequestedTheme);

        // Remove MainPage assignment here to avoid CS0618
        // MainPage = new Pages.SplashPage();

        // Fix AsyncFixer03: Use a non-async lambda and call a local async Task method, handling exceptions
        Dispatcher.Dispatch(() =>
        {
            _ = ShowMainPageAfterDelayAsync();
        });
    }

    private async Task ShowMainPageAfterDelayAsync()
    {
        try
        {
            await Task.Delay(1200); // Show splash for 1.2 seconds
            // Use Windows[0].Page to set the root page at runtime
            if (Windows.Count > 0)
            {
                Windows[0].Page = new AppShell();
            }
        }
        catch (Exception ex)
        {
            // Optionally log or handle the exception
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Create a new Window
        var window = new Window
        {
            Width = 472,
            Height = 750,
            Page = new Pages.SplashPage() // Set the initial page here
        };

        return window;
    }

    private void LoadThemePreference()
    {
        string? theme = Preferences.Get("AppTheme", "System");

        switch (theme)
        {
            case "Light":
                UserAppTheme = AppTheme.Light;
                break;
            case "Dark":
                UserAppTheme = AppTheme.Dark;
                break;
            default:
                UserAppTheme = AppTheme.Unspecified; // Follow system theme
                break;
        }
    }

    public static void SetTheme(string theme)
    {
        Preferences.Set("AppTheme", theme);

        switch (theme)
        {
            case "Light":
                Current!.UserAppTheme = AppTheme.Light;
                break;
            case "Dark":
                Current!.UserAppTheme = AppTheme.Dark;
                break;
            default:
                Current!.UserAppTheme = AppTheme.Unspecified;
                break;
        }
    }

    private static void ApplyThemeResources(AppTheme theme)
    {
        var rd = Current!.Resources;

        if (theme == AppTheme.Dark)
        {
            rd["DisplayBackgroundColor"] = Color.FromArgb("#1E1E1E");
            rd["OperatorButtonColor"] = Color.FromArgb("#FF9800");
            rd["SecondaryTextColor"] = Color.FromArgb("#B0B0B0");
        }
        else
        {
            rd["DisplayBackgroundColor"] = Color.FromArgb("#F5F5F5");
            rd["OperatorButtonColor"] = Color.FromArgb("#FFC864");
            rd["SecondaryTextColor"] = Color.FromArgb("#757575");
        }
    }
}
