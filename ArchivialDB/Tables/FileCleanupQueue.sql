CREATE TABLE [dbo].[FileCleanupQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[FileRevisionNumber] INT NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_FileCleanupQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_FileCleanupQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
) 