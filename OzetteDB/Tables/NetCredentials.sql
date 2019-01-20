CREATE TABLE [dbo].[NetCredentials]
(
	[ID] INT NOT NULL IDENTITY(1,1),
	[CredentialName] NVARCHAR(256) NOT NULL,

	CONSTRAINT PK_NetCredentials_ID PRIMARY KEY ([ID]),
	INDEX IDX_NetCredentials_CredentialName ([CredentialName])
)
