Outfile "OofDetector_installer.exe"
BrandingText "Ur mom gay since 1945"
RequestExecutionLevel highest

InstallDir "C:\\Program Files (x86)\CorpInsanity\OofDetector"
 
Section
 
SetOutPath "$INSTDIR"

ExecWait 'taskkill.exe -f -im oofdetector.exe'

File OofDetector.exe
File OofDetector.exe.config
File OofDetector.pdb
File Newtonsoft.Json.xml
File Newtonsoft.Json.dll
File favicon.ico
File runOnStartup.ps1
File /r media

CreateShortCut "$SMPROGRAMS\OofDetector.lnk" "$INSTDIR\OofDetector.exe"

ExecWait 'powershell.exe -ExecutionPolicy Bypass -File "$INSTDIR\runOnStartup.ps1"'
Exec '$INSTDIR\OofDetector.exe'

WriteUninstaller $INSTDIR\uninstaller.exe
 
SectionEnd

Section "Uninstall"

ExecWait 'taskkill.exe -f -im oofdetector.exe'
Delete $INSTDIR\uninstaller.exe
RMDir /R $INSTDIR
Delete "$SMPROGRAMS\OofDetector.lnk"
 
SectionEnd