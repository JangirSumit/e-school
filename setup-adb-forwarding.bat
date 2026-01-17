@echo off
setlocal enabledelayedexpansion
echo ========================================
echo ADB Port Forwarding Setup
echo ========================================
echo.

REM Check if ADB is available
where adb >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: ADB not found in PATH!
    echo.
    echo Trying common ADB locations...
    set "ADB_PATH="
    
    if exist "%LOCALAPPDATA%\Android\Sdk\platform-tools\adb.exe" (
        set "ADB_PATH=%LOCALAPPDATA%\Android\Sdk\platform-tools"
    ) else if exist "%USERPROFILE%\AppData\Local\Android\Sdk\platform-tools\adb.exe" (
        set "ADB_PATH=%USERPROFILE%\AppData\Local\Android\Sdk\platform-tools"
    ) else if exist "C:\Android\Sdk\platform-tools\adb.exe" (
        set "ADB_PATH=C:\Android\Sdk\platform-tools"
    )
    
    if defined ADB_PATH (
        echo Found ADB at: !ADB_PATH!
        set "PATH=!ADB_PATH!;%PATH%"
    ) else (
        echo Please install Android SDK Platform Tools:
        echo https://developer.android.com/tools/releases/platform-tools
        echo.
        echo After installation, add to PATH:
        echo Example: C:\Users\YourName\AppData\Local\Android\Sdk\platform-tools
        pause
        exit /b 1
    )
)

echo Checking connected devices...
adb devices
echo.

echo Setting up port forwarding for School Management API...
echo Port: 7001 (device) -> 7001 (PC)
echo.

adb reverse tcp:7001 tcp:7001

if %errorlevel% equ 0 (
    echo.
    echo ✓ SUCCESS! Port forwarding configured
    echo.
    echo Your Android app can now access:
    echo   http://localhost:7001
    echo.
    echo This forwards to your PC's API server at:
    echo   https://localhost:7001
    echo.
) else (
    echo.
    echo ✗ FAILED! Could not set up port forwarding
    echo.
    echo Possible reasons:
    echo   1. No device connected
    echo   2. USB debugging not enabled
    echo   3. Device not authorized
    echo.
    echo Solutions:
    echo   - Connect device via USB
    echo   - Enable USB Debugging in Developer Options
    echo   - Accept authorization prompt on device
    echo   - Try: adb kill-server then adb start-server
    echo.
)

echo.
echo To remove port forwarding:
echo   adb reverse --remove tcp:7001
echo.
echo To remove all port forwarding:
echo   adb reverse --remove-all
echo.
pause
