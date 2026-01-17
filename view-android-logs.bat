@echo off
setlocal enabledelayedexpansion
echo ========================================
echo Android App Crash Logs
echo ========================================
echo.

REM Check if ADB is available
where adb >nul 2>nul
if %errorlevel% neq 0 (
    set "ADB_PATH="
    if exist "%LOCALAPPDATA%\Android\Sdk\platform-tools\adb.exe" (
        set "ADB_PATH=%LOCALAPPDATA%\Android\Sdk\platform-tools"
    ) else if exist "%USERPROFILE%\AppData\Local\Android\Sdk\platform-tools\adb.exe" (
        set "ADB_PATH=%USERPROFILE%\AppData\Local\Android\Sdk\platform-tools"
    )
    if defined ADB_PATH (
        set "PATH=!ADB_PATH!;%PATH%"
    ) else (
        echo ADB not found!
        pause
        exit /b 1
    )
)

echo Clearing old logs...
adb logcat -c

echo.
echo Watching for SchoolManagement app logs...
echo Press Ctrl+C to stop
echo.
echo ========================================
echo.

adb logcat | findstr /i "SchoolManagement mono crash exception error"
