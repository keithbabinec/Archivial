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

	-- transaction
	
	BEGIN TRY
		
		SELECT	[dbo].[Providers].[ID],
				[dbo].[Providers].[Name],
				[dbo].[Providers].[Type]
		FROM	[dbo].[Providers] WHERE [Type] = @Type

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END