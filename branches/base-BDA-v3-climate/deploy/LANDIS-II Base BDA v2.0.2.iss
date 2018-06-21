#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "2.0.2"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif


[Files]

Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BaseBDA.dll; DestDir: {app}\bin; Flags: replacesameversion

; Base BDA
Source: docs\LANDIS-II Biological Disturbance Agent v2.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\base-BDA

#define BaseBDA "Base BDA 2.0.2.txt"
Source: {#BaseBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base BDA"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseBDA}"" "; WorkingDir: {#LandisPlugInDir}


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
