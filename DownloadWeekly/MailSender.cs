using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using EbaySweden.Trading.DatabaseAccess;

namespace DownloadWeekly
{
    public class MailSender
    {
        private IDbAccessor _dbAccessor;
        private readonly string _smtpServer;
        private readonly int _portnumber;

        public MailSender(IDbAccessor dbAccessor, string smtpServer, int portnumber)
        {
            _dbAccessor = dbAccessor;
            _smtpServer = smtpServer;
            _portnumber = portnumber;
        }

        public void SendMail()
        {

            var filesToSend = GetAttachmentsToSend();
            List<string> recipients = GetRecipients();
            if (filesToSend == null || filesToSend.Count == 0)
                return;
            if (recipients == null || recipients.Count == 0)
                return;
            var mail = new MailMessage();
            var smtpServer = new SmtpClient(_smtpServer);
            mail.From = new MailAddress("weekly@numlock.se");
            foreach (var recipient in recipients)
                mail.To.Add(recipient);
            foreach (var fileToSend in filesToSend)
            {
                mail.Subject = "Veckobrev " + fileToSend.WeekNumber;
                var filename = Path.GetFileName(fileToSend.FileName);
                mail.Attachments.Add(new Attachment(new MemoryStream(fileToSend.FileContent), filename));
            }
            smtpServer.Port = _portnumber;
            smtpServer.Send(mail);
            SaveDocumentSent(filesToSend.Select(t => t.Id));
        }

        private List<string> GetRecipients()
        {
            var reader = new ParametersAndReader<string>
            {
                RecordReader = r => r["EmailAddress"].ToString()

            };
            return _dbAccessor.PerformSpRead(reader, "[dbo].[GetRecipients]");
        }

        private List<FileToSend> GetAttachmentsToSend()
        {
            var reader = new ParametersAndReader<FileToSend>
            {
                RecordReader = r => new FileToSend
                {
                    Id = Convert.ToInt32(r["Id"]),
                    FileName = r["FileName"].ToString(),
                    FileContent = (byte[])r["FileContent"],
                    WeekNumber = Convert.ToInt32(r["WeekNumber"])
                }
            };
            return _dbAccessor.PerformSpRead(reader, "[dbo].[GetFilesToSend]");
        }

        private void SaveDocumentSent(IEnumerable<int> downloadedDocumentIds)
        {
            _dbAccessor.PerformSpNonQuery("[dbo].[SaveFilesSent]", p => p.AddWithValues("@ids", downloadedDocumentIds));
        }
    }

    internal class FileToSend
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int WeekNumber { get; set; }
    }
}