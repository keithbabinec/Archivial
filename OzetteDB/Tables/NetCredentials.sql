CREATE TABLE [dbo].[NetCredentials]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[CredentialName] NVARCHAR(2048) NOT NULL,

	CONSTRAINT PK_NetCredentials_ID PRIMARY KEY ([ID])
)
