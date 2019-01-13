CREATE PROCEDURE [dbo].[GetSourceLocations]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- transaction
	
	BEGIN TRY
		
		-- grab local sources

		SELECT	[dbo].[LocalSourceLocations].[ID],
				[dbo].[LocalSourceLocations].[Path],
				[dbo].[LocalSourceLocations].[FileMatchFilter],
				[dbo].[LocalSourceLocations].[Priority],
				[dbo].[LocalSourceLocations].[RevisionCount],
				[dbo].[LocalSourceLocations].[LastCompletedScan]
		FROM	[dbo].[LocalSourceLocations]

		-- grab network sources

		SELECT	[dbo].[NetworkSourceLocations].[ID],
				[dbo].[NetworkSourceLocations].[Path],
				[dbo].[NetworkSourceLocations].[FileMatchFilter],
				[dbo].[NetworkSourceLocations].[Priority],
				[dbo].[NetworkSourceLocations].[RevisionCount],
				[dbo].[NetworkSourceLocations].[LastCompletedScan],
				[dbo].[NetworkSourceLocations].[CredentialName],
				[dbo].[NetworkSourceLocations].[IsConnected],
				[dbo].[NetworkSourceLocations].[IsFailed],
				[dbo].[NetworkSourceLocations].[LastConnectionCheck]
		FROM	[dbo].[NetworkSourceLocations]

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END