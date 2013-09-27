
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecipients]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetRecipients] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[GetRecipients] 
	
AS

BEGIN

	SELECT * FROM dbo.Subscription


END