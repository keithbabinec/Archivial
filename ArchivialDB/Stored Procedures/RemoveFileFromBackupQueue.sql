CREATE PROCEDURE [dbo].[RemoveFileFromBackupQueue]
(
	@ID				UNIQUEIDENTIFIER,
	@Priority		INT
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
	IF @Priority IS NULL
	BEGIN
		;THROW 50000, 'Priority must be populated.', 0
	END
	IF @Priority < 1 OR @Priority > 4
	BEGIN
		;THROW 50000, 'Priority must be within range (1-4).', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF @Priority = 1
		BEGIN
			DELETE FROM	[dbo].[LowPriBackupFilesQueue]
			WHERE [dbo].[LowPriBackupFilesQueue].[FileID] = @ID
		END
		ELSE IF @Priority = 2
		BEGIN
			DELETE FROM	[dbo].[MedPriBackupFilesQueue]
			WHERE [dbo].[MedPriBackupFilesQueue].[FileID] = @ID
		END
		ELSE IF @Priority = 3
		BEGIN
			DELETE FROM	[dbo].[HighPriBackupFilesQueue]
			WHERE [dbo].[HighPriBackupFilesQueue].[FileID] = @ID
		END
		ELSE IF @Priority = 4
		BEGIN
			DELETE FROM	[dbo].[MetaFilesQueue]
			WHERE [dbo].[MetaFilesQueue].[FileID] = @ID
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