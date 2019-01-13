CREATE TABLE [dbo].[DirectoryMapItems]
(
	[ID] UNIQUEIDENTIFIER NOT NULL,
	[LocalPath] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT PK_DirectoryMapItems_ID PRIMARY KEY ([ID]),
	INDEX IDX_DirectoryMapItems_LocalPath ([LocalPath])
)
