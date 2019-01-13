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

RAISERROR ('Adding core application option defaults.', 0, 1) WITH NOWAIT

IF (NOT EXISTS(SELECT [ID] FROM [dbo].[ApplicationOptions] WHERE [Name] = 'LowPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] )
	VALUES ( 'LowPriorityScanFrequencyInHours', '48' )
END

IF (NOT EXISTS(SELECT [ID] FROM [dbo].[ApplicationOptions] WHERE [Name] = 'MedPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] )
	VALUES ( 'MedPriorityScanFrequencyInHours', '12' )
END

IF (NOT EXISTS(SELECT [ID] FROM [dbo].[ApplicationOptions] WHERE [Name] = 'HighPriorityScanFrequencyInHours'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] )
	VALUES ( 'HighPriorityScanFrequencyInHours', '1' )
END

IF (NOT EXISTS(SELECT [ID] FROM [dbo].[ApplicationOptions] WHERE [Name] = 'StatusUpdateSchedule'))
BEGIN
    INSERT INTO	[dbo].[ApplicationOptions] ( [Name], [Value] )
	VALUES ( 'StatusUpdateSchedule', '0 8 * * *' )
END

