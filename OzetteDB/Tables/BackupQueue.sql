CREATE TABLE [dbo].[BackupQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_BackupQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_BackupQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)
