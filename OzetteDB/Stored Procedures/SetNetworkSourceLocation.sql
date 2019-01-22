CREATE PROCEDURE [dbo].[SetNetworkSourceLocation]
(
	@Path					NVARCHAR(1024),
	@FileMatchFilter		NVARCHAR(256),
	@Priority				INT,
	@RevisionCount			INT,
	@LastCompletedScan		DATETIME NULL,
	@CredentialName			NVARCHAR(256),
	@IsConnected			BIT,
	@IsFailed				BIT,
	@LastConnectionCheck	DATETIME NULL
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

	IF @CredentialName IS NULL
	BEGIN
		;THROW 50000, 'CredentialName must be populated.', 0
	END

	IF @IsConnected IS NULL
	BEGIN
		;THROW 50000, 'IsConnected must be populated.', 0
	END

	IF @IsFailed IS NULL
	BEGIN
		;THROW 50000, 'IsFailed must be populated.', 0
	END

	-- transaction
	
	BEGIN TRY
		
		BEGIN TRANSACTION

		IF NOT EXISTS (SELECT 1 FROM [dbo].[NetworkSourceLocations] WHERE [Path] = @Path)
		BEGIN
			INSERT INTO [dbo].[NetworkSourceLocations]
			(
				[Path],
				[FileMatchFilter],
				[Priority],
				[RevisionCount],
				[LastCompletedScan],
				[CredentialName],
				[IsConnected],
				[IsFailed],
				[LastConnectionCheck]
			)
			VALUES
			(
				@Path,
				@FileMatchFilter,
				@Priority,
				@RevisionCount,
				@LastCompletedScan,
				@CredentialName,
				@IsConnected,
				@IsFailed,
				@LastConnectionCheck
			)
		END
		ELSE
		BEGIN
			UPDATE	[dbo].[NetworkSourceLocations]
			SET		[dbo].[NetworkSourceLocations].[Path] = @Path,
					[dbo].[NetworkSourceLocations].[FileMatchFilter] = @FileMatchFilter,
					[dbo].[NetworkSourceLocations].[Priority] = @Priority,
					[dbo].[NetworkSourceLocations].[RevisionCount] = @RevisionCount,
					[dbo].[NetworkSourceLocations].[LastCompletedScan] = @LastCompletedScan,
					[dbo].[NetworkSourceLocations].[CredentialName] = @CredentialName,
					[dbo].[NetworkSourceLocations].[IsConnected] = @IsConnected,
					[dbo].[NetworkSourceLocations].[IsFailed] = @IsFailed,
					[dbo].[NetworkSourceLocations].[LastConnectionCheck] = @LastConnectionCheck
			WHERE	[dbo].[NetworkSourceLocations].[Path] = @Path
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