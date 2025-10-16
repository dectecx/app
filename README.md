# 批次檔案說明

## 📁 可用的批次檔案

### 🚀 start-dev.bat
**一鍵啟動完整開發環境**
- 停止既有的 Docker 容器
- 啟動 SQL Server 和 Redis 容器
- 等待 15 秒讓資料庫啟動
- 執行 init.sql 建立資料庫結構
- 執行 insert-roles.sql 插入預設角色
- 自動啟動 .NET Web API
- 開啟 Swagger UI: https://localhost:7194/swagger

### 🛑 stop-services.bat
**停止所有服務**
- 停止並移除所有 Docker 容器
- 清理資源

## 🎯 使用方式

### 啟動開發環境
```bash
start-dev.bat
```

### 停止服務
```bash
stop-services.bat
```

## 📋 服務資訊

- **SQL Server**: localhost:1434
- **Redis**: localhost:6379
- **Web API**: https://localhost:7194
- **Swagger UI**: https://localhost:7194/swagger

## 🔧 測試功能

### TestData API（僅限開發環境）
- `POST /api/TestData/create-test-users`: 建立測試使用者 (admin/123456, user/123456)
- `POST /api/TestData/login-test-admin`: Admin 登入測試
- `POST /api/TestData/login-test-user`: User 登入測試
- `GET /api/TestData/test-users-status`: 檢查測試使用者狀態
- `DELETE /api/TestData/delete-test-users`: 刪除測試使用者

### CacheTest API（僅限開發環境）
- `GET /api/CacheTest/info`: 查看目前使用的快取提供者
- `POST /api/CacheTest/set`: 設定快取值
- `GET /api/CacheTest/get/{key}`: 取得快取值
- `DELETE /api/CacheTest/remove/{key}`: 移除快取值
- `GET /api/CacheTest/exists/{key}`: 檢查快取鍵是否存在

## ⚠️ 注意事項

- 確保已安裝 Docker Desktop、.NET 8 SDK 和 SQL Server Command Line Tools
- 如果資料庫初始化失敗，請檢查 SQL Server 容器是否正常啟動
- TestData 和 CacheTest API 僅在開發環境中可用
