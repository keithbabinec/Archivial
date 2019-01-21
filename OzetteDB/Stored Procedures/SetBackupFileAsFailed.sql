CREATE PROCEDURE [dbo].[SetBackupFileAsFailed]
(
	@ID						UNIQUEIDENTIFIER,
	@Message				NVARCHAR(MAX)
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

	IF @Message IS NULL
	BEGIN
		;THROW 50000, 'Message must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE	[dbo].[BackupFiles]
		SET		[dbo].[BackupFiles].[OverallState] = 4, -- failed
				[dbo].[BackupFiles].[StateMessage] = @Message
		WHERE	[dbo].[BackupFiles].[ID] = @ID

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