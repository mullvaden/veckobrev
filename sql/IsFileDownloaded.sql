
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IsFileDownloaded]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[IsFileDownloaded] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[IsFileDownloaded] 
	@FileName VARCHAR(100)
AS

BEGIN

	IF EXISTS (SELECT * FROM dbo.DownloadedDocument WHERE FileName = @FileName)
		SELECT 1 FileIsDownloaded
	ELSE 
		SELECT 0 FileIsDownloaded

END