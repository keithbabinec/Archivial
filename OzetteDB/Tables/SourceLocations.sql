CREATE TABLE [dbo].[SourceLocations]
(
	[ID] INT NOT NULL,
	[Type] INT NOT NULL,
	[Path] NVARCHAR(MAX) NOT NULL,
	[FileMatchFilter] NVARCHAR(2048) NOT NULL,
	[Priority] INT NOT NULL,
	[RevisionCount] INT NOT NULL,
	[LastCompletedScan] DATETIME NOT NULL
)
