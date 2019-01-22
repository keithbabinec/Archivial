CREATE PROCEDURE [dbo].[SetCopyState]
(
	@FileID							UNIQUEIDENTIFIER,
	@StorageProvider				INT,
	@SyncStatus						INT,
	@HydrationStatus				INT,
	@LastCompletedFileBlockIndex	INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @FileID IS NULL
	BEGIN
		;THROW 50000, 'FileID must be populated.', 0
	END

	IF @StorageProvider IS NULL
	BEGIN
		;THROW 50000, 'StorageProvider must be populated.', 0
	END

	IF @SyncStatus IS NULL
	BEGIN
		;THROW 50000, 'SyncStatus must be populated.', 0
	END

	IF @HydrationStatus IS NULL
	BEGIN
		;THROW 50000, 'HydrationStatus must be populated.', 0
	END

	IF @LastCompletedFileBlockIndex IS NULL
	BEGIN
		;THROW 50000, 'LastCompletedFileBlockIndex must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM [dbo].[CopyState] WHERE [FileID] = @FileID AND [StorageProvider] = @StorageProvider)
		BEGIN
			INSERT INTO [dbo].[CopyState]
			(
				[FileID],
				[StorageProvider],
				[SyncStatus],
				[HydrationStatus],
				[LastCompletedFileBlockIndex]
			)
			VALUES
			(
				@FileID,
				@StorageProvider,
				@SyncStatus,
				@HydrationStatus,
				@LastCompletedFileBlockIndex
			)
		END
		ELSE
		BEGIN
			UPDATE	[dbo].[CopyState]
			SET		[dbo].[CopyState].[SyncStatus] = @SyncStatus,
					[dbo].[CopyState].[HydrationStatus] = @HydrationStatus,
					[dbo].[CopyState].[LastCompletedFileBlockIndex] = @LastCompletedFileBlockIndex
			WHERE	[dbo].[CopyState].[FileID] = @FileID
			AND		[dbo].[CopyState].[StorageProvider] = @StorageProvider
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