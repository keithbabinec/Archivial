﻿CREATE TABLE [dbo].[MedPriBackupFilesQueue]
(
	[OrderID] BIGINT NOT NULL IDENTITY(1,1),
	[FileID] UNIQUEIDENTIFIER NOT NULL,
	[AssignedInstanceID] INT NOT NULL,

	CONSTRAINT PK_MedPriBackupFilesQueue_OrderID PRIMARY KEY ([OrderID]),
	CONSTRAINT FK_MedPriBackupFilesQueue_FileID FOREIGN KEY ([FileID]) REFERENCES [dbo].[BackupFiles] ([ID])
)