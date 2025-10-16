@echo off
chcp 65001 >nul
echo 正在啟動資料庫和 Redis 服務（不含 Web API）...
echo.

echo 0. 停止既有的 Docker 容器...
docker-compose down

echo.
echo 1. 啟動 Docker SQL Server 和 Redis 容器...
docker-compose up -d

echo.
echo 2. 等待資料庫啟動 (30 秒)...
timeout /t 30 /nobreak

echo.
echo 3. 執行資料庫初始化腳本...
sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\init.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ 資料庫初始化成功！
    echo.
    echo 📋 服務狀態:
    echo    - SQL Server: localhost:1434
    echo    - Redis: localhost:6379
    echo.
    echo 🚀 要啟動 Web API，請執行以下指令:
    echo    dotnet run --project WebApplication1/WebApplication1 --launch-profile https
    echo.
    echo 📝 Swagger UI: https://localhost:7194/swagger
) else (
    echo.
    echo ❌ 資料庫初始化失敗！請檢查 SQL Server 是否正常啟動。
    echo 您可以手動執行以下指令：
    echo sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\init.sql"
    pause
)

echo.
echo 💡 提示: 要停止服務請執行 stop-services.bat
pause