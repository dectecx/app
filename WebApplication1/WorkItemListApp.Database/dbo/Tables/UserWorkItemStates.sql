CREATE TABLE [dbo].[UserWorkItemStates] (
    [StateId]     INT            IDENTITY (1, 1) NOT NULL,
    [UserId]      INT            NOT NULL,
    [WorkItemId]  INT            NOT NULL,
    [IsChecked]   BIT            NOT NULL,
    [IsConfirmed] BIT            NOT NULL,
    [CreatedUser] NVARCHAR (256) NULL,
    [CreatedTime] DATETIME2 (7)  NOT NULL,
    [UpdatedUser] NVARCHAR (256) NULL,
    [UpdatedTime] DATETIME2 (7)  NULL,
    PRIMARY KEY CLUSTERED ([StateId] ASC),
    CONSTRAINT [FK_UserWorkItemStates_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserWorkItemStates_WorkItems] FOREIGN KEY ([WorkItemId]) REFERENCES [dbo].[WorkItems] ([Id]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserWorkItemStates_UserId_WorkItemId]
    ON [dbo].[UserWorkItemStates]([UserId] ASC, [WorkItemId] ASC);

