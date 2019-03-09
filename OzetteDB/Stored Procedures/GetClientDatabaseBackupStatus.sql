CREATE PROCEDURE [dbo].[GetClientDatabaseBackupStatus]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- transaction
	
	BEGIN TRY
		
		SELECT	[dbo].[ClientDatabaseBackupStatus].[LastFullBackup],
				[dbo].[ClientDatabaseBackupStatus].[LastDifferentialBackup],
				[dbo].[ClientDatabaseBackupStatus].[LastTransactionLogBackup]
		FROM	[dbo].[ClientDatabaseBackupStatus]

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END