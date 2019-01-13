CREATE TABLE [dbo].[SourceLocations]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Type] INT NOT NULL,
	[Path] NVARCHAR(MAX) NOT NULL,
	[FileMatchFilter] NVARCHAR(2048) NOT NULL,
	[Priority] INT NOT NULL,
	[RevisionCount] INT NOT NULL,
	[LastCompletedScan] DATETIME NOT NULL,

	CONSTRAINT PK_SourceLocations_ID PRIMARY KEY ([ID])
)
