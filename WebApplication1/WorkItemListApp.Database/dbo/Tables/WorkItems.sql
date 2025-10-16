CREATE TABLE [dbo].[WorkItems] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (256) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Status]      NVARCHAR (50)  NULL,
    [CreatedUser] NVARCHAR (256) NULL,
    [CreatedTime] DATETIME2 (7)  NOT NULL,
    [UpdatedUser] NVARCHAR (256) NULL,
    [UpdatedTime] DATETIME2 (7)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

