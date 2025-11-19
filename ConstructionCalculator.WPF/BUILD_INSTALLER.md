# Building the Construction Calculator Installer

This guide explains how to build the Windows installer for Construction Calculator (WPF Version).

## Prerequisites

1. **Inno Setup 6.x or later**
   - Download from: https://jrsoftware.org/isdl.php
   - Install with default options

2. **.NET 10 SDK**
   - Already installed (required for building the application)

## Build Steps

### 1. Publish the Application

First, create a self-contained release build:

```bash
cd ConstructionCalculator.WPF
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true -p:PublishTrimmed=false -p:DebugType=None -o publish/win-x64
```

This creates a single executable with all dependencies in `ConstructionCalculator.WPF/publish/win-x64/`.

**Publish options explained:**
- `--self-contained true`: Includes .NET runtime (no separate installation needed)
- `-p:PublishSingleFile=true`: Creates a single executable file
- `-p:IncludeNativeLibrariesForSelfExtract=true`: Bundles native libraries
- `-p:PublishReadyToRun=true`: Improves startup performance
- `-p:PublishTrimmed=false`: Keeps all assemblies (required for WPF)
- `-p:DebugType=None`: Removes debug symbols for smaller size

### 2. Build the Installer

Open `Setup.iss` in Inno Setup Compiler and click "Compile", or run from command line:

```bash
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Setup.iss
```

The installer will be created in `ConstructionCalculator.WPF/installer/` as:
- `ConstructionCalculator-Setup-2.0.0.exe`

### 3. Test the Installer

**Important:** Test on a clean Windows machine or VM to ensure:
- Installation completes successfully
- Application launches correctly
- All features work (calculators, themes, etc.)
- Desktop shortcut works (if selected)
- Start Menu shortcuts work
- Uninstallation removes all files

## Installer Features

- **Self-contained**: No .NET runtime installation required
- **Single file**: 148MB executable with all dependencies
- **Start Menu integration**: Shortcuts in Start Menu
- **Optional desktop shortcut**: User can choose during installation
- **Clean uninstall**: Removes all application files
- **Modern UI**: Uses Inno Setup's modern wizard style
- **64-bit only**: Optimized for x64 Windows systems

## Customization

### Updating Version Number

Edit `Setup.iss` line 6:
```iss
#define MyAppVersion "2.0.0"
```

### Changing Publisher Name

Edit `Setup.iss` line 7:
```iss
#define MyAppPublisher "Your Company Name"
```

### Adding Code Signing

If you have a code signing certificate:

1. Uncomment line 30 in `Setup.iss`:
```iss
SignTool=signtool
```

2. Configure signtool in Inno Setup:
   - Tools → Configure Sign Tools
   - Add: `signtool=path\to\signtool.exe sign /f "path\to\cert.pfx" /p "password" /t http://timestamp.digicert.com $f`

### Changing Install Location

Default is `C:\Program Files\Construction Calculator`. To change, edit line 21:
```iss
DefaultDirName={autopf}\YourFolderName
```

## Troubleshooting

### Installer build fails with "File not found"

**Solution:** Ensure you've run the publish command first. The installer expects files in `publish/win-x64/`.

### Application doesn't launch after install

**Solution:** 
1. Check Windows Event Viewer for errors
2. Verify all files were copied to installation directory
3. Try running as administrator
4. Check antivirus isn't blocking the executable

### Installer is too large

**Solution:** The self-contained deployment includes the entire .NET runtime (~140MB). This is normal. For smaller installers, consider framework-dependent deployment (requires users to install .NET Desktop Runtime separately).

### Desktop shortcut not created

**Solution:** This is by design - the desktop shortcut is optional and unchecked by default. Users must check the box during installation.

## Distribution

Once built and tested, distribute the installer:
- Upload to GitHub Releases
- Host on your website
- Share via email or cloud storage

**Note:** Windows SmartScreen may show a warning for unsigned installers. Code signing eliminates this warning but requires a certificate (~$100-300/year).

## Support

For issues or questions:
- GitHub: https://github.com/lcassin/Construction-Calculator
- Report bugs via GitHub Issues
