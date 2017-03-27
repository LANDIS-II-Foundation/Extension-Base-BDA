#define PackageName      "Budworm BDA"
#define PackageNameLong  "Budworm BDA Extension"
#define Version          "1.0"
#define ReleaseType      "beta"
#define ReleaseNumber    "4"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif


[Files]

Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BudwormBDA.dll; DestDir: {app}\bin; Flags: replacesameversion

; Budworm BDA
Source: docs\LANDIS-II Budworm BDA v1.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\budworm-BDA

#define BudwormBDA "Budworm BDA 1.0.txt"
Source: {#BudwormBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Budworm BDA"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BudwormBDA}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"

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
