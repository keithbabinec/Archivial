CREATE PROCEDURE [dbo].[FindNextFileToBackup]
(
	@EngineInstanceID		INT,
	@Priority				INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;

	-- param checks

	IF @EngineInstanceID IS NULL
	BEGIN
		;THROW 50000, 'EngineInstanceID must be populated.', 0
	END
	IF @Priority IS NULL
	BEGIN
		;THROW 50000, 'Priority must be populated.', 0
	END
	IF @Priority < 1 OR @Priority > 4
	BEGIN
		;THROW 50000, 'Priority must be within range (1-4).', 0
	END

	-- some constants to make the queries more readable

	DECLARE @UnassignedInstance INT = -1

	-- transaction
	
	BEGIN TRY
		
		DECLARE @NextFileID UNIQUEIDENTIFIER
		DECLARE @NextOrderID INT

		BEGIN TRANSACTION

		-- grab next file to backup
		-- update its assigned instance ID (so it won't be grabbed by another instance).

		IF @Priority = 1
		BEGIN
			SELECT		TOP 1 @NextOrderID = [dbo].[LowPriBackupFilesQueue].[OrderID]
			FROM		[dbo].[LowPriBackupFilesQueue] WITH (UPDLOCK)
			WHERE		[dbo].[LowPriBackupFilesQueue].[AssignedInstanceID] = @UnassignedInstance
			OR			[dbo].[LowPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID
			ORDER BY	[dbo].[LowPriBackupFilesQueue].[OrderID]

			UPDATE		[dbo].[LowPriBackupFilesQueue] WITH (ROWLOCK)
			SET			[dbo].[LowPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID,
						@NextFileID = [dbo].[LowPriBackupFilesQueue].[FileID]
			WHERE		[dbo].[LowPriBackupFilesQueue].[OrderID] = @NextOrderID
		END
		ELSE IF @Priority = 2
		BEGIN
		SELECT		TOP 1 @NextOrderID = [dbo].[MedPriBackupFilesQueue].[OrderID]
			FROM		[dbo].[MedPriBackupFilesQueue] WITH (UPDLOCK)
			WHERE		[dbo].[MedPriBackupFilesQueue].[AssignedInstanceID] = @UnassignedInstance
			OR			[dbo].[MedPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID
			ORDER BY	[dbo].[MedPriBackupFilesQueue].[OrderID]

			UPDATE		[dbo].[MedPriBackupFilesQueue] WITH (ROWLOCK)
			SET			[dbo].[MedPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID,
						@NextFileID = [dbo].[MedPriBackupFilesQueue].[FileID]
			WHERE		[dbo].[MedPriBackupFilesQueue].[OrderID] = @NextOrderID
		END
		ELSE IF @Priority = 3
		BEGIN
			SELECT		TOP 1 @NextOrderID = [dbo].[HighPriBackupFilesQueue].[OrderID]
			FROM		[dbo].[HighPriBackupFilesQueue] WITH (UPDLOCK)
			WHERE		[dbo].[HighPriBackupFilesQueue].[AssignedInstanceID] = @UnassignedInstance
			OR			[dbo].[HighPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID
			ORDER BY	[dbo].[HighPriBackupFilesQueue].[OrderID]

			UPDATE		[dbo].[HighPriBackupFilesQueue] WITH (ROWLOCK)
			SET			[dbo].[HighPriBackupFilesQueue].[AssignedInstanceID] = @EngineInstanceID,
						@NextFileID = [dbo].[HighPriBackupFilesQueue].[FileID]
			WHERE		[dbo].[HighPriBackupFilesQueue].[OrderID] = @NextOrderID
		END
		ELSE IF @Priority = 4
		BEGIN
			SELECT		TOP 1 @NextOrderID = [dbo].[MetaFilesQueue].[OrderID]
			FROM		[dbo].[MetaFilesQueue] WITH (UPDLOCK)
			WHERE		[dbo].[MetaFilesQueue].[AssignedInstanceID] = @UnassignedInstance
			OR			[dbo].[MetaFilesQueue].[AssignedInstanceID] = @EngineInstanceID
			ORDER BY	[dbo].[MetaFilesQueue].[OrderID]

			UPDATE		[dbo].[MetaFilesQueue] WITH (ROWLOCK)
			SET			[dbo].[MetaFilesQueue].[AssignedInstanceID] = @EngineInstanceID,
						@NextFileID = [dbo].[MetaFilesQueue].[FileID]
			WHERE		[dbo].[MetaFilesQueue].[OrderID] = @NextOrderID
		END

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