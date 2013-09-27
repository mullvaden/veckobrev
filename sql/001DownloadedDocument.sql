--drop table DownloadedDocument
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DownloadedDocument]') AND type in (N'U')) 
BEGIN

CREATE TABLE [dbo].[DownloadedDocument]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Filename] [varchar] (100) COLLATE Finnish_Swedish_CI_AS NOT NULL,
[FileContent] [varbinary] (max) NOT NULL,
[WeekNumber] [int] NOT NULL,
[Created] [datetime] NOT NULL CONSTRAINT [DF_DownloadedDocument_Created] DEFAULT (getdate()),
[Sent] [bit] NOT NULL CONSTRAINT [DF_DownloadedDocument_Sent] DEFAULT ((0))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[DownloadedDocument] ADD CONSTRAINT [PK_DownloadedDocument] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO


END
GO

