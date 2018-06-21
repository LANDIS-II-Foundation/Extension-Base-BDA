#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "1.1"
#define ReleaseType      "official"
;#define ReleaseNumber    "4"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include "..\package-setup.iss"


[Files]

; Base BDA
Source: {#CoreBinDir}\Landis.BaseBDA.dll; DestDir: {app}\bin
Source: doc\*; DestDir: {app}\doc
Source: examples\*; DestDir: {app}\examples

#define BaseBDA "Base BDA 1.1.txt"
Source: {#BaseBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseBDA}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }

function CheckOtherPrerequisites(): Boolean;
  	var appId: String;
  	    uninstallKey: String;
  	    uninstaller: String;
  	    resultCode: Integer;
begin
  { See if there's uninstaller for Base BDA 1.0; if so, run it. }
  appId := 'LANDIS-II Base BDA v1.0';
  uninstallKey := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\' + appId + '_is1';
  uninstaller := '';
  if not RegQueryStringValue(HKLM, uninstallKey, 'UninstallString', uninstaller) then begin
    RegQueryStringValue(HKCU, uninstallKey, 'UninstallString', uninstaller);
  end;

  if uninstaller <> '' then begin
    uninstaller := RemoveQuotes(uninstaller);
    { msgBox('uninstaller = "' + uninstaller + '"', mbInformation, MB_OK); }
    Exec(uninstaller, '/VERYSILENT', ExtractFilePath(uninstaller),
         SW_HIDE, ewWaitUntilTerminated, resultCode);

    { remove Base BDA from plug-ins database }
    Exec('{#PlugInAdminTool}', 'remove "Base BDA"', ExtractFilePath('{#PlugInAdminTool}'),
         SW_HIDE, ewWaitUntilTerminated, resultCode);
  end;

  Result := True
end;


#include "..\package-code.iss"
