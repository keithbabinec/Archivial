CREATE PROCEDURE [dbo].[SetLocalSourceLocation]
(
	@Path					NVARCHAR(1024),
	@FileMatchFilter		NVARCHAR(256),
	@Priority				INT,
	@RevisionCount			INT,
	@LastCompletedScan		DATETIME
)
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- param validation

	IF @Path IS NULL
	BEGIN
		;THROW 50000, 'Path must be populated.', 0
	END

	IF @FileMatchFilter IS NULL
	BEGIN
		;THROW 50000, 'FileMatchFilter must be populated.', 0
	END

	IF @Priority IS NULL
	BEGIN
		;THROW 50000, 'Priority must be populated.', 0
	END

	IF @RevisionCount IS NULL
	BEGIN
		;THROW 50000, 'RevisionCount must be populated.', 0
	END

	IF @LastCompletedScan IS NULL
	BEGIN
		;THROW 50000, 'LastCompletedScan must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM [dbo].[LocalSourceLocations] WHERE [Path] = @Path)
		BEGIN
			INSERT INTO [dbo].[LocalSourceLocations]
			(
				[Path],
				[FileMatchFilter],
				[Priority],
				[RevisionCount],
				[LastCompletedScan]
			)
			VALUES
			(
				@Path,
				@FileMatchFilter,
				@Priority,
				@RevisionCount,
				@LastCompletedScan
			)
		END
		ELSE
		BEGIN
			UPDATE	[dbo].[LocalSourceLocations]
			SET		[dbo].[LocalSourceLocations].[Path] = @Path,
					[dbo].[LocalSourceLocations].[FileMatchFilter] = @FileMatchFilter,
					[dbo].[LocalSourceLocations].[Priority] = @Priority,
					[dbo].[LocalSourceLocations].[RevisionCount] = @RevisionCount,
					[dbo].[LocalSourceLocations].[LastCompletedScan] = @LastCompletedScan
			WHERE	[dbo].[LocalSourceLocations].[Path] = @Path
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