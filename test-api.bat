@echo off
echo Testing API Connection...
echo.

echo Testing HTTP endpoint (http://localhost:5001)...
curl -X GET http://localhost:5001/api/health 2>nul
if %errorlevel% equ 0 (
    echo [OK] HTTP endpoint is accessible
) else (
    echo [FAIL] HTTP endpoint not accessible
)

echo.
echo Testing HTTPS endpoint (https://localhost:7001)...
curl -k -X GET https://localhost:7001/api/health 2>nul
if %errorlevel% equ 0 (
    echo [OK] HTTPS endpoint is accessible
) else (
    echo [FAIL] HTTPS endpoint not accessible
)

echo.
echo Testing Signup endpoint...
curl -X POST http://localhost:5001/api/auth/signup ^
  -H "Content-Type: application/json" ^
  -d "{\"schoolName\":\"Test School\",\"adminName\":\"Admin\",\"email\":\"test@test.com\",\"phone\":\"1234567890\",\"address\":\"Test\",\"password\":\"test123\"}" 2>nul

echo.
echo.
pause
