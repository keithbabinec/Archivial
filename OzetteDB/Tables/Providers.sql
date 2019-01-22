CREATE TABLE [dbo].[Providers]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[Type] INT NOT NULL,
	[Name] NVARCHAR(256) NOT NULL,

	CONSTRAINT PK_Providers_ID PRIMARY KEY ([ID]),
	INDEX IDX_Providers_Name ([Name])
)
