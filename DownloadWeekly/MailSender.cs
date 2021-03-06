﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using DownloadWeekly.Dtos;
using EbaySweden.Trading.DatabaseAccess;

namespace DownloadWeekly
{
    public class MailSender : IMailSender
    {
        private IDbAccessor _dbAccessor;
        private SmtpClient _smtpServer;

        public MailSender(IDbAccessor dbAccessor, MailSettings mailSettings)
        {
            _dbAccessor = dbAccessor;
            _smtpServer = new SmtpClient(mailSettings.SmtpServer, mailSettings.SmtpPort);
        }

        public string SendMail()
        {
            var filesToSend = GetAttachmentsToSend();
            var recipients = GetRecipients();
            if (filesToSend == null || !filesToSend.Any())
                return "No files to send.";
            if (recipients == null || !recipients.Any())
                return "No recipients to send to.";

            using (_smtpServer)
            {
                foreach (var fileToSend in filesToSend)
                {
                    var mail = new MailMessage();
                    mail.From = new MailAddress("veckobrev@numlock.se");
                    mail.Subject = string.Format("Veckobrev v.{0} {1}", fileToSend.WeekNumber, fileToSend.Name);
                    mail.Attachments.Add(new Attachment(new MemoryStream(fileToSend.FileContent), Path.GetFileName(fileToSend.FileName)));
                    foreach (var recipient in recipients.Where(r => r.DocumentId == fileToSend.DocumentToDownloadId))
                        mail.To.Add(recipient.Email);
                    if (mail.To.Count == 0)
                        continue;
                    _smtpServer.Send(mail);
                }
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
                    Name = r["Name"].ToString(), 
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
}