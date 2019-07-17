CREATE PROCEDURE [dbo].[ConfigureFullTextSearchIndexesIfRequired]
AS
BEGIN

	SET ARITHABORT, NOCOUNT, XACT_ABORT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	BEGIN TRY

	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id('dbo.BackupFiles'))
	BEGIN
		CREATE FULLTEXT INDEX ON [dbo].[BackupFiles]( [FileName], [Directory], [FullSourcePath], [FileHashString] ) KEY INDEX PK_BackupFiles_ID WITH CHANGE_TRACKING AUTO
	END

	END TRY
	BEGIN CATCH

		;THROW

	END CATCH

	RETURN 0

END