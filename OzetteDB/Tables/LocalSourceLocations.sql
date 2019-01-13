CREATE TABLE [dbo].[LocalSourceLocations]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Path] NVARCHAR(MAX) NOT NULL,
	[FileMatchFilter] NVARCHAR(2048) NOT NULL,
	[Priority] INT NOT NULL,
	[RevisionCount] INT NOT NULL,
	[LastCompletedScan] DATETIME NOT NULL,

	CONSTRAINT PK_LocalSourceLocations_ID PRIMARY KEY ([ID]),
	INDEX IDX_LocalSourceLocations_Path ([Path])
)
