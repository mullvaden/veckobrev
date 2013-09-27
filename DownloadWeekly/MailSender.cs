using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using DownloadWeekly.Dtos;
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

        public string SendMail()
        {

            var filesToSend = GetAttachmentsToSend();
            var recipients = GetRecipients();
            if (filesToSend == null || !filesToSend.Any())
                return "No files to send.";
            if (recipients == null || !recipients.Any())
                return "No recients to send to.";

            var smtpServer = new SmtpClient(_smtpServer, _portnumber);
            foreach (var fileToSend in filesToSend)
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("veckobrev@numlock.se");
                mail.Subject = "Veckobrev " + fileToSend.WeekNumber;
                var filename = Path.GetFileName(fileToSend.FileName);
                mail.Attachments.Add(new Attachment(new MemoryStream(fileToSend.FileContent), filename));
                foreach (var recipient in recipients.Where(r => r.DocumentId == fileToSend.DocumentToDownloadId))
                    mail.To.Add(recipient.Email);
                smtpServer.Send(mail);
            }

            SaveDocumentSent(filesToSend.Select(t => t.Id));

            return string.Format(" Sent {0} file(s) to {1} recipients.", filesToSend.Count(), recipients.Count());
        }

        private IEnumerable<Recipient> GetRecipients()
        {
            var reader = new ParametersAndReader<Recipient>
            {
                RecordReader = r => new Recipient
                {
                    Email = r["EmailAddress"].ToString(),
                    DocumentId = Convert.ToInt32(r["DocumentToDownloadId"])

                }
            };
            return _dbAccessor.PerformSpRead(reader, "[dbo].[GetRecipients]");
        }

        private IEnumerable<FileToSend> GetAttachmentsToSend()
        {
            var reader = new ParametersAndReader<FileToSend>
            {
                RecordReader = r => new FileToSend
                {
                    Id = Convert.ToInt32(r["Id"]),
                    DocumentToDownloadId = Convert.ToInt32(r["DocumentToDownloadId"]),
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
        public int DocumentToDownloadId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int WeekNumber { get; set; }
    }
}