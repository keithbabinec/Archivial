CREATE TABLE [dbo].[LowPriBackupFilesQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_LowPriBackupFilesQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_LowPriBackupFilesQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)