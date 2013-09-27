
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveFilesSent]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveFilesSent] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[SaveFilesSent] 
	@ids AS TableOfInts READONLY
AS

BEGIN

	UPDATE dbo.DownloadedDocument 
	SET Sent = 1
	FROM DownloadedDocument dd
	INNER JOIN @ids ids ON dd.Id = ids.id

END