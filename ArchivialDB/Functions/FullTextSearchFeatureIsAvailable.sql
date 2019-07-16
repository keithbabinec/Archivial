CREATE FUNCTION [dbo].[FullTextSearchFeatureIsAvailable]
(
)
RETURNS BIT
AS
BEGIN
	DECLARE @Result BIT 
	
	SELECT @Result = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')

	IF @Result IS NULL
	BEGIN
		-- no value returned?
		-- fallback to disabled since we always need to return something here.
		SET @Result = 0
	END

	RETURN @Result
END