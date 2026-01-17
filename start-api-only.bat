@echo off
echo Starting API Server...
cd SchoolManagement.API
echo.
echo Applying migrations...
dotnet ef database update
echo.
echo Starting server on http://localhost:5001 and https://localhost:7001
dotnet run
