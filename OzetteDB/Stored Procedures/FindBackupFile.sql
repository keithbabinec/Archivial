CREATE PROCEDURE [dbo].[FindBackupFile]
(
	@FullFilePath			NVARCHAR(1024),
	@FileSizeBytes			BIGINT,
	@FileLastModified		DATETIME
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @FullFilePath IS NULL
	BEGIN
		;THROW 50000, 'FullFilePath must be populated.', 0
	END

	IF @FileSizeBytes IS NULL
	BEGIN
		;THROW 50000, 'FileSizeBytes must be populated.', 0
	END

	IF @FileLastModified IS NULL
	BEGIN
		;THROW 50000, 'FileLastModified must be populated.', 0
	END

	-- some constants to make the queries more readable

	DECLARE @BackupFileLookupResultNew INT = 0
	DECLARE @BackupFileLookupResultExisting INT = 1
	DECLARE @BackupFileLookupResultUpdated INT = 2

	-- transaction
	
	BEGIN TRY
		
		-- does this file already exist in the exact constraints?
		-- if yes, this is an existing-unchanged file.

		DECLARE @FileID UNIQUEIDENTIFIER

		SELECT		@FileID = [dbo].[BackupFiles].[ID]
		FROM		[dbo].[BackupFiles]
		WHERE		[dbo].[BackupFiles].[FullSourcePath] = @FullFilePath
		AND			[dbo].[BackupFiles].[FileSizeBytes] = @FileSizeBytes
		AND			[dbo].[BackupFiles].[LastModified] = @FileLastModified

		IF @FileID IS NOT NULL
		BEGIN
			-- File exists and is unchanged.

			SELECT @BackupFileLookupResultExisting AS [Result]

			SELECT	[dbo].[BackupFiles].[ID],
					[dbo].[BackupFiles].[FileName],
					[dbo].[BackupFiles].[Directory],
					[dbo].[BackupFiles].[FullSourcePath],
					[dbo].[BackupFiles].[FileSizeBytes],
					[dbo].[BackupFiles].[LastModified],
					[dbo].[BackupFiles].[TotalFileBlocks],
					[dbo].[BackupFiles].[FileHash],
					[dbo].[BackupFiles].[FileHashString],
					[dbo].[BackupFiles].[Priority],
					[dbo].[BackupFiles].[FileRevisionNumber],
					[dbo].[BackupFiles].[HashAlgorithmType],
					[dbo].[BackupFiles].[LastChecked],
					[dbo].[BackupFiles].[LastUpdated],
					[dbo].[BackupFiles].[OverallState]
			FROM	[dbo].[BackupFiles]
			WHERE	[dbo].[BackupFiles].[ID] = @FileID

			RETURN 0

		END

		-- does this file already exist in the under modified constraints?
		-- if yes, this is an existing-updated file.

		SELECT		@FileID = [dbo].[BackupFiles].[ID]
		FROM		[dbo].[BackupFiles]
		WHERE		[dbo].[BackupFiles].[FullSourcePath] = @FullFilePath
		AND
		(
					[dbo].[BackupFiles].[FileSizeBytes] != @FileSizeBytes
			OR		[dbo].[BackupFiles].[LastModified] != @FileLastModified
		)

		IF @FileID IS NOT NULL
		BEGIN
			-- File exists and is modified.

			SELECT @BackupFileLookupResultUpdated AS [Result]

			SELECT	[dbo].[BackupFiles].[ID],
					[dbo].[BackupFiles].[FileName],
					[dbo].[BackupFiles].[Directory],
					[dbo].[BackupFiles].[FullSourcePath],
					[dbo].[BackupFiles].[FileSizeBytes],
					[dbo].[BackupFiles].[LastModified],
					[dbo].[BackupFiles].[TotalFileBlocks],
					[dbo].[BackupFiles].[FileHash],
					[dbo].[BackupFiles].[FileHashString],
					[dbo].[BackupFiles].[Priority],
					[dbo].[BackupFiles].[FileRevisionNumber],
					[dbo].[BackupFiles].[HashAlgorithmType],
					[dbo].[BackupFiles].[LastChecked],
					[dbo].[BackupFiles].[LastUpdated],
					[dbo].[BackupFiles].[OverallState]
			FROM	[dbo].[BackupFiles]
			WHERE	[dbo].[BackupFiles].[ID] = @FileID

			RETURN 0

		END

		-- made it this far with no result? 
		-- this means the file is new (doesn't exist in our database).

		SELECT @BackupFileLookupResultNew AS [Result]

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END