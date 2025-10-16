CREATE TABLE [dbo].[Users] (
    [UserId]       INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (256) NULL,
    [PasswordHash] NVARCHAR (MAX) NULL,
    [CreatedUser]  NVARCHAR (256) NULL,
    [CreatedTime]  DATETIME2 (7)  NOT NULL,
    [UpdatedUser]  NVARCHAR (256) NULL,
    [UpdatedTime]  DATETIME2 (7)  NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);

