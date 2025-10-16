# 專案快速上手指南 (Quick Start)

本指南將引導您如何在本機環境中設定、啟動並使用這個專案。

## 懶人包 (TL;DR - The Easy Way)

我們提供了多個批次檔 (`.bat`)，可以一鍵啟動開發環境。

### 🚀 完整開發環境啟動
1.  確保您的電腦上已安裝 **.NET 8 SDK**、**Docker Desktop** 和 **SQL Server Command Line Tools**。
2.  在專案根目錄下，直接**雙擊執行 `start-dev.bat`**。
3.  腳本會自動完成以下所有事情：
    -   停止既有的 Docker 容器
    -   啟動 Docker SQL Server 和 Redis 容器
    -   等待 30 秒，確保資料庫完全啟動
    -   執行 `init.sql` 腳本初始化資料庫
    -   啟動 .NET Web API
4.  看到 `https://localhost:7194/swagger` 的網址出現後，即可開始使用。

### 🔧 僅啟動服務（不含 Web API）
如果您只想啟動資料庫和 Redis，請執行 `start-services-only.bat`：
- 啟動 SQL Server 和 Redis 容器
- 初始化資料庫
- 提供手動啟動 Web API 的指令（使用 HTTPS 配置）
- **不會自動啟動 Web API**

### 🛑 停止服務
執行 `stop-services.bat` 來停止所有 Docker 容器。

**注意**: 如果自動初始化失敗，請參考「手動啟動」章節中的「步驟三：初始化資料庫」。

---

## 手動啟動 (The Manual Way)

如果您想了解每個步驟的細節，或是懶人包執行失敗，可以依照以下手動步驟執行。

### 1. 環境需求 (Prerequisites)

