namespace ConstructionCalculatorMAUI.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentTheme();
    }

    private void LoadCurrentTheme()
    {
        string savedTheme = Preferences.Get("AppTheme", "System");
        
        switch (savedTheme)
        {
            case "Light":
                LightThemeRadio.IsChecked = true;
                break;
            case "Dark":
                DarkThemeRadio.IsChecked = true;
                break;
            default:
                SystemThemeRadio.IsChecked = true;
                break;
        }
    }

    private void OnThemeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;

        RadioButton radioButton = (RadioButton)sender;
        string themeName = radioButton.Content.ToString() ?? "System Default";

        AppTheme newTheme;
        string themePreference;

        if (themeName == "Light")
        {
            newTheme = AppTheme.Light;
            themePreference = "Light";
        }
        else if (themeName == "Dark")
        {
            newTheme = AppTheme.Dark;
            themePreference = "Dark";
        }
        else
        {
            newTheme = AppTheme.Unspecified;
            themePreference = "System";
        }

        Application.Current.UserAppTheme = newTheme;
        Preferences.Set("AppTheme", themePreference);
    }
}
