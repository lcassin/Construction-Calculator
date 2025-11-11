namespace ConstructionCalculatorMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        LoadThemePreference();

        MainPage = new AppShell();
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
}
