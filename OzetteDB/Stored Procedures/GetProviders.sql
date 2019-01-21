CREATE PROCEDURE [dbo].[GetProviders]
(
	@Type			INT
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @Type IS NULL
	BEGIN
		;THROW 50000, 'Type must be populated.', 0
	END
	IF @Type < 0 OR @Type > 2
	BEGIN
		;THROW 50000, 'Type value must be within range (0-2).', 0
	END

	-- transaction
	
	BEGIN TRY
		
		IF @Type = 0
		BEGIN
			-- any type (dont use a filter)
			SELECT	[dbo].[Providers].[ID],
					[dbo].[Providers].[Name],
					[dbo].[Providers].[Type]
			FROM	[dbo].[Providers]
		END
		ELSE
		BEGIN
			-- a specific type
			SELECT	[dbo].[Providers].[ID],
					[dbo].[Providers].[Name],
					[dbo].[Providers].[Type]
			FROM	[dbo].[Providers]
			WHERE	[dbo].[Providers].[Type] = @Type
		END

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END