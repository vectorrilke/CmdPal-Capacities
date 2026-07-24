; TEMPLATE: Inno Setup Configuration for Command Palette Extensions
; This template is used by build-exe.ps1 to create platform-specific installers
; 
; Customize these values:
; - AppVersion: Extracted from project version (set by build-exe.ps1)
; - AppName, AppPublisher: Your extension info
; - AppGUID: Must match the CLSID in your extension class
; - DefaultDirName: Installation target folder

#define AppName "Capacities Command Palette Extension"
#define AppVersion "0.1.0.0"
#define AppPublisher "Vector Rilke"
#define AppURL "https://github.com/vectorrilke/CmdPal-Capacities"
#define AppGUID "f7e5cd38-f4cf-4280-930a-6a352b70672e"
#define ProjectName "CapacitiesCommandPaletteExtension"
#define BuildFolder "bin\Release\win-x64\publish"

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
CreateAppDir=no
OutputDir=bin\Release\installer
OutputBaseFilename={#ProjectName}-{#AppVersion}
DefaultDirName={userappdata}\Microsoft\Command Palette Extensions\{#ProjectName}
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
ShowLanguageDialog=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Files are sourced from the published build output directory
Source: "{#BuildFolder}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Create Start Menu shortcuts if desired
Name: "{userappdata}\Microsoft\Windows\Start Menu\Programs\{#AppName}"; Filename: "{app}\{#ProjectName}.exe"; Parameters: "-"; WorkingDir: "{app}"

[Run]
; Optional: Run extension after install
Filename: "{app}\{#ProjectName}.exe"; Description: "Launch {#AppName}"; Flags: postinstall nowait

[UninstallDelete]
Type: dirifempty; Name: "{userappdata}\Microsoft\Command Palette Extensions\{#ProjectName}"

[Registry]
; Register the extension COM object
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Explorer\CommandPalette\Extensions\{#AppGUID}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Explorer\CommandPalette\Extensions\{#AppGUID}"; ValueType: string; ValueName: "Path"; ValueData: "{app}\{#ProjectName}.dll"
