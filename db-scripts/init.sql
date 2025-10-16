-- 1. 建立資料庫 (如果它不存在)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WorkItemListApp')
BEGIN
    CREATE DATABASE [WorkItemListApp];
END
GO

-- 2. 切換到新的資料庫上下文
USE [WorkItemListApp];
GO

-- 3. 建立 Roles 資料表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [RoleId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(256) NULL,
        [CreatedTime] DATETIME2 NOT NULL,
        [UpdatedTime] DATETIME2 NULL
    );
END
GO

-- 4. 建立 Users 資料表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(256) NULL,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [CreatedUser] NVARCHAR(256) NULL,
        [CreatedTime] DATETIME2 NOT NULL,
        [UpdatedUser] NVARCHAR(256) NULL,
        [UpdatedTime] DATETIME2 NULL
    );
END
GO

-- 5. 建立 UserRoles 關聯表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        [AssignedTime] DATETIME2 NOT NULL,
        [AssignedBy] NVARCHAR(256) NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([RoleId]) ON DELETE CASCADE
    );
END
GO

-- 6. 建立 WorkItems 資料表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkItems' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[WorkItems] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(256) NULL,
        [Description] NVARCHAR(MAX) NULL,
        [CreatedUser] NVARCHAR(256) NULL,
        [CreatedTime] DATETIME2 NOT NULL,
        [UpdatedUser] NVARCHAR(256) NULL,
        [UpdatedTime] DATETIME2 NULL
    );
END
GO

-- 7. 建立 UserWorkItemStates 資料表 (關聯表)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserWorkItemStates' and xtype='U')
BEGIN
    CREATE TABLE [dbo].[UserWorkItemStates] (
        [StateId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NOT NULL,
        [WorkItemId] INT NOT NULL,
        [IsChecked] BIT NOT NULL,
        [IsConfirmed] BIT NOT NULL,
        [CreatedUser] NVARCHAR(256) NULL,
        [CreatedTime] DATETIME2 NOT NULL,
        [UpdatedUser] NVARCHAR(256) NULL,
        [UpdatedTime] DATETIME2 NULL,
        CONSTRAINT [FK_UserWorkItemStates_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserWorkItemStates_WorkItems] FOREIGN KEY ([WorkItemId]) REFERENCES [dbo].[WorkItems]([Id]) ON DELETE CASCADE
    );
END
GO

-- 8. 建立索引和約束
-- Roles 名稱唯一索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_Name' AND object_id = OBJECT_ID('dbo.Roles'))
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [dbo].[Roles]([Name]);
END
GO

-- UserWorkItemStates 唯一索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserWorkItemStates_UserId_WorkItemId' AND object_id = OBJECT_ID('dbo.UserWorkItemStates'))
BEGIN
    CREATE UNIQUE INDEX [IX_UserWorkItemStates_UserId_WorkItemId] ON [dbo].[UserWorkItemStates]([UserId], [WorkItemId]);
END
GO

-- 9. 插入預設角色資料
IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = N'User')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [CreatedTime])
    VALUES (N'User', N'前台使用者', GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = N'Admin')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [CreatedTime])
    VALUES (N'Admin', N'後台管理員', GETUTCDATE());
END
GO

PRINT N'資料庫與資料表建立完成。';
