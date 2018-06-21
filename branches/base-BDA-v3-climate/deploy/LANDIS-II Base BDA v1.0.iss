#define PackageName      "Base BDA"
#define PackageNameLong  "Base BDA Extension"
#define Version          "1.0"
#define ReleaseType      "official"
;#define ReleaseNumber    "4"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include "..\package-setup.iss"


[Files]

; Base BDA (v1.0)
Source: {#CoreBinDir}\Landis.BaseBDA.dll; DestDir: {app}\bin
Source: doc\*; DestDir: {app}\doc
Source: examples\*; DestDir: {app}\examples

#define BaseBDA "Base BDA 1.0.txt"
Source: {#BaseBDA}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add entries for each plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseBDA}"" "; WorkingDir: {#LandisPlugInDir}


[Code]

{ Check for other prerequisites during the setup initialization }

function CheckOtherPrerequisites(): Boolean;
begin
  Result := True
end;


#include "..\package-code.iss"
