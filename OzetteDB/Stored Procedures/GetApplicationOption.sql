CREATE PROCEDURE [dbo].[GetApplicationOption]
(
	@Name			NVARCHAR(2048)
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
		
		SELECT [Value] FROM [dbo].[ApplicationOptions] WHERE [Name] = @Name

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END