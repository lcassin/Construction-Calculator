using System.Configuration;
using System.Data;
using System.Windows;

namespace ConstructionCalculator.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        string themePreference = ConstructionCalculator.WPF.Properties.Settings.Default.ThemePreference;
        ApplyTheme(themePreference);
    }
    
    public static void ApplyTheme(string theme)
    {
        ConstructionCalculator.WPF.Properties.Settings.Default.ThemePreference = theme;
        
        switch (theme)
        {
            case "Light":
                Application.Current.ThemeMode = ThemeMode.Light;
                break;
            case "Dark":
                Application.Current.ThemeMode = ThemeMode.Dark;
                break;
            case "System":
            default:
                Application.Current.ThemeMode = ThemeMode.System;
                break;
        }
    }
}

