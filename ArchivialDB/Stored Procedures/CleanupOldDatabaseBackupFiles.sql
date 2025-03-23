CREATE PROCEDURE [dbo].[CleanupOldDatabaseBackupFiles]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;

	-- some constants to make the queries more readable

	DECLARE @UnassignedInstance INT = -1

	-- transaction
	
	BEGIN TRY
		
		DECLARE @NextFileID UNIQUEIDENTIFIER
		DECLARE @FileRevisionNumber INT = 1
		DECLARE @AssignedEngine INT = -1

		BEGIN TRANSACTION

		-- grab next database backup file to cleanup
		-- update its assigned instance ID (so it won't be grabbed by another instance).

		INSERT INTO [dbo].[FileCleanupQueue] ( [FileID], [FileRevisionNumber], [AssignedInstanceID] ) 
		SELECT		[dbo].[BackupFiles].[ID], @FileRevisionNumber, @AssignedEngine
		FROM		[dbo].[BackupFiles]
		WHERE		[dbo].[BackupFiles].[FileName] like 'ArchivialDB%.bak'
		AND			[LastModified] < DATEADD(DAY, -30, GETDATE())

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