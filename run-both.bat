@echo off
echo Starting School Management System...
echo.

echo [1/3] Building API...
cd SchoolManagement.API
dotnet build -c Debug
if %errorlevel% neq 0 (
    echo API build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo [1.5/3] Applying database migrations...
dotnet ef database update
if %errorlevel% neq 0 (
    echo Migration failed!
    pause
    exit /b %errorlevel%
)

echo.
echo [2/3] Building Windows App...
cd ..\SchoolManagement
dotnet build -f net9.0-windows10.0.19041.0 -c Debug
if %errorlevel% neq 0 (
    echo Windows App build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo [3/3] Starting applications...
cd ..

start "School Management API" cmd /k "cd SchoolManagement.API && dotnet run"
timeout /t 5 /nobreak > nul

start "School Management App" cmd /k "cd SchoolManagement && dotnet run -f net9.0-windows10.0.19041.0"

echo.
echo Both applications are starting...
echo API will be available at https://localhost:7001
echo.
pause
