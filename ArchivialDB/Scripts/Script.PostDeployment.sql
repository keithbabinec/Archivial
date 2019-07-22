/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

RAISERROR ('Starting database post-deployment script.', 0, 1) WITH NOWAIT

-- Add core settings, if missing.

RAISERROR ('Adding core application option defaults, if missing.', 0, 1) WITH NOWAIT

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'LowPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'LowPriorityScanFrequencyInHours', '48' )
	RAISERROR ('LowPriorityScanFrequencyInHours application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'MedPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'MedPriorityScanFrequencyInHours', '12' )
	RAISERROR ('MedPriorityScanFrequencyInHours application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'HighPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'HighPriorityScanFrequencyInHours', '1' )
	RAISERROR ('HighPriorityScanFrequencyInHours application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'MetaPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'MetaPriorityScanFrequencyInHours', '1' )
	RAISERROR ('MetaPriorityScanFrequencyInHours application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'StatusUpdateSchedule'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'StatusUpdateSchedule', '0 8 * * *' )
	RAISERROR ('StatusUpdateSchedule application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'BackupEngineInstancesCount'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'BackupEngineInstancesCount', '4' )
	RAISERROR ('BackupEngineInstancesCount application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'BackupEngineStartupDelayInSeconds'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'BackupEngineStartupDelayInSeconds', '30' )
	RAISERROR ('BackupEngineStartupDelayInSeconds application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'CleanupEngineInstancesCount'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'CleanupEngineInstancesCount', '1' )
	RAISERROR ('CleanupEngineInstancesCount application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'DatabaseBackupsRetentionInDays'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'DatabaseBackupsRetentionInDays', '7' )
	RAISERROR ('DatabaseBackupsRetentionInDays application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'LogFilesRetentionInDays'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'LogFilesRetentionInDays', '30' )
	RAISERROR ('LogFilesRetentionInDays application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'MasterExclusionMatches'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'MasterExclusionMatches', '' )
	RAISERROR ('MasterExclusionMatches application option setting has been added', 0, 1) WITH NOWAIT
END

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ApplicationOptions] WHERE [Name] = 'ProtectionIv'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] ) VALUES ( 'ProtectionIv', '' )
	RAISERROR ('ProtectionIv application option setting has been added', 0, 1) WITH NOWAIT
END

RAISERROR ('Adding client database backup status record, if missing.', 0, 1) WITH NOWAIT

IF (NOT EXISTS(SELECT 1 FROM [dbo].[ClientDatabaseBackupStatus]))
BEGIN
    INSERT INTO	[dbo].[ClientDatabaseBackupStatus] ( [LastFullBackup], [LastDifferentialBackup], [LastTransactionLogBackup] ) VALUES ( NULL, NULL, NULL )
	RAISERROR ('Client database backup status record has been added', 0, 1) WITH NOWAIT
END