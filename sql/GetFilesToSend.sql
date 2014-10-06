
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetFilesToSend]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetFilesToSend] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[GetFilesToSend] 
	
AS

BEGIN

	SELECT dd.*, dtd.Name FROM dbo.DownloadedDocument dd 
	INNER JOIN dbo.DocumentToDownload dtd ON dtd.Id = dd.DocumentToDownloadId
	WHERE Sent = 0


END