# Building the Construction Calculator Installer

This guide explains how to create a Windows installer for the Construction Calculator using Inno Setup.

## Prerequisites

1. **Inno Setup Compiler** - Download and install from: https://jrsoftware.org/isdl.php
   - Get the latest version (currently Inno Setup 6.x)
   - The installer is small (~3MB) and installs quickly

2. **.NET 8.0 SDK** - Already required for building the application

## Step 1: Publish the Application

Before creating the installer, you need to publish the application in Release mode:

```bash
cd ConstructionCalculator
dotnet add package MaterialSkin.2 --version 2.3.1
dotnet publish -c Release -r win-x64 --self-contained false
```

This creates the published application in: `ConstructionCalculator\bin\Release\net8.0-windows\win-x64\publish\`

The `--self-contained false` flag creates a framework-dependent deployment, which means users will need .NET 8.0 Runtime installed on their Windows 11 machine.

## Step 2: Compile the Installer

1. Open the `ConstructionCalculator-Setup.iss` file in **Inno Setup Compiler**
   - Right-click the .iss file → Open With → Inno Setup Compiler
   - Or launch Inno Setup and use File → Open to open the .iss file

2. Click **Build → Compile** (or press Ctrl+F9)
   - The compiler will process the script and create the installer
   - Progress will be shown in the output window
   - Make sure you published the application first (Step 1) or the build will fail

3. The installer will be created in the `Installer` folder as:
   - `ConstructionCalculator-Setup.exe`

## Step 3: Test the Installer

1. Double-click `ConstructionCalculator-Setup.exe` to run the installer
2. Follow the installation wizard
3. The application will be installed to `C:\Program Files\Construction Calculator\` by default
4. A Start Menu shortcut will be created
5. Optionally, a Desktop shortcut can be created during installation

## Customizing the Installer

You can edit the `ConstructionCalculator-Setup.iss` file to customize:

- **App Version**: Change `#define MyAppVersion "1.0.0"` at the top
- **Publisher Name**: Change `#define MyAppPublisher "Lee Cassin"`
- **Default Install Directory**: Modify `DefaultDirName={autopf}\{#MyAppName}`
- **Desktop Icon**: Change `Flags: unchecked` to `Flags: checked` in the Tasks section to enable desktop icon by default

## Distributing the Installer

Once compiled, you can distribute the `ConstructionCalculator-Setup.exe` file to users. It includes:

- The application executable
- MaterialSkin.2 library
- All application dependencies

**Important:** Users will need to have **.NET 8.0 Runtime** installed on their Windows 11 machine. They can download it from: https://dotnet.microsoft.com/download/dotnet/8.0

Users just need to:
1. Install .NET 8.0 Runtime (if not already installed)
2. Download the installer
3. Run it
4. Follow the installation wizard
5. Launch the application from Start Menu or Desktop

## File Size

The installer will be approximately 5-10 MB since it's a framework-dependent deployment (doesn't include the .NET runtime). Users need to install the .NET 8.0 Runtime separately, but this keeps the installer small and allows multiple .NET applications to share the same runtime.

## Notes

- The installer requires administrator privileges to install to Program Files
- Users can change the installation directory during installation
- The uninstaller is automatically created and added to Windows "Add or Remove Programs"
- All files are properly uninstalled when the user uninstalls the application
