CREATE PROCEDURE [dbo].[FindNextFileToCleanup]
(
	@EngineInstanceID		INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;

	-- param checks

	IF @EngineInstanceID IS NULL
	BEGIN
		;THROW 50000, 'EngineInstanceID must be populated.', 0
	END

	-- some constants to make the queries more readable

	DECLARE @UnassignedInstance INT = -1

	-- transaction
	
	BEGIN TRY
		
		DECLARE @NextFileID UNIQUEIDENTIFIER
		DECLARE @NextOrderID INT
		DECLARE @NextFileRevision INT

		BEGIN TRANSACTION

		-- grab next file to cleanup
		-- update its assigned instance ID (so it won't be grabbed by another instance).

		SELECT		TOP 1 @NextOrderID = [dbo].[FileCleanupQueue].[OrderID],
					@NextFileRevision = [dbo].[FileCleanupQueue].[FileRevisionNumber]
		FROM		[dbo].[FileCleanupQueue] WITH (UPDLOCK)
		WHERE		[dbo].[FileCleanupQueue].[AssignedInstanceID] = @UnassignedInstance
		OR			[dbo].[FileCleanupQueue].[AssignedInstanceID] = @EngineInstanceID
		ORDER BY	[dbo].[FileCleanupQueue].[OrderID]

		UPDATE		[dbo].[FileCleanupQueue] WITH (ROWLOCK)
		SET			[dbo].[FileCleanupQueue].[AssignedInstanceID] = @EngineInstanceID,
					@NextFileID = [dbo].[FileCleanupQueue].[FileID]
		WHERE		[dbo].[FileCleanupQueue].[OrderID] = @NextOrderID

		COMMIT TRANSACTION

		-- return the found file and its copystate.

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
				[dbo].[BackupFiles].[SourceID],
				[dbo].[BackupFiles].[SourceType],
				@NextFileRevision,
				[dbo].[BackupFiles].[HashAlgorithmType],
				[dbo].[BackupFiles].[LastChecked],
				[dbo].[BackupFiles].[LastUpdated],
				[dbo].[BackupFiles].[OverallState]
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[ID] = @NextFileID

		SELECT	[dbo].[CopyState].[StorageProvider],
				[dbo].[CopyState].[SyncStatus],
				[dbo].[CopyState].[HydrationStatus],
				[dbo].[CopyState].[LastCompletedFileBlockIndex]
		FROM	[dbo].[CopyState]
		WHERE	[dbo].[CopyState].[FileID] = @NextFileID
		
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