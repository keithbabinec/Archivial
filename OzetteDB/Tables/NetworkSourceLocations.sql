CREATE TABLE [dbo].[NetworkSourceLocations]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Path] NVARCHAR(MAX) NOT NULL,
	[FileMatchFilter] NVARCHAR(2048) NOT NULL,
	[Priority] INT NOT NULL,
	[RevisionCount] INT NOT NULL,
	[LastCompletedScan] DATETIME NOT NULL,
	[CredentialName] NVARCHAR(2048) NOT NULL,
	[IsConnected] BIT NOT NULL,
	[IsFailed] BIT NOT NULL,
	[LastConnectionCheck] DATETIME NULL,

	CONSTRAINT PK_NetworkSourceLocations_ID PRIMARY KEY ([ID]),
	INDEX IDX_NetworkSourceLocations_Path ([Path])
)
