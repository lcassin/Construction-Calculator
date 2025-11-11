# Construction Calculator - MAUI Version

This is a cross-platform version of the Construction Calculator built with .NET MAUI, supporting Windows, macOS, iOS, and Android.

## What is MAUI?

.NET MAUI (Multi-platform App UI) is Microsoft's framework for building native cross-platform apps with C# and XAML. It's the evolution of Xamarin.Forms and allows you to write your app once and run it on multiple platforms.

### Key Concepts for C# Developers New to MAUI:

1. **XAML for UI**: Instead of Windows Forms designer, MAUI uses XAML (XML-based markup) to define UI layouts
2. **ContentPage**: The basic page type (similar to a Form in Windows Forms)
3. **Shell Navigation**: Built-in navigation system with flyout menus (replaces your Tools menu)
4. **Data Binding**: Connect UI elements to code properties automatically
5. **Theming**: Built-in Light/Dark theme support with `AppThemeBinding`

## Project Structure

```
ConstructionCalculatorMAUI/
├── App.xaml/App.xaml.cs          # Application entry point, theme management
├── AppShell.xaml/AppShell.xaml.cs # Navigation structure (flyout menu)
├── MauiProgram.cs                 # App configuration
├── Pages/                         # All calculator pages
│   ├── MainPage.xaml/.cs         # Main calculator
│   ├── AngleCalculatorPage.xaml/.cs
│   ├── StairCalculatorPage.xaml/.cs
│   ├── SurveyCalculatorPage.xaml/.cs
│   ├── SeatingLayoutCalculatorPage.xaml/.cs
│   ├── AreaCalculatorPage.xaml/.cs
│   └── AboutPage.xaml/.cs
└── Resources/                     # Images, fonts, styles

ConstructionCalculator.Core/
└── Measurement.cs                 # Shared measurement logic (documented)
```

## Prerequisites

### Windows
- Visual Studio 2022 (17.8 or later)
- .NET 8.0 SDK
- MAUI workload installed

### macOS
- Visual Studio 2022 for Mac or VS Code
- .NET 8.0 SDK
- MAUI workload installed
- Xcode (for iOS development)

## Installing MAUI Workload

Open a terminal/command prompt and run:

```bash
dotnet workload install maui
```

## Building and Running

### Option 1: Visual Studio (Recommended for Windows)

1. Open `Construction-Calculator.sln` in Visual Studio 2022
2. Set `ConstructionCalculatorMAUI` as the startup project
3. Select your target platform from the dropdown (Windows, Android, iOS, etc.)
4. Press F5 to build and run

### Option 2: Command Line

```bash
# Navigate to the MAUI project directory
cd ConstructionCalculatorMAUI

# Build for Windows
dotnet build -f net8.0-windows10.0.19041.0

# Run on Windows
dotnet run -f net8.0-windows10.0.19041.0

# Build for Android (requires Android SDK)
dotnet build -f net8.0-android

# Build for iOS (requires macOS and Xcode)
dotnet build -f net8.0-ios
```

## Features

### Main Calculator
- Feet-inches-fractions arithmetic (e.g., "12' 3-1/2\"")
- Decimal mode toggle
- Calculation history chain
- Copy to clipboard
- Keyboard shortcuts

### Specialized Calculators
1. **Angle Calculator** - Convert between degrees, radians, and gradians
2. **Stair Calculator** - Calculate rise, run, and tread dimensions
3. **Survey Calculator** - Bearing/azimuth conversions and coordinate calculations
4. **Seating Layout Calculator** - Calculate seating arrangements
5. **Area Calculator** - Multi-section square footage calculations

### Theming
- Light mode
- Dark mode
- System default (follows OS theme)
- Theme preference is saved automatically

## Differences from Windows Forms Version

### UI Layout
- **Responsive**: Adapts to different screen sizes and orientations
- **Touch-friendly**: Larger buttons and touch targets for mobile
- **Native controls**: Uses platform-specific controls for better performance

### Navigation
- **Shell Flyout**: Replaces the Tools menu with a slide-out navigation drawer
- **Deep linking**: Each calculator has its own route for navigation

### Input
- **Entry control**: Replaces TextBox, optimized for mobile keyboards
- **Keyboard handling**: Different keyboard types per platform (numeric, text, etc.)

### Theming
- **AppThemeBinding**: Automatically switches colors based on Light/Dark mode
- **Resource Dictionary**: Centralized styling in App.xaml

## Code Architecture

### Core Library (`ConstructionCalculator.Core`)
Contains platform-independent business logic:
- `Measurement` class: Parsing, formatting, and arithmetic operations
- Fully documented with XML comments
- Shared between MAUI app and original Windows Forms app

### MAUI App (`ConstructionCalculatorMAUI`)
Platform-specific UI and navigation:
- XAML pages for each calculator
- Code-behind for event handling
- Shell navigation structure

## Development Tips

### Debugging
- Use breakpoints in code-behind (.cs files) just like Windows Forms
- XAML Hot Reload: Edit XAML while app is running to see changes instantly
- Use `Debug.WriteLine()` for logging (appears in Output window)

### XAML Basics
```xml
<!-- Layout containers -->
<VerticalStackLayout>  <!-- Stack items vertically -->
<HorizontalStackLayout>  <!-- Stack items horizontally -->
<Grid>  <!-- Grid layout with rows/columns -->

<!-- Common controls -->
<Label Text="Hello" />  <!-- Text display -->
<Entry Text="{Binding Value}" />  <!-- Text input -->
<Button Text="Click" Clicked="OnButtonClicked" />  <!-- Button -->
```

### Data Binding
Instead of manually updating UI:
```csharp
// Windows Forms way
displayTextBox.Text = "0\"";

// MAUI way (with binding)
public string DisplayText { get; set; } = "0\"";
// In XAML: <Entry Text="{Binding DisplayText}" />
```

## Troubleshooting

### Build Errors
- **"MAUI workload not found"**: Run `dotnet workload install maui`
- **"Android SDK not found"**: Install Android SDK through Visual Studio Installer
- **"iOS build failed"**: Requires macOS with Xcode installed

### Runtime Issues
- **App crashes on startup**: Check MauiProgram.cs configuration
- **Navigation not working**: Verify routes in AppShell.xaml
- **Theme not applying**: Check App.xaml resource dictionary

## Migration Notes

This MAUI version maintains the same core calculation logic as the Windows Forms version but with a modernized, cross-platform UI. The `Measurement` class has been extracted into a shared Core library with full documentation, making it reusable across both versions.

### What's the Same
- All calculation logic and measurement parsing
- Feet-inches-fractions format support
- All specialized calculators
- Theme support (Light/Dark/System)

### What's Different
- XAML-based UI instead of Windows Forms designer
- Shell navigation instead of menu bar
- Responsive layouts for mobile devices
- Platform-specific behaviors (keyboard, clipboard, etc.)

## Next Steps

1. **Test on Windows**: Build and run to verify all calculators work
2. **Test on Android**: Deploy to Android emulator or device
3. **Test on iOS/Mac**: Requires macOS for building
4. **Customize**: Modify colors, fonts, and layouts in App.xaml
5. **Extend**: Add new calculators by creating new Pages

## Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [XAML Basics](https://docs.microsoft.com/dotnet/maui/xaml/)
- [Shell Navigation](https://docs.microsoft.com/dotnet/maui/fundamentals/shell/)
- [Data Binding](https://docs.microsoft.com/dotnet/maui/fundamentals/data-binding/)

## Support

For issues or questions about this MAUI migration, refer to the original Windows Forms version in the `ConstructionCalculator` directory for comparison.