在開始之前，請確保您的電腦上已安裝以下軟體：

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**: 本專案的後端 API 使用 .NET 8 框架。
- **[Docker Desktop](https://www.docker.com/products/docker-desktop/)**: 我們使用 Docker 來容器化並執行 SQL Server 資料庫，以確保開發環境的一致性。
- **[SQL Server Command Line Tools](https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility)**: 用於執行 SQL 腳本來初始化資料庫。

## 2. 專案設定 (Configuration)

本專案的設定已盡可能簡化，您幾乎不需要手動修改任何設定檔。

- **資料庫**: 專案會自動連接到由 Docker 啟動的 SQL Server 容器。連線字串已設定在 `WebApplication1/WebApplication1/appsettings.json` 中。
- **資料庫結構**: 資料庫的結構 (Schema) 是透過 `db-scripts/init.sql` 檔案來定義的。您需要手動執行這個腳本來建立所需的資料庫和資料表。
- **快取服務**: 專案支援兩種快取方式：
  - **Memory Cache**: 預設使用記憶體快取，適用於單一應用程式實例
  - **Redis Cache**: 支援分散式快取，適用於多應用程式實例
  - 可在 `appsettings.json` 中修改 `CacheSettings.ProviderType` 來切換快取方式

### 3. 如何啟動專案 (Running the Project)

請依照以下三個步驟來啟動完整的應用程式：

### 步驟一：停止既有容器並啟動服務

首先，我們需要停止既有的 Docker 容器，然後啟動 SQL Server 資料庫容器和 Redis 快取服務。

打開您的終端機 (Terminal)，切換到本專案的**根目錄** (也就是 `docker-compose.yml` 所在的目錄)，然後執行以下指令：

```bash
# 停止既有的容器
docker-compose down

# 啟動開發環境 (資料庫和 Redis)
docker-compose up -d
```

- `docker-compose down` 會停止並移除既有的容器
- `-d` 參數會讓容器在背景模式下執行
- 第一次執行此指令時，Docker 會需要一些時間來下載 SQL Server 和 Redis 的映像檔
- 成功啟動後，您的資料庫會在 `localhost:1434` 上運行，Redis 會在 `localhost:6379` 上運行

### 步驟二：初始化資料庫

**重要**: SQL Server 容器啟動後，需要手動執行 SQL 腳本來建立資料庫結構。

等待約 30 秒讓 SQL Server 完全啟動，然後執行以下指令：

```bash
sqlcmd -S localhost,1434 -U sa -P "yourStrong(!)Password123" -i "db-scripts/init.sql"
```

- 這個指令會執行 `db-scripts/init.sql` 腳本，建立所需的資料庫和資料表。
- 成功執行後，您會看到 "資料庫與資料表建立完成。" 的訊息。

### 步驟三：啟動後端 API

資料庫成功初始化後，接著啟動 .NET Web API。

同樣在您的終端機中，執行以下指令：

```bash
dotnet run --project WebApplication1/WebApplication1 --launch-profile https
```

- 這個指令會建置並啟動您的 Web API 專案。
- 成功啟動後，您會看到終端機顯示 API 正在監聽 `https://localhost:7194` 和 `http://localhost:5102`。

## 4. 如何使用 (Usage)

專案成功啟動後，您可以透過以下方式與 API 互動：

- **Swagger UI**: 這是測試 API 最方便的方式。請在您的瀏覽器中開啟以下網址：
  > **https://localhost:7194/swagger**

- **API 基礎 URL**: `https://localhost:7194`

### 測試流程

1.  **註冊使用者**: 使用 `/api/Auth/register` 端點來註冊一個新帳號。
2.  **登入**: 使用 `/api/Auth/login` 端點，以剛剛註冊的帳號密碼登入，您將會得到一組 Access Token。
3.  **授權**: 點擊 Swagger 介面右上角的「**Authorize**」按鈕，在輸入框中填入 `Bearer <您的 Access Token>`，然後點擊「Authorize」。
4.  **測試受保護的 API**: 現在您可以測試 `WorkItems` 或 `user/states` 等需要授權的 API 了。Swagger 會自動在您的每個請求中加入 `Authorization` 標頭。
5.  **測試快取功能**: 使用 `/api/CacheTest` 端點來測試快取功能（**僅限開發環境**）：
    - `GET /api/CacheTest/info`: 查看目前使用的快取提供者
    - `POST /api/CacheTest/set`: 設定快取值
    - `GET /api/CacheTest/get/{key}`: 取得快取值
    - `DELETE /api/CacheTest/remove/{key}`: 移除快取值
    - `GET /api/CacheTest/exists/{key}`: 檢查快取鍵是否存在

6.  **測試資料管理**: 使用 `/api/TestData` 端點來管理測試資料（**僅限開發環境**）：
    - `POST /api/TestData/create-test-users`: 建立測試使用者 (admin/123456, user/123456)
    - `POST /api/TestData/login-test-admin`: Admin 登入測試
    - `POST /api/TestData/login-test-user`: User 登入測試
    - `GET /api/TestData/test-users-status`: 檢查測試使用者狀態
    - `DELETE /api/TestData/delete-test-users`: 刪除測試使用者
    
    **注意**: TestDataController 和 CacheTestController 僅在開發環境中可用，生產環境會回傳404。

## 5. 如何停止 (Stopping the Project)

- **停止 API**: 在執行 `dotnet run` 的終端機視窗中，按下 `Ctrl + C` 即可。
- **停止服務**: 若要停止並移除 Docker 容器，請在專案根目錄執行：
  ```bash
  docker-compose down
  ```

## 6. 快取服務切換 (Cache Service Switching)

### 切換到 Redis 快取
1. 修改 `WebApplication1/WebApplication1/appsettings.json` 中的 `CacheSettings.ProviderType` 為 `"Redis"`
2. 確保 Redis 服務正在運行
3. 重啟應用程式

### 切換到記憶體快取
1. 修改 `WebApplication1/WebApplication1/appsettings.json` 中的 `CacheSettings.ProviderType` 為 `"Memory"`
2. 重啟應用程式

### 測試快取切換
使用 `/api/CacheTest/info` 端點來確認目前使用的快取提供者。
