@echo off
setlocal enabledelayedexpansion
echo ========================================
echo School Management - Android Testing
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
        echo Please install Android SDK Platform Tools
        echo Download: https://developer.android.com/tools/releases/platform-tools
        echo Or restart command prompt after adding ADB to PATH
        pause
        exit /b 1
    )
)

echo ADB found: 
where adb
echo.

echo [1/4] Building API...
cd SchoolManagement.API
dotnet build -c Debug
if %errorlevel% neq 0 (
    echo API build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo [2/4] Starting API Server...
cd ..
start "School Management API" cmd /k "cd SchoolManagement.API && dotnet run"
echo Waiting for API to start...
timeout /t 5 /nobreak > nul

echo.
echo [3/4] Setting up ADB port forwarding...
echo Checking for connected devices...
adb devices

echo.
echo Setting up port forwarding (localhost:7001 on device -> localhost:7001 on PC)...
adb reverse tcp:7001 tcp:7001
if %errorlevel% equ 0 (
    echo ✓ Port forwarding configured successfully
    echo Android app can now access API at: http://localhost:7001
) else (
    echo ⚠ Port forwarding failed - device might not be connected
    echo.
    echo For Android Emulator: App will use http://10.0.2.2:7001 automatically
    echo For Real Device: Make sure USB debugging is enabled
)

echo.
echo [4/4] Building and deploying Android app...
cd SchoolManagement
dotnet build -f net9.0-android -c Debug
if %errorlevel% neq 0 (
    echo Android build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo Deploying to device/emulator...
echo.
echo If app crashes, run: view-android-logs.bat
echo.
dotnet build -t:Run -f net9.0-android

echo.
echo ========================================
echo Setup Complete!
echo ========================================
echo.
echo API Server: https://localhost:7001
echo Android App: Deployed to device/emulator
echo.
echo IMPORTANT NOTES:
echo ================
echo.
echo For Android Emulator:
echo   - App uses: http://10.0.2.2:7001
echo   - No additional setup needed
echo.
echo For Real Android Device (USB):
echo   1. Enable USB Debugging on phone
echo   2. Connect via USB
echo   3. Run: adb reverse tcp:7001 tcp:7001
echo   4. App uses: http://localhost:7001
echo.
echo For Real Android Device (WiFi):
echo   1. Find your PC's IP address (ipconfig)
echo   2. Update ApiConfig.cs with your IP
echo   3. Example: http://192.168.1.100:7001
echo   4. Make sure firewall allows port 7001
echo.
echo To stop API server, close the API command window
echo.
pause
