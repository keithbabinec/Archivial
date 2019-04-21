CREATE PROCEDURE [dbo].[GetSourceLocation]
(
	@SourceID				INT,
	@SourceType				INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- parameter checks

	IF @SourceID IS NULL
	BEGIN
		;THROW 50000, 'SourceID must be populated.', 0
	END
	IF @SourceType IS NULL
	BEGIN
		;THROW 50000, 'SourceType must be populated.', 0
	END
	IF @SourceType < 0 OR @SourceType > 1
	BEGIN
		;THROW 50000, 'SourceType must be within range (0-1).', 0
	END

	BEGIN TRY
		
		-- grab source by ID and type.

		IF @SourceType = 0
		BEGIN
			SELECT	[dbo].[LocalSourceLocations].[ID],
					[dbo].[LocalSourceLocations].[Path],
					[dbo].[LocalSourceLocations].[FileMatchFilter],
					[dbo].[LocalSourceLocations].[Priority],
					[dbo].[LocalSourceLocations].[RevisionCount],
					[dbo].[LocalSourceLocations].[LastCompletedScan],
					[dbo].[LocalSourceLocations].[DestinationContainerName]
			FROM	[dbo].[LocalSourceLocations]
			WHERE	[dbo].[LocalSourceLocations].[ID] = @SourceID
		END
		ELSE IF @SourceType = 1
		BEGIN
			SELECT	[dbo].[NetworkSourceLocations].[ID],
					[dbo].[NetworkSourceLocations].[Path],
					[dbo].[NetworkSourceLocations].[FileMatchFilter],
					[dbo].[NetworkSourceLocations].[Priority],
					[dbo].[NetworkSourceLocations].[RevisionCount],
					[dbo].[NetworkSourceLocations].[LastCompletedScan],
					[dbo].[NetworkSourceLocations].[CredentialName],
					[dbo].[NetworkSourceLocations].[IsConnected],
					[dbo].[NetworkSourceLocations].[IsFailed],
					[dbo].[NetworkSourceLocations].[LastConnectionCheck],
					[dbo].[NetworkSourceLocations].[DestinationContainerName]
			FROM	[dbo].[NetworkSourceLocations]
			WHERE	[dbo].[NetworkSourceLocations].[ID] = @SourceID
		END

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END