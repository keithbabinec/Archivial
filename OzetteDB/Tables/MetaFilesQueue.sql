CREATE TABLE [dbo].[MetaFilesQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_MetaFilesQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_MetaFilesQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)
