﻿CREATE PROCEDURE [dbo].[RemoveNetCredential]
(
	@Name			NVARCHAR(256)
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @Name IS NULL
	BEGIN
		;THROW 50000, 'Name must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		DELETE FROM	[dbo].[NetCredentials]
		WHERE		[dbo].[NetCredentials].[CredentialName] = @Name

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