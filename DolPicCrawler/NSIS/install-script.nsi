; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "돌픽 크롤러"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "mocadev"
!define PRODUCT_WEB_SITE "http://www.mocadev.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\DolPicCrawler.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "license.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\DolPicCrawler.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "Korean"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "dolpicSetup.exe"
InstallDir "$PROGRAMFILES\돌픽 크롤러"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite try
  File "Release\DolPicCrawler.exe"
  CreateDirectory "$SMPROGRAMS\돌픽 크롤러"
  CreateShortCut "$SMPROGRAMS\돌픽 크롤러\돌픽 크롤러.lnk" "$INSTDIR\DolPicCrawler.exe"
  CreateShortCut "$DESKTOP\돌픽 크롤러.lnk" "$INSTDIR\DolPicCrawler.exe"
  File "Release\DolPicCrawler.exe.config"
  File "Release\DolPicCrawler.exe.Debug.config"
  File "Release\DolPicCrawler.exe.deploy-test.config"
  File "Release\DolPicCrawler.exe.local-test.config"
  File "Release\DolPicCrawler.exe.Release.config"
  File "Release\DolPicCrawler.pdb"
  File "Release\DolPicCrawler.vshost.exe"
  File "Release\DolPicCrawler.vshost.exe.config"
  File "Release\DolPicCrawler.vshost.exe.manifest"
  File "Release\log4net.dll"
  File "Release\log4net.xml"
  File "Release\Newtonsoft.Json.dll"
  File "Release\Newtonsoft.Json.xml"
  File "Release\System.Net.Http.dll"
  File "Release\System.Net.Http.Extensions.dll"
  File "Release\System.Net.Http.Formatting.dll"
  File "Release\System.Net.Http.Primitives.dll"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\돌픽 크롤러\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\돌픽 크롤러\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\DolPicCrawler.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\DolPicCrawler.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name)는(은) 완전히 제거되었습니다."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(^Name)을(를) 제거하시겠습니까?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\System.Net.Http.Primitives.dll"
  Delete "$INSTDIR\System.Net.Http.Formatting.dll"
  Delete "$INSTDIR\System.Net.Http.Extensions.dll"
  Delete "$INSTDIR\Newtonsoft.Json.xml"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\log4net.xml"
  Delete "$INSTDIR\log4net.dll"
  Delete "$INSTDIR\DolPicCrawler.vshost.exe.manifest"
  Delete "$INSTDIR\DolPicCrawler.vshost.exe.config"
  Delete "$INSTDIR\DolPicCrawler.vshost.exe"
  Delete "$INSTDIR\DolPicCrawler.pdb"
  Delete "$INSTDIR\DolPicCrawler.exe.Release.config"
  Delete "$INSTDIR\DolPicCrawler.exe.local-test.config"
  Delete "$INSTDIR\DolPicCrawler.exe.deploy-test.config"
  Delete "$INSTDIR\DolPicCrawler.exe.Debug.config"
  Delete "$INSTDIR\DolPicCrawler.exe.config"
  Delete "$INSTDIR\DolPicCrawler.exe"

  Delete "$SMPROGRAMS\돌픽 크롤러\Uninstall.lnk"
  Delete "$SMPROGRAMS\돌픽 크롤러\Website.lnk"
  Delete "$DESKTOP\돌픽 크롤러.lnk"
  Delete "$SMPROGRAMS\돌픽 크롤러\돌픽 크롤러.lnk"

  RMDir "$SMPROGRAMS\돌픽 크롤러"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd