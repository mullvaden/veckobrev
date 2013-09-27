--drop table Subscription
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscriptions]') AND type in (N'U')) 
BEGIN

CREATE TABLE [dbo].[Subscription]
(
DocumentToDownloadId INT NOT NULL,
[EmailAddress] [varchar] (100) COLLATE Finnish_Swedish_CI_AS NOT NULL
) ON [PRIMARY]

END

