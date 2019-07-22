/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) != 1
BEGIN
	;THROW 50000, 'The SQL Server full-text search feature is not installed or not enabled for this SQL Server installation.', 1
END