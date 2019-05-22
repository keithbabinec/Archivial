CREATE PROCEDURE [dbo].[ResetBackupFile]
(
	@ID							UNIQUEIDENTIFIER,
	@FileSizeBytes				BIGINT,
	@LastModified				DATETIME,
	@TotalFileBlocks			INT
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

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		DECLARE @PreviousRevision INT
		DECLARE @SourceType INT
		DECLARE @SourceID INT

		SELECT	@PreviousRevision = [dbo].[BackupFiles].[FileRevisionNumber],
				@SourceType = [dbo].[BackupFiles].[SourceType],
				@SourceID = [dbo].[BackupFiles].[SourceID]
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[ID] = @ID

		IF @PreviousRevision IS NULL
		BEGIN
			;THROW 50000, 'File was not found in the BackupFiles table.', 0
		END

		DECLARE @MaxRevisionCount INT = [dbo].[GetSourceLocationRevisionCount](@SourceID, @SourceType)

		UPDATE	[dbo].[BackupFiles]
		SET		[dbo].[BackupFiles].[FileHash] = NULL,
				[dbo].[BackupFiles].[FileHashString] = NULL,
				[dbo].[BackupFiles].[HashAlgorithmType] = NULL,
				[dbo].[BackupFiles].[LastChecked] = GETDATE(),
				[dbo].[BackupFiles].[LastUpdated] = GETDATE(),
				[dbo].[BackupFiles].[LastModified] = @LastModified,
				[dbo].[BackupFiles].[FileSizeBytes] = @FileSizeBytes,
				[dbo].[BackupFiles].[TotalFileBlocks] = @TotalFileBlocks,
				[dbo].[BackupFiles].[FileRevisionNumber] = (@PreviousRevision + 1),
				[dbo].[BackupFiles].[WasDeleted] = NULL,
				[dbo].[BackupFiles].[OverallState] = 0, -- unsynced
				[dbo].[BackupFiles].[StateMessage] = NULL
		WHERE	[dbo].[BackupFiles].[ID] = @ID

		DECLARE @RevisionToRemove INT = @PreviousRevision - @MaxRevisionCount

		IF (@RevisionToRemove > 0)
		BEGIN
			INSERT INTO [dbo].[FileCleanupQueue] ( [FileID], [FileRevisionNumber], [AssignedInstanceID] ) VALUES ( @ID, @RevisionToRemove, -1 )
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