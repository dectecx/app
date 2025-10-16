@echo off
echo 正在啟動開發環境...
echo.

echo 1. 啟動 Docker SQL Server 容器...
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
    echo 4. 啟動 .NET Web API...
    dotnet run --project WebApplication1/WebApplication1
) else (
    echo.
    echo ❌ 資料庫初始化失敗！請檢查 SQL Server 是否正常啟動。
    echo 您可以手動執行以下指令：
    echo sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts\init.sql"
    pause
)