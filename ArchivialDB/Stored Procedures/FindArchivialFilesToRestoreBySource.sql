CREATE PROCEDURE [dbo].[FindArchivialFilesToRestoreBySource]
(
	@SourceID				INT,
	@SourceType				INT,
	@LimitResults			INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @SourceID IS NULL
	BEGIN
		;THROW 50000, 'SourceID must be populated.', 0
	END

	IF @SourceType IS NULL
	BEGIN
		;THROW 50000, 'SourceType must be populated.', 0
	END

	IF @LimitResults IS NULL
	BEGIN
		;THROW 50000, 'LimitResults must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		IF (@LimitResults > 0)
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
			AND		[dbo].[BackupFiles].[SourceID] = @SourceID
			AND		[dbo].[BackupFiles].[SourceType] = @SourceType
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
			AND		[dbo].[BackupFiles].[SourceID] = @SourceID
			AND		[dbo].[BackupFiles].[SourceType] = @SourceType
		END

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END