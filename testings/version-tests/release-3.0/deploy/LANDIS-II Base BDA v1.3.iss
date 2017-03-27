#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "1.3"
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

Source: {#LandisBuildDir}\disturbanceextensions\base bda\src\basebda\obj\release\Landis.Extension.BaseBDA.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: {#LandisBuildDir}\disturbanceextensions\base bda\src\Troschuetz.Random.dll; DestDir: {app}\bin

; Base BDA
Source: docs\LANDIS-II Base BDA v1.3 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples

#define BaseBDA "Base BDA 1.3.txt"
Source: {#BaseBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base BDA"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseBDA}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }

#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
end;


//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
