#define PackageName      "Base Fire"
#define PackageNameLong  "Base Fire Extension"
#define Version          "2.1"
#define ReleaseType      "official"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif


[Files]

Source: {#LandisBuildDir}\disturbanceextensions\base fire\build\release\Landis.Extension.BaseFire.dll; DestDir: {app}\bin

; Base Fire
Source: docs\*; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples

#define BaseFire "Base Fire 2.1.txt"
Source: {#BaseFire}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseFire}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }

#include AddBackslash(LandisDeployDir) + "package (Code section) v2.iss"

//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
  // Alpha and beta releases of version 1.0 don't remove the plug-in name from
  // database
  if StartsWith(currentVersion.Version, '1.0') or
     StartsWith(currentVersion.Version, '1.1') or
     StartsWith(currentVersion.Version, '1.2') then
    begin
      Exec('{#PlugInAdminTool}', 'remove "Base Fire"',
           ExtractFilePath('{#PlugInAdminTool}'),
		   SW_HIDE, ewWaitUntilTerminated, Result);
	end
  else
    Result := 0;
end;

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
