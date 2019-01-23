CREATE PROCEDURE [dbo].[AddFileToBackupQueue]
(
	@ID							UNIQUEIDENTIFIER
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

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		INSERT INTO [dbo].[BackupQueue]
		(
			[FileID],
			[AssignedInstanceID]
		)
		VALUES
		(
			@ID,
			-1 -- unassigned.
		)

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