CREATE PROCEDURE [dbo].[GetDirectoryMapItem]
(
	@DirectoryPath			NVARCHAR(1024)
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @DirectoryPath IS NULL
	BEGIN
		;THROW 50000, 'DirectoryPath must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		DECLARE @DirectoryID UNIQUEIDENTIFIER

		SELECT	@DirectoryID = [dbo].[DirectoryMapItems].[ID] 
		FROM	[dbo].[DirectoryMapItems]
		WHERE	[dbo].[DirectoryMapItems].[LocalPath] = @DirectoryPath

		IF @DirectoryID IS NULL
		BEGIN
			-- no existing directory map exists.
			-- make a new one
			SET @DirectoryID = NEWID()

			INSERT INTO	[dbo].[DirectoryMapItems]
			(
				[ID],
				[LocalPath]
			)
			VALUES
			(
				@DirectoryID,
				@DirectoryPath
			)
		END
		
		SELECT @DirectoryID, @DirectoryPath

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