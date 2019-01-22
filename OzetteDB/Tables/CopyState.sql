CREATE TABLE [dbo].[CopyState]
(
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[StorageProvider] INT NOT NULL,
	[SyncStatus] INT NOT NULL,
	[HydrationStatus] INT NOT NULL,
	[LastCompletedFileBlockIndex] INT NOT NULL,

	CONSTRAINT PK_CopyState_FileID PRIMARY KEY ([FileID]),
	CONSTRAINT FK_CopyState_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)
