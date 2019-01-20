CREATE TABLE [dbo].[BackupFiles]
(
	[ID] UNIQUEIDENTIFIER NOT NULL,
	[FileName] NVARCHAR(512) NOT NULL,
	[Directory] NVARCHAR(1024) NOT NULL,
	[FullSourcePath] NVARCHAR(1024) NOT NULL,
	[FileSizeBytes] BIGINT NOT NULL,
	[LastModified] DATETIME NOT NULL,
	[TotalFileBlocks] INT NOT NULL,
	[FileHash] VARBINARY(512) NOT NULL,
	[FileHashString] NVARCHAR(512) NOT NULL,
	[Priority] INT NOT NULL,
	[FileRevisionNumber] INT NOT NULL,
	[HashAlgorithmType] NVARCHAR(512) NOT NULL,
	[LastChecked] DATETIME NOT NULL,
	[LastUpdated] DATETIME NOT NULL,
	[OverallState] INT NOT NULL,

	CONSTRAINT PK_BackupFiles_ID PRIMARY KEY ([ID]),
	INDEX IDX_BackupFiles_FileName ([FileName]),
	INDEX IDX_BackupFiles_FullSourcePath ([FullSourcePath]),
	INDEX IDX_BackupFiles_FileHashString ([FileHashString])
)
