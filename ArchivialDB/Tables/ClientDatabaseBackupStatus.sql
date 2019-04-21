CREATE TABLE [dbo].[ClientDatabaseBackupStatus]
(
	[LastFullBackup] DATETIME NULL,
	[LastDifferentialBackup] DATETIME NULL,
	[LastTransactionLogBackup] DATETIME NULL
)
