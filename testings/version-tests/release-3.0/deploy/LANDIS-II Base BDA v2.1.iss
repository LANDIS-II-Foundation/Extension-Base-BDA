#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "2.1"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include "J:\Scheller\LANDIS-II\deploy\package (Setup section) v6.0.iss"
#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"

[Files]

Source: ..\src\bin\debug\Landis.Extension.BaseBDA.dll; DestDir: {#ExtDir}; Flags: replacesameversion
Source: ..\src\bin\debug\Landis.Library.Metadata.dll; DestDir: {#ExtDir}; Flags: replacesameversion uninsneveruninstall

; Base BDA
Source: docs\LANDIS-II Biological Disturbance Agent v2.1 User Guide.pdf; DestDir: {#AppDir}\docs
Source: examples\*.txt; DestDir: {#AppDir}\examples\base-BDA
Source: examples\ecoregions.gis; DestDir: {#AppDir}\examples\base-BDA
Source: examples\initial-communities.gis; DestDir: {#AppDir}\examples\base-BDA
Source: examples\*.bat; DestDir: {#AppDir}\examples\base-BDA

#define BaseBDA "Base BDA 2.1.txt"
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

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
end;


//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
