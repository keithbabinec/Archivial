CREATE PROCEDURE [dbo].[AddBackupFile]
(
	@ID							UNIQUEIDENTIFIER,
	@FileName					NVARCHAR(512),
	@Directory					NVARCHAR(1024),
	@FullSourcePath				NVARCHAR(1024),
	@FileSizeBytes				BIGINT,
	@LastModified				DATETIME,
	@TotalFileBlocks			INT,
	@Priority					INT
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

	IF @Priority IS NULL
	BEGIN
		;THROW 50000, 'Priority must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

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
			NULL,
			NULL,
			@Priority,
			1,
			NULL,
			GETDATE(),
			GETDATE(),
			0 -- unsynced
		)

		-- new file- add it to the backup queue.

		INSERT INTO [dbo].[BackupQueue]
		(
			[FileID],
			[AssignedInstanceID]
		)
		VALUES
		(
			@ID,
			-1 -- unassigned.
		)

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