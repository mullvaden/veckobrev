
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveFile]') AND type in (N'P', N'PC'))
    BEGIN
        EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveFile] AS SELECT ''dummy'''
    END
GO

ALTER PROCEDURE [dbo].[SaveFile] 
	@FileName VARCHAR(100),
	@documentToDownloadId INT,
	@fileContent varbinary(max),
	@weeknumber INT
AS

BEGIN

	INSERT dbo.DownloadedDocument
	        ( Filename ,
			  DocumentToDownloadId,
	          FileContent ,
	          WeekNumber 
	          )
	VALUES  ( @FileName,
	          @documentToDownloadId,
			  @fileContent,
	          @weeknumber 
	        )
END