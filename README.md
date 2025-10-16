# Work Item List 應用程式開發專案

這是一個全端 Web 應用程式，旨在提供一個工作項目 (Work Item) 的管理與互動平台。前台使用者登入後可以查看、勾選、確認並檢視由後台管理者所建立的工作項目列表。

此專案的核心挑戰在於實現**個人化狀態的持久化**，確保每位使用者的勾選與確認狀態都是獨立保存的，並且在他們重新訪問時能夠被還原。

## 專案簡介 (Project Overview)

本應用程式分為兩大主要部分：

1.  **前台 (Frontend):** 供一般使用者登入後與工作項目列表互動的介面。使用者可以瀏覽所有項目，並對其進行個人化的勾選與確認操作。
2.  **後台 (Backend):** 提供管理者或系統管理員一個安全的介面或 API，用以對工作項目進行新增、讀取、更新和刪除 (CRUD) 操作。

## 核心功能 (Core Features)

### 前台功能 (For End Users)

  - **使用者認證:**
      - 使用者可以進行登入與登出。
      - 系統需驗證使用者身份以存取列表。
  - **查看工作項目列表:**
      - 登入後，使用者可以看到由管理者發布的所有工作項目。
  - **個人化勾選操作:**
      - 使用者可以對列表中的任一項目進行「勾選」或「取消勾選」。
      - 此勾選狀態**僅對當前使用者有效**，不會影響其他使用者看到的狀態。
  - **狀態確認與保存:**
      - 使用者點擊「確認 (Confirm)」按鈕後，系統會將該使用者當前的勾-選狀態保存至後端資料庫。
  - **狀態持久化:**
      - 當使用者登出後再重新登入，或關閉瀏覽器後再次訪問，系統應還原其上一次確認後的操作狀態。
  - **查看項目詳情:**
      - 使用者可以點擊任一工作項目，以查看其詳細資訊（如：詳細描述、建立時間等）。

### 後台功能 (For Administrators)

  - **工作項目管理 (CRUD):**
      - **新增 (Create):** 建立新的工作項目，包含標題、詳細描述等資訊。
      - **讀取 (Read):** 查詢並瀏覽所有已建立的工作項目。
      - **更新 (Update):** 修改現有工作項目的內容。
      - **刪除 (Delete):** 從系統中移除不再需要的工作項目。
  - **安全的管理介面:**
      - 後台功能應受到保護，僅限授權的管理者存取。

## 技術棧建議 (Suggested Tech Stack)

| 元件 (Component) | 技術 (Technology)                               | 備註 (Notes)                                              |
| :--------------- | :---------------------------------------------- | :-------------------------------------------------------- |
| **前端 (Frontend)** | `React`, `Vue.js`, 或 `Angular`                 | 用於建構互動式且響應快速的使用者介面。                        |
| **後端 (Backend)** | `Node.js` (`Express.js`), `Python` (`Django`/`Flask`) | 處理業務邏輯、API 請求及資料庫互動。                        |
| **資料庫 (Database)** | `PostgreSQL`, `MySQL`, 或 `MongoDB`             | `PostgreSQL` 或 `MySQL` (SQL) 適合處理關聯性資料，`MongoDB` (NoSQL) 則提供較高的靈活性。 |
| **使用者認證** | `JWT` (JSON Web Tokens)                         | 用於實現無狀態 (Stateless) 的使用者身份驗證機制。           |

## 資料庫結構設計 (Database Schema)

為了實現個人化的勾選狀態，建議採用以下三個核心資料表的設計：

1.  **`Users` Table**

      - 儲存使用者基本資訊。
      - `user_id` (Primary Key)
      - `username`
      - `password_hash`
      - ...其他使用者欄位

2.  **`WorkItems` Table**

      - 儲存由管理者建立的工作項目。
      - `item_id` (Primary Key)
      - `title`
      - `description`
      - `created_at`
      - `updated_at`
      - ...其他項目相關欄位

3.  **`UserWorkItemStates` Table (核心)**

      - 這是一個**關聯表 (Junction Table)**，用來記錄每位使用者對每個工作項目的個人狀態。
      - `state_id` (Primary Key)
      - `user_id` (Foreign Key, references `Users.user_id`)
      - `item_id` (Foreign Key, references `WorkItems.item_id`)
      - `is_checked` (Boolean)
      - `is_confirmed` (Boolean)
      - `last_updated` (Timestamp)

## API 端點設計 (API Endpoint Design)

以下是建議的 RESTful API 設計：

#### 使用者認證 (Auth)

  - `POST /api/auth/login`: 使用者登入，成功後回傳 JWT。
  - `POST /api/auth/register`: (可選) 註冊新使用者。

#### 後台管理 (Admin-Only)

  - `POST /api/work-items`: 新增一個工作項目。
  - `PUT /api/work-items/:itemId`: 更新指定 ID 的工作項目。
  - `DELETE /api/work-items/:itemId`: 刪除指定 ID 的工作項目。

#### 前台資料 (User-Facing)

  - `GET /api/work-items`: 取得所有工作項目列表。
      - **核心邏輯**: 後端在回傳此列表時，需根據當前登入使用者的 `user_id`，去 `UserWorkItemStates` 表中查詢並附加該使用者的 `is_checked` 狀態。
  - `GET /api/work-items/:itemId`: 取得單一工作項目的詳細資料。
  - `POST /api/user/states/confirm`: **確認並儲存狀態**。
      - **請求 Body 範例**:
        ```json
        {
          "states": [
            { "itemId": 1, "isChecked": true },
            { "itemId": 2, "isChecked": false },
            { "itemId": 5, "isChecked": true }
          ]
        }
        ```
      - 後端接收到請求後，會根據 `user_id` 與 `itemId` 批次更新或建立 `UserWorkItemStates` 表中的紀錄。

## 專案執行方法 (Getting Started)

1.  **環境準備:**

      - 安裝 Node.js, Python 或其他後端運行環境。
      - 安裝對應的資料庫 (如 PostgreSQL)。

2.  **Clone 專案:**

    ```bash
    git clone <repository-url>
    cd <project-folder>
    ```

3.  **後端設定:**

    ```bash
    cd backend
    npm install # 或 pip install -r requirements.txt
    # 設定 .env 檔案，填入資料庫連線資訊、JWT 密鑰等
    cp .env.example .env
    # 啟動後端伺服器
    npm start # 或 python app.py
    ```

4.  **前端設定:**

    ```bash
    cd frontend
    npm install
    # 設定環境變數，指向後端 API 位址
    # 啟動前端開發伺服器
    npm start
    ```

5.  **訪問應用程式:**

      - 開啟瀏覽器並訪問 `http://localhost:3000` (或前端設定的 Port)。