@echo off
chcp 65001 >nul
echo 正在停止所有服務...
echo.

echo 停止 Docker 容器...
docker-compose down

echo.
echo ✅ 所有服務已停止！
echo.
echo 💡 提示: 
echo    - 要重新啟動服務請執行 start-services-only.bat
echo    - 要啟動完整開發環境請執行 start-dev.bat
echo.
pause
