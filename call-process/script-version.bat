@echo off
setlocal enabledelayedexpansion

echo ===========================================
echo Checking versions of applications named
echo ===========================================

set "NAME=MauiApp3"
set "VersionList="
set "TargetVersion=2.0.4.3"
set "FoundVersion=No"

:: Sử dụng PowerShell để tìm các ứng dụng có tên chứa giá trị trong biến NAME
for /f "tokens=*" %%A in ('powershell -command "Get-Process -Name %NAME% -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Path | ForEach-Object { $_ + '#' + (Get-Item $_).VersionInfo.FileVersion }"') do (
    echo Found Path and Version: %%A
    set "VersionList=!VersionList!%%A;"
)

echo ===========================================
echo All versions and paths found: %VersionList%
echo ===========================================

:: Kiểm tra xem TargetVersion có trong VersionList hay không
for %%V in (!VersionList!) do (
    set "Line=%%V"
    for /f "tokens=1,2 delims=#" %%X in ("!Line!") do (
        echo Checking version: %%Y
        if "%%Y"=="%TargetVersion%" (
            set "FoundVersion=Yes"
            echo Version %TargetVersion% found at path: %%X
            goto :found
        )
    )
)
:found
if "%FoundVersion%"=="No" (
    echo Version %TargetVersion% not found.
)

echo ===========================================
echo Done.
endlocal
pause
