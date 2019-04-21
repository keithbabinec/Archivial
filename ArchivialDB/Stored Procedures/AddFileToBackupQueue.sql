CREATE PROCEDURE [dbo].[AddFileToBackupQueue]
(
	@ID							UNIQUEIDENTIFIER,
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
			IF NOT EXISTS (SELECT 1 FROM [dbo].[LowPriBackupFilesQueue] WHERE [FileID] = @ID)
			BEGIN
				INSERT INTO [dbo].[LowPriBackupFilesQueue] ( [FileID], [AssignedInstanceID] ) VALUES ( @ID, -1 )
			END
		END
		ELSE IF @Priority = 2
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM [dbo].[MedPriBackupFilesQueue] WHERE [FileID] = @ID)
			BEGIN
				INSERT INTO [dbo].[MedPriBackupFilesQueue] ( [FileID], [AssignedInstanceID] ) VALUES ( @ID, -1 )
			END
		END
		ELSE IF @Priority = 3
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM [dbo].[HighPriBackupFilesQueue] WHERE [FileID] = @ID)
			BEGIN
				INSERT INTO [dbo].[HighPriBackupFilesQueue] ( [FileID], [AssignedInstanceID] ) VALUES ( @ID, -1 )
			END
		END
		ELSE IF @Priority = 4
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM [dbo].[MetaFilesQueue] WHERE [FileID] = @ID)
			BEGIN
				INSERT INTO [dbo].[MetaFilesQueue] ( [FileID], [AssignedInstanceID] ) VALUES ( @ID, -1 )
			END
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