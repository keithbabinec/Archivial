CREATE TABLE [dbo].[DirectoryMapItems]
(
	[ID] UNIQUEIDENTIFIER NOT NULL,
	[LocalPath] NVARCHAR(1024) NOT NULL,

	CONSTRAINT PK_DirectoryMapItems_ID PRIMARY KEY ([ID]),
	INDEX IDX_DirectoryMapItems_LocalPath ([LocalPath])
)
