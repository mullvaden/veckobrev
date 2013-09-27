--drop table DocumentToDownload
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DocumentToDownload]') AND type in (N'U')) 
BEGIN

CREATE TABLE [dbo].[DocumentToDownload]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] varchar (100),
[BaseUrl] varchar (200)
) ON [PRIMARY]

ALTER TABLE [dbo].[DocumentToDownload] ADD CONSTRAINT [PK_DocumentToDownload] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]

END

