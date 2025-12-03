namespace ConstructionCalculatorMAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Track navigation to pass previous route to Main Calculator
        Navigating += OnShellNavigating;
    }

    private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        // DEBUG: Log navigation details
        System.Diagnostics.Debug.WriteLine($"[AppShell Navigating] Target={e.Target.Location.OriginalString}, Current={Shell.Current.CurrentState?.Location?.OriginalString}");
        
        // If navigating to Main Calculator from another calculator, add the previous route
        // Guard against infinite loops: only intercept if target doesn't already have "from=" parameter
        if (e.Target.Location.OriginalString.Contains("MainPage") && 
            !e.Target.Location.OriginalString.Contains("from="))
        {
            var currentRoute = Shell.Current.CurrentState?.Location?.OriginalString;
            
            // Only add "from" parameter if we're coming from a calculator (not from hub or settings)
            if (!string.IsNullOrEmpty(currentRoute) && 
                !currentRoute.Contains("MainPage") && 
                !currentRoute.Contains("CalculatorHub") &&
                !currentRoute.Contains("Settings") &&
                !currentRoute.Contains("About"))
            {
                // Extract the route name from the current location
                var routeName = ExtractRouteName(currentRoute);
                
                if (!string.IsNullOrEmpty(routeName))
                {
                    // DEBUG: Log interception
                    System.Diagnostics.Debug.WriteLine($"[AppShell Navigating] Intercepting to MainPage: currentRoute={currentRoute}, routeName={routeName}");
                    
                    // Cancel this navigation and navigate with the query parameter
                    e.Cancel();
                    
                    // Navigate to Main Calculator with the previous route
                    Shell.Current.GoToAsync($"//MainPage?from={routeName}");
                }
            }
        }
    }

    private string? ExtractRouteName(string location)
    {
        // Extract route name from location string (e.g., "//StairCalculator" -> "StairCalculator")
        if (location.Contains("//"))
        {
            var parts = location.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                // Get the route name (remove any query parameters)
                var route = parts[0].Split('?')[0];
                return route;
            }
        }
        return null;
    }
}
