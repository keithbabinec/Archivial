CREATE PROCEDURE [dbo].[AddProvider]
(
	@Name			NVARCHAR(256),
	@Type			INT
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

	IF @Type IS NULL
	BEGIN
		;THROW 50000, 'Type must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION
		
		INSERT INTO [dbo].[Providers]
		(
			[Name],
			[Type]
		)
		VALUES
		(
			@Name,
			@Type
		)

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