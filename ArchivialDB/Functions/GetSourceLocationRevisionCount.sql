CREATE FUNCTION [dbo].[GetSourceLocationRevisionCount]
(
	@SourceID		INT,
	@SourceType		INT
)
RETURNS INT
AS
BEGIN
	
	DECLARE @RevisionCount INT

	-- parameter checks

	IF @SourceID IS NULL OR @SourceType IS NULL
	BEGIN
		SET @RevisionCount = -1
	END
	ELSE
	BEGIN
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
	END

	IF @RevisionCount IS NULL
	BEGIN
		SET @RevisionCount = -1
	END

	RETURN @RevisionCount

END