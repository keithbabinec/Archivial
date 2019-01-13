CREATE PROCEDURE [dbo].[SetApplicationOption]
(
	@Name			NVARCHAR(2048),
	@Value			NVARCHAR(MAX)
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

	IF @Value IS NULL
	BEGIN
		;THROW 50000, 'Value must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = @Name)
		BEGIN
			INSERT INTO [dbo].[ApplicationOptions]
			(
				[Name],
				[Value]
			)
			VALUES
			(
				@Name,
				@Value
			)
		END
		ELSE
		BEGIN
			UPDATE	[dbo].[ApplicationOptions]
			SET		[dbo].[ApplicationOptions].[Value] = @Value
			WHERE	[dbo].[ApplicationOptions].[Name] = @Name
		END

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