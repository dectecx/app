@echo off
chcp 65001 >nul
echo 正在啟動開發環境...
echo.

echo 0. 停止既有的 Docker 容器...
docker-compose down

echo.
echo 1. 啟動 Docker SQL Server 和 Redis 容器...
docker-compose up -d

echo.
echo 2. 等待資料庫啟動 (5 秒)...
timeout /t 5 /nobreak

echo.
echo 3. 執行資料庫結構建立腳本...
sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\init.sql"

if %ERRORLEVEL% NEQ 0 (
    echo ❌ 資料庫結構建立失敗！
    pause
    exit /b 1
)

echo.
echo 4. 執行預設角色資料插入腳本...
sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\insert-roles.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ 資料庫初始化成功！
    echo.
    echo 📋 服務狀態:
    echo    - SQL Server: localhost:1434
    echo    - Redis: localhost:6379
    echo.
    echo 5. 啟動 .NET Web API...
    echo    Swagger UI: https://localhost:7194/swagger
    echo.
    dotnet run --project WebApplication1/WebApplication1 --launch-profile https
) else (
    echo.
    echo ❌ 預設角色資料插入失敗！請檢查 SQL Server 是否正常啟動。
    echo 您可以手動執行以下指令：
    echo sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\insert-roles.sql"
    pause
)