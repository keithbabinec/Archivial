﻿CREATE PROCEDURE [dbo].[GetBackupProgress]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	-- some constants to make the queries more readable

	DECLARE @UnsyncedConstant INT = 0
	DECLARE @OutOfDateConstant INT = 1
	DECLARE @InProgressConstant INT = 2
	DECLARE @SyncedConstant INT = 3
	DECLARE @ProviderErrorConstant INT = 4

	-- transaction
	
	BEGIN TRY
		
		-- total files

		DECLARE @TotalFileCount BIGINT
		DECLARE @TotalFileSizeBytes BIGINT

		SELECT	@TotalFileSizeBytes = ISNULL(SUM([dbo].[BackupFiles].[FileSizeBytes]),0),
				@TotalFileCount = COUNT(*)
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[WasDeleted] IS NULL

		-- synced files

		DECLARE @BackedUpFileCount BIGINT
		DECLARE @BackedUpFileSizeBytes BIGINT

		SELECT	@BackedUpFileSizeBytes = ISNULL(SUM([dbo].[BackupFiles].[FileSizeBytes]),0),
				@BackedUpFileCount = COUNT(*)
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[OverallState] = @SyncedConstant
		AND		[dbo].[BackupFiles].[WasDeleted] IS NULL

		-- in-progress files

		DECLARE @RemainingFileCount BIGINT
		DECLARE @RemainingFileSizeBytes BIGINT

		SELECT	@RemainingFileSizeBytes = ISNULL(SUM([dbo].[BackupFiles].[FileSizeBytes]),0),
				@RemainingFileCount = COUNT(*)
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[OverallState] IN
				(
					@UnsyncedConstant,
					@OutOfDateConstant,
					@InProgressConstant
				)
		AND		[dbo].[BackupFiles].[WasDeleted] IS NULL

		-- failed files

		DECLARE @FailedFileCount BIGINT
		DECLARE @FailedFileSizeBytes BIGINT

		SELECT	@FailedFileSizeBytes = ISNULL(SUM([dbo].[BackupFiles].[FileSizeBytes]),0),
				@FailedFileCount = COUNT(*)
		FROM	[dbo].[BackupFiles]
		WHERE	[dbo].[BackupFiles].[OverallState] = @ProviderErrorConstant
		AND		[dbo].[BackupFiles].[WasDeleted] IS NULL

		-- return results

		SELECT	@TotalFileCount AS [TotalFileCount],
				@TotalFileSizeBytes AS [TotalFileSizeBytes],
				@BackedUpFileCount AS [BackedUpFileCount],
				@BackedUpFileSizeBytes AS [BackedUpFileSizeBytes],
				@RemainingFileCount AS [RemainingFileCount],
				@RemainingFileSizeBytes AS [RemainingFileSizeBytes],
				@FailedFileCount AS [FailedFileCount],
				@FailedFileSizeBytes AS [FailedFileSizeBytes]

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END