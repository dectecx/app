CREATE TABLE [dbo].[Roles] (
    [RoleId]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [CreatedTime] DATETIME2 (7)  NOT NULL,
    [UpdatedTime] DATETIME2 (7)  NULL,
    PRIMARY KEY CLUSTERED ([RoleId] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name]
    ON [dbo].[Roles]([Name] ASC);

