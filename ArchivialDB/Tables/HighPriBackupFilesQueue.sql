CREATE TABLE [dbo].[HighPriBackupFilesQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_HighPriBackupFilesQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_HighPriBackupFilesQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)