using System;

namespace DownloadWeekly.Dtos
{
    public class MailSettings 
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }

        public MailSettings(string smtpServer, int smtpPort)
        {
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
        }
    }
}