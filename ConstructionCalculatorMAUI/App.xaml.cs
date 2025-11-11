namespace ConstructionCalculatorMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        LoadThemePreference();
        ApplyThemeResources(RequestedTheme);

        RequestedThemeChanged += (s, e) => ApplyThemeResources(e.RequestedTheme);

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
