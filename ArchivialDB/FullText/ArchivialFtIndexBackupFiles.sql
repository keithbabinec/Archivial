CREATE FULLTEXT INDEX 
	ON [dbo].[BackupFiles]
		( [FileName], [Directory], [FullSourcePath], [FileHashString] ) 
	KEY INDEX PK_BackupFiles_ID 
	ON [ArchivialFtsCatalog]
	WITH CHANGE_TRACKING AUTO