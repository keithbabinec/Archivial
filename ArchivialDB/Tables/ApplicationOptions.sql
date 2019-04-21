CREATE TABLE [dbo].[ApplicationOptions]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Name] NVARCHAR(512) NOT NULL,
	[Value] NVARCHAR(MAX) NOT NULL,

	CONSTRAINT PK_ApplicationOptions_ID PRIMARY KEY ([ID]),
	INDEX IDX_ApplicationOptions_Name ([Name])
)
