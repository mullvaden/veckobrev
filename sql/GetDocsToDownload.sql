
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDocsToDownload]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetDocsToDownload] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[GetDocsToDownload] 
	
AS

BEGIN

	SELECT * FROM dbo.DocumentToDownload


END