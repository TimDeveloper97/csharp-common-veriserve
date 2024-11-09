@echo off
:: Tên file batch: build-release.bat
:: Mô tả: Build dự án .NET MAUI ở chế độ Release

:: Thực hiện lệnh git pull
git pull

:: Kiểm tra kết quả của lệnh git pull
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Git pull failed! Please check the errors above.
    pause
    exit /b %ERRORLEVEL%
)

:: Điều hướng đến thư mục chứa file .csproj
set PROJECT_PATH=O:\Git\MauiApp3\MauiApp3\

:: Tìm kiếm file .csproj trong thư mục PROJECT_PATH
for /r "%PROJECT_PATH%" %%f in (*.csproj) do (
    set CSPROJ_FILE=%%f
)

cd /d "%PROJECT_PATH%"

:: setup version
set NEW_VERSION=4.0.4.3

:: get application version
for /f "tokens=1,2 delims=." %%a in ("%NEW_VERSION%") do set DISPLAY_VERSION=%%a.%%b

:: Lấy phần cuối "0" và gán vào biến B
for /f "tokens=4 delims=." %%c in ("%NEW_VERSION%") do set APP_VERSION=%%c

:: start build
echo =============================================
echo Building .NET MAUI Project in Release Mode
echo =============================================

:: update version
powershell -Command "(Get-Content '%CSPROJ_FILE%') -replace '<AssemblyVersion>.*</AssemblyVersion>', '<AssemblyVersion>%NEW_VERSION%</AssemblyVersion>' -replace '<FileVersion>.*</FileVersion>', '<FileVersion>%NEW_VERSION%</FileVersion>' -replace '<ApplicationDisplayVersion>.*</ApplicationDisplayVersion>', '<ApplicationDisplayVersion>%DISPLAY_VERSION%</ApplicationDisplayVersion>'-replace '<ApplicationVersion>.*</ApplicationVersion>', '<ApplicationVersion>%APP_VERSION%</ApplicationVersion>'| Set-Content '%CSPROJ_FILE%'"

:: clean
dotnet clean

:: Build Release
dotnet build -c Release

:: Kiểm tra kết quả build
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build failed! Please check the errors above.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo Build succeeded!
pause
