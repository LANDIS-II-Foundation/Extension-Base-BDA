#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "3.0"
#define ReleaseType      "official"
#define ReleaseNumber    "3"
#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"
#define LandisPlugInDir "C:\Program Files\LANDIS-II\plug-ins"

#include "package (Setup section) v6.0.iss"

[Files]
; This .dll IS the extension (ie, the extension's assembly)
; NB: Do not put a version number in the file name of this .dll
Source: ..\..\src\bin\debug\Landis.Extension.BaseBDA.dll; DestDir: {#ExtDir}; Flags: replacesameversion


; Requisite auxiliary libraries
; NB. These libraries are used by other extensions and thus are never uninstalled.
Source: ..\..\src\bin\debug\Landis.Library.Metadata.dll; DestDir: {#ExtDir}; Flags: replacesameversion uninsneveruninstall
Source: ..\..\src\bin\debug\Landis.Library.Climate.dll; DestDir: {#ExtDir}; Flags: replacesameversion uninsneveruninstall


; Complete example for testing the extension
Source: ..\examples\*.txt; DestDir: {#AppDir}\examples\Base BDA
Source: ..\examples\*.gis; DestDir: {#AppDir}\examples\Base BDA
Source: ..\examples\*.bat; DestDir: {#AppDir}\examples\Base BDA


; User Guides are no longer shipped with installer
;Source: docs\LANDIS-II Biological Disturbance Agent v3.0 User Guide.pdf; DestDir: {#AppDir}\docs



; LANDIS-II identifies the extension with the info in this .txt file
; NB. New releases must modify the name of this file and the info in it
#define InfoTxt "Base BDA 3.0.1.txt"
Source: {#InfoTxt}; DestDir: {#LandisPlugInDir}


[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base BDA"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#InfoTxt}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }
#include "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
