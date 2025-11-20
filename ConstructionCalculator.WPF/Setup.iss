; Inno Setup Script for Construction Calculator (WPF Version)
; This script creates a Windows installer for the Construction Calculator WPF application
; Requires .NET 10 self-contained publish output

#define MyAppName "Construction Calculator"
#define MyAppVersion "2.0.1"
#define MyAppPublisher "Lee Cassin"
#define MyAppURL "https://github.com/lcassin/Construction-Calculator"
#define MyAppExeName "ConstructionCalculator.WPF.exe"
#define PublishDir "publish\win-x64"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{8B3F4A1C-9D2E-4F8A-A7B3-1C5D6E7F8G9H}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=installer
OutputBaseFilename=ConstructionCalculator-Setup-{#MyAppVersion}
Compression=lzma2/ultra
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
SetupIconFile=Assets\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
; LicenseFile=..\ConstructionCalculator\LICENSE
; Uncomment the above line if you want to include a license agreement during installation
; Uncomment the following line if you have a code signing certificate
; SignTool=signtool

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; Install the self-contained executable and all dependencies
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: The publish output includes all .NET runtime dependencies, so no separate runtime installation is needed

[Icons]
; Start Menu shortcut
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
; Desktop shortcut (optional, based on user selection)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
; Option to launch the application after installation
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Clean up any user-generated files (optional)
Type: filesandordirs; Name: "{app}"

[Code]
// Custom code section for advanced installation logic (if needed in the future)
