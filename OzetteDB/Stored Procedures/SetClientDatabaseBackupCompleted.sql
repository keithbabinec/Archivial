CREATE PROCEDURE [dbo].[SetClientDatabaseBackupCompleted]
(
	@DatabaseBackupType		INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @DatabaseBackupType IS NULL
	BEGIN
		;THROW 50000, 'DatabaseBackupType must be populated.', 0
	END
	IF @DatabaseBackupType < 1 OR @DatabaseBackupType > 3
	BEGIN
		;THROW 50000, 'DatabaseBackupType value must be within range (1-3).', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF @DatabaseBackupType = 1
		BEGIN
			UPDATE	[dbo].[ClientDatabaseBackupStatus]
			SET		[dbo].[ClientDatabaseBackupStatus].[LastFullBackup] = GETDATE()
		END
		ELSE IF @DatabaseBackupType = 2
		BEGIN
			UPDATE	[dbo].[ClientDatabaseBackupStatus]
			SET		[dbo].[ClientDatabaseBackupStatus].[LastDifferentialBackup] = GETDATE()
		END
		ELSE IF @DatabaseBackupType = 3
		BEGIN
			UPDATE	[dbo].[ClientDatabaseBackupStatus]
			SET		[dbo].[ClientDatabaseBackupStatus].[LastTransactionLogBackup] = GETDATE()
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