-- 插入預設角色資料
-- 此檔案用於插入預設的角色資料

-- 切換到 WorkItemListApp 資料庫
USE [WorkItemListApp];
GO

PRINT N'Starting to insert default role data...';

-- Check if Roles table exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
BEGIN
    PRINT N'Error: Roles table does not exist!';
    RETURN;
END

-- Insert User role
IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = N'User')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [CreatedTime])
    VALUES (N'User', N'Frontend User', GETUTCDATE());
    PRINT N'User role inserted successfully';
END
ELSE
BEGIN
    PRINT N'User role already exists';
END
GO

-- Insert Admin role
IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = N'Admin')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [CreatedTime])
    VALUES (N'Admin', N'Backend Administrator', GETUTCDATE());
    PRINT N'Admin role inserted successfully';
END
ELSE
BEGIN
    PRINT N'Admin role already exists';
END
GO

-- Show final results
PRINT N'Final role data:';
SELECT [RoleId], [Name], [Description], [CreatedTime] FROM [dbo].[Roles];
GO

PRINT N'Default role data insertion completed!';
