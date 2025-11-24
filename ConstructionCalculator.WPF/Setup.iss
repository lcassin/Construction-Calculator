; Inno Setup Script for Construction Calculator (WPF Version)
; This script creates a Windows installer for the Construction Calculator WPF application
; Uses self-contained .NET 10 publish (includes runtime, no separate installation needed)

#define MyAppName "Construction Calculator"
#define MyAppVersion "2.0.6"
#define MyAppPublisher "Lee Cassin"
#define MyAppURL "https://github.com/lcassin/Construction-Calculator"
#define MyAppExeName "ConstructionCalculator.WPF.exe"
#define PublishDir "bin\release\net10.0-windows\publish"

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
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
SetupIconFile=Assets\app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
; Upgrade behavior settings
UsePreviousAppDir=yes
UsePreviousGroup=yes
UsePreviousTasks=yes
CloseApplications=yes
RestartApplications=no
LicenseFile=..\LICENSE
; Uncomment the following line if you have a code signing certificate
; SignTool=signtool

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; Install the self-contained executable and all dependencies
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Install the high-resolution icon file for shortcuts to reference directly
Source: "Assets\app.ico"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: The publish output includes all .NET runtime dependencies, so no separate runtime installation is needed

[Icons]
; Start Menu shortcut (uses explicit icon file for crisp display)
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\app.ico"; WorkingDir: "{app}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
; Desktop shortcut (optional, based on user selection, uses explicit icon file)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\app.ico"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
; Option to launch the application after installation
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Clean up any user-generated files (optional)
Type: filesandordirs; Name: "{app}"

[Code]
// Migration code to handle transition from per-machine to per-user installs
// This detects and uninstalls any old per-machine (admin) installations

function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{#SetupSetting("AppId")}_is1';
  sUnInstallString := '';
  
  // Check HKLM (per-machine installs) for old admin-installed versions
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    sUnInstallString := '';
  
  Result := sUnInstallString;
end;

function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;

function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
  Result := 0;
  sUnInstallString := GetUninstallString();
  
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    
    // Show message to user about removing old version
    if MsgBox('A previous per-machine installation of {#MyAppName} was detected. ' +
              'It will be uninstalled before installing the new per-user version. ' +
              'You may see a UAC prompt to remove the old version.' + #13#10#13#10 +
              'Continue with uninstall?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      // Run the uninstaller silently
      if Exec(sUnInstallString, '/VERYSILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
        Result := 1
      else
        Result := 2; // Uninstall failed
    end else
      Result := 3; // User cancelled
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  UninstallResult: Integer;
begin
  Result := '';
  
  // Check if we need to uninstall an old per-machine version
  if IsUpgrade() then
  begin
    UninstallResult := UnInstallOldVersion();
    
    case UninstallResult of
      0: ; // No old version found, continue normally
      1: ; // Successfully uninstalled, continue
      2: Result := 'Failed to uninstall the previous version. Please uninstall it manually from Windows Settings > Apps before continuing.';
      3: Result := 'Installation cancelled by user.';
    end;
  end;
end;
