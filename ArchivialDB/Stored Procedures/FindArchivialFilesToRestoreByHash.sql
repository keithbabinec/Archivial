CREATE PROCEDURE [dbo].[FindArchivialFilesToRestoreByHash]
(
	@FileHash				NVARCHAR(1024),
	@LimitResults			INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @FileHash IS NULL
	BEGIN
		;THROW 50000, 'FileHash must be populated.', 0
	END

	IF @LimitResults IS NULL
	BEGIN
		;THROW 50000, 'LimitResults must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		IF @LimitResults > 0
		BEGIN
			SELECT TOP (@LimitResults)
					[dbo].[BackupFiles].[ID],
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
			AND		CONTAINS([dbo].[BackupFiles].[FileHashString], @FileHash)
		END
		ELSE
		BEGIN
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
			AND		CONTAINS([dbo].[BackupFiles].[FileHashString], @FileHash)
		END

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END