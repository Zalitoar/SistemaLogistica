#define MyAppName "Sistema Logistica"
#define MyAppExeName "IngSoft.exe"

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

#ifndef SourceDir
  #define SourceDir "..\IngSoft\bin\Release"
#endif

#ifndef OutputDir
  #define OutputDir "..\artifacts\installer"
#endif

[Setup]
AppId={{64D46A80-2552-4A5F-A732-7D9209D7C82C}
AppName={#MyAppName}
AppVersion={#AppVersion}
AppVerName={#MyAppName} {#AppVersion}
DefaultDirName={autopf}\SistemaLogistica
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir={#OutputDir}
OutputBaseFilename=SistemaLogistica-Setup-{#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x86compatible x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
CloseApplications=yes
RestartApplications=no
SetupLogging=yes
MinVersion=6.1sp1

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Accesos directos:"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

#ifdef IncludeDotNet472
Source: "{#DotNetInstaller}"; DestDir: "{tmp}"; DestName: "dotnet472-offline.exe"; Flags: deleteafterinstall; Check: NeedsDotNet472
#endif

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: desktopicon

#ifdef IncludeDotNet472
[Run]
Filename: "{tmp}\dotnet472-offline.exe"; Parameters: "/q /norestart"; StatusMsg: "Instalando Microsoft .NET Framework 4.7.2..."; Flags: waituntilterminated runhidden; Check: NeedsDotNet472
Filename: "{app}\{#MyAppExeName}"; Description: "Ejecutar {#MyAppName}"; Flags: nowait postinstall skipifsilent; Check: IsDotNet472OrNewer
#else
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Ejecutar {#MyAppName}"; Flags: nowait postinstall skipifsilent
#endif

[Code]
const
  DotNet472Release = 461808;

function IsDotNet472OrNewer(): Boolean;
var
  Release: Cardinal;
begin
  Result := False;

  if IsWin64 then
  begin
    if RegQueryDWordValue(
      HKLM64,
      'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
      'Release',
      Release) then
      Result := Release >= DotNet472Release;
  end
  else
  begin
    if RegQueryDWordValue(
      HKLM32,
      'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
      'Release',
      Release) then
      Result := Release >= DotNet472Release;
  end;
end;

function NeedsDotNet472(): Boolean;
begin
  Result := not IsDotNet472OrNewer();
end;

#ifndef IncludeDotNet472
function InitializeSetup(): Boolean;
begin
  Result := True;

  if not IsDotNet472OrNewer() then
  begin
    MsgBox(
      'Sistema Logistica requiere Microsoft .NET Framework 4.7.2 o una versión 4.x posterior.' + #13#10 + #13#10 +
      'Instale .NET Framework y vuelva a ejecutar este instalador.',
      mbError,
      MB_OK);
    Result := False;
  end;
end;
#endif
