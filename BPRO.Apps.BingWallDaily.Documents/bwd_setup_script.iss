; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "BingWall Daily"
#define MyAppVersion "1"
#define MyAppPublisher "CodeTampus"
#define MyAppURL "http://www.codetampus.com/"
#define MyAppExeName "BingWallDaily.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7BA39039-7720-47C5-A380-91E313A0F232}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=bwdbeta_setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\BingWallDaily.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BPRO.Apps.BingWallDaily.Core\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
