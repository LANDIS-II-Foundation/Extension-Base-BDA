#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "1.2.3"
#define ReleaseType      "official"
#define ReleaseNumber    "1"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif


[Files]

Source: {#LandisBuildDir}\base bda\build\{#Configuration}\Landis.BaseBDA.dll; DestDir: {app}\bin
#if ReleaseType != "official"
  ; Source: {#LandisBuildDir}\base bda\build\{#Configuration}\Landis.BaseBDA.pdb; DestDir: {app}\bin
#endif

; Base BDA
Source: docs\*; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples

#define BaseBDA "Base BDA 1.2.txt"
Source: {#BaseBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseBDA}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }

#include AddBackslash(LandisDeployDir) + "package (Code section).iss"

//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
  // Alpha and beta releases of version 1.0 don't remove the plug-in name from
  // database
  if StartsWith(currentVersion.Version, '1.0') or
     StartsWith(currentVersion.Version, '1.1') then
    begin
      Exec('{#PlugInAdminTool}', 'remove "Base BDA"',
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
