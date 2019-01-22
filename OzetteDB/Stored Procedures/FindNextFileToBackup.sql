CREATE PROCEDURE [dbo].[FindNextFileToBackup]
(
	@EngineInstanceID		INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

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

		BEGIN TRANSACTION

		-- grab next file to backup
		-- update its assigned instance ID (so it won't be grabbed by another instance).

		UPDATE		[dbo].[BackupQueue]
		SET			[dbo].[BackupQueue].[AssignedInstanceID] = @EngineInstanceID,
					@NextFileID = [dbo].[BackupQueue].[FileID]
		WHERE		[dbo].[BackupQueue].[OrderID] =
		(
			SELECT		TOP 1 [dbo].[BackupQueue].[OrderID]
			FROM		[dbo].[BackupQueue]
			WHERE		[dbo].[BackupQueue].[AssignedInstanceID] = @UnassignedInstance
			OR			[dbo].[BackupQueue].[AssignedInstanceID] = @EngineInstanceID
			ORDER BY	[dbo].[BackupQueue].[OrderID]
		)

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
				[dbo].[BackupFiles].[FileRevisionNumber],
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