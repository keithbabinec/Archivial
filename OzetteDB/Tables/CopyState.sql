CREATE TABLE [dbo].[CopyState]
(
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[StorageProvider] INT NOT NULL,
	[SyncStatus] INT NOT NULL,
	[HydrationStatus] INT NOT NULL,
	[LastCompletedFileBlockIndex] INT NOT NULL
)
