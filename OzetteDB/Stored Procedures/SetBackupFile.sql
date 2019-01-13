CREATE PROCEDURE [dbo].[SetBackupFile]
(
	@ID							UNIQUEIDENTIFIER,
	@FileName					NVARCHAR(MAX),
	@Directory					NVARCHAR(MAX),
	@FullSourcePath				NVARCHAR(MAX),
	@FileSizeBytes				BIGINT,
	@LastModified				DATETIME,
	@TotalFileBlocks			INT,
	@FileHash					VARBINARY(4096),
	@FileHashString				NVARCHAR(4096),
	@Priority					INT,
	@FileRevisionNumber			INT,
	@HashAlgorithmType			NVARCHAR(1024),
	@LastChecked				DATETIME,
	@LastUpdated				DATETIME,
	@OverallState				INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @ID IS NULL
	BEGIN
		;THROW 50000, 'ID must be populated.', 0
	END

	IF @FileName IS NULL
	BEGIN
		;THROW 50000, 'FileName must be populated.', 0
	END

	IF @Directory IS NULL
	BEGIN
		;THROW 50000, 'Directory must be populated.', 0
	END

	IF @FullSourcePath IS NULL
	BEGIN
		;THROW 50000, 'FullSourcePath must be populated.', 0
	END

	IF @FileSizeBytes IS NULL
	BEGIN
		;THROW 50000, 'FileSizeBytes must be populated.', 0
	END

	IF @LastModified IS NULL
	BEGIN
		;THROW 50000, 'LastModified must be populated.', 0
	END

	IF @TotalFileBlocks IS NULL
	BEGIN
		;THROW 50000, 'TotalFileBlocks must be populated.', 0
	END

	IF @FileHash IS NULL
	BEGIN
		;THROW 50000, 'FileHash must be populated.', 0
	END

	IF @FileHashString IS NULL
	BEGIN
		;THROW 50000, 'FileHashString must be populated.', 0
	END

	IF @Priority IS NULL
	BEGIN
		;THROW 50000, 'Priority must be populated.', 0
	END

	IF @FileRevisionNumber IS NULL
	BEGIN
		;THROW 50000, 'FileRevisionNumber must be populated.', 0
	END

	IF @HashAlgorithmType IS NULL
	BEGIN
		;THROW 50000, 'HashAlgorithmType must be populated.', 0
	END

	IF @LastChecked IS NULL
	BEGIN
		;THROW 50000, 'LastChecked must be populated.', 0
	END

	IF @LastUpdated IS NULL
	BEGIN
		;THROW 50000, 'LastUpdated must be populated.', 0
	END

	IF @OverallState IS NULL
	BEGIN
		;THROW 50000, 'OverallState must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM [dbo].[BackupFiles] WHERE [ID] = @ID)
		BEGIN
			INSERT INTO [dbo].[BackupFiles]
			(
				[ID],
				[FileName],
				[Directory],
				[FullSourcePath],
				[FileSizeBytes],
				[LastModified],
				[TotalFileBlocks],
				[FileHash],
				[FileHashString],
				[Priority],
				[FileRevisionNumber],
				[HashAlgorithmType],
				[LastChecked],
				[LastUpdated],
				[OverallState]
			)
			VALUES
			(
				@ID,
				@FileName,
				@Directory,
				@FullSourcePath,
				@FileSizeBytes,
				@LastModified,
				@TotalFileBlocks,
				@FileHash,
				@FileHashString,
				@Priority,
				@FileRevisionNumber,
				@HashAlgorithmType,
				@LastChecked,
				@LastUpdated,
				@OverallState
			)
		END
		ELSE
		BEGIN
			UPDATE	[dbo].[BackupFiles]
			SET		[dbo].[BackupFiles].[FileName] = @FileName,
					[dbo].[BackupFiles].[Directory] = @Directory,
					[dbo].[BackupFiles].[FullSourcePath] = @FullSourcePath,
					[dbo].[BackupFiles].[FileSizeBytes] = @FileSizeBytes,
					[dbo].[BackupFiles].[LastModified] = @LastModified,
					[dbo].[BackupFiles].[TotalFileBlocks] = @TotalFileBlocks,
					[dbo].[BackupFiles].[FileHash] = @FileHash,
					[dbo].[BackupFiles].[FileHashString] = @FileHashString,
					[dbo].[BackupFiles].[Priority] = @Priority,
					[dbo].[BackupFiles].[FileRevisionNumber] = @FileRevisionNumber,
					[dbo].[BackupFiles].[HashAlgorithmType] = @HashAlgorithmType,
					[dbo].[BackupFiles].[LastChecked] = @LastChecked,
					[dbo].[BackupFiles].[LastUpdated] = @LastUpdated,
					[dbo].[BackupFiles].[OverallState] = @OverallState
			WHERE	[dbo].[BackupFiles].[ID] = @ID
		END

		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH

		IF(@@TRANCOUNT > 0)
		BEGIN
			ROLLBACK TRAN
		END

		;THROW

	END CATCH

	RETURN 0

END