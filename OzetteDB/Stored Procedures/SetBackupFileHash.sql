CREATE PROCEDURE [dbo].[SetBackupFileHash]
(
	@ID						UNIQUEIDENTIFIER,
	@FileHash				VARBINARY(512),
	@FileHashString			NVARCHAR(512),
	@HashAlgorithm			NVARCHAR(512)
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

	IF @FileHash IS NULL
	BEGIN
		;THROW 50000, 'FileHash must be populated.', 0
	END

	IF @FileHashString IS NULL
	BEGIN
		;THROW 50000, 'FileHashString must be populated.', 0
	END

	IF @HashAlgorithm IS NULL
	BEGIN
		;THROW 50000, 'HashAlgorithm must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE	[dbo].[BackupFiles]
		SET		[dbo].[BackupFiles].[FileHash] = @FileHash,
				[dbo].[BackupFiles].[FileHashString] = @FileHashString,
				[dbo].[BackupFiles].[HashAlgorithmType] = @HashAlgorithm,
				[dbo].[BackupFiles].[LastUpdated] = GETDATE()
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