CREATE TABLE [dbo].[UserRoles] (
    [UserId]       INT            NOT NULL,
    [RoleId]       INT            NOT NULL,
    [AssignedTime] DATETIME2 (7)  NOT NULL,
    [AssignedBy]   NVARCHAR (256) NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

