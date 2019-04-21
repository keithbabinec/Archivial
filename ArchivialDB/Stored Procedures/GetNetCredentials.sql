CREATE PROCEDURE [dbo].[GetNetCredentials]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- transaction
	
	BEGIN TRY
		
		SELECT	[dbo].[NetCredentials].[ID],
				[dbo].[NetCredentials].[CredentialName]
		FROM	[dbo].[NetCredentials]

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END