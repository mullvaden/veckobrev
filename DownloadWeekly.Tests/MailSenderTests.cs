﻿using EbaySweden.Trading.DatabaseAccess;
using NUnit.Framework;

namespace DownloadWeekly.Tests
{
    [TestFixture]
    public class MailSenderTests
    {
        private const string ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=WeeklyDownloader;";

        [Test]
        public void name()
        {
            // Arrange
            var mailsender = new MailSender(new DbAccessor(ConnectionString), "test07", 25);

            // Act
            mailsender.SendMail();
            // Assert

        }
    }
}