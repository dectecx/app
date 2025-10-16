@echo off
echo ======================================================
echo          Work Item List - Dev Start Script
echo ======================================================
echo.

echo [1/3] Starting SQL Server container in the background...
docker-compose up -d

echo.
echo [2/3] Waiting for 15 seconds for the database to initialize...
timeout /t 15 /nobreak

echo.
echo [3/3] Starting .NET Web API...
dotnet run --project WebApplication1/WebApplication1

echo.
echo ======================================================
echo      Script finished. You can close this window.
echo ======================================================
pause
