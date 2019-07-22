CREATE PROCEDURE [dbo].[FindAllArchivialFilesToRestore]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- transaction
	
	BEGIN TRY
		
		SELECT	[dbo].[BackupFiles].[ID],
				[dbo].[BackupFiles].[FileName],
				[dbo].[BackupFiles].[Directory],
				[dbo].[BackupFiles].[FullSourcePath],
				[dbo].[BackupFiles].[FileSizeBytes],
				[dbo].[BackupFiles].[LastModified],
				[dbo].[BackupFiles].[FileRevisionNumber],
				[dbo].[BackupFiles].[FileHashString],
				[dbo].[BackupFiles].[HashAlgorithmType]
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[OverallState] = 3

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END