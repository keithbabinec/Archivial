CREATE FUNCTION [dbo].[GetSourceLocationRevisionCount]
(
	@SourceID		INT,
	@SourceType		INT
)
RETURNS INT
AS
BEGIN
	
	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @RevisionCount INT

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

	-- grab source by ID and type.

	IF @SourceType = 0
	BEGIN
		SELECT	@RevisionCount = [dbo].[LocalSourceLocations].[RevisionCount]
		FROM	[dbo].[LocalSourceLocations]
		WHERE	[dbo].[LocalSourceLocations].[ID] = @SourceID
	END
	ELSE IF @SourceType = 1
	BEGIN
		SELECT	@RevisionCount = [dbo].[NetworkSourceLocations].[RevisionCount]
		FROM	[dbo].[NetworkSourceLocations]
		WHERE	[dbo].[NetworkSourceLocations].[ID] = @SourceID
	END

	IF @RevisionCount IS NULL
	BEGIN
		;THROW 50000, 'Specified source was not found.', 0
	END

	RETURN @RevisionCount

END