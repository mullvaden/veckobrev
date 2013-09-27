﻿using System;
using EbaySweden.Trading.DatabaseAccess;
using NUnit.Framework;
using Rhino.Mocks;

namespace DownloadWeekly.Tests
{
    [TestFixture]
    public class FileGetterTests
    {
        private const string ConnectionString ="Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=WeeklyDownloader;";

        [Test]
        public void trygetfile()
        {
            // Arrange
            var clock = MockRepository.GenerateMock<IClock>();
            clock.Stub(t => t.Now).Return(new DateTime(2013, 9, 25));
            var getter = new FileGetter(clock, MockRepository.GenerateMock<IDbAccessor>());
            // Act
            getter.DownloadWeeklyLetter();
            // Assert
        }
        [Test]
        public void trygetwithCheckinDb()
        {
            // Arrange
            var clock = MockRepository.GenerateMock<IClock>();
            clock.Stub(t => t.Now).Return(new DateTime(2013, 9, 9));
            var getter = new FileGetter(clock, new DbAccessor(ConnectionString));
            // Act
            getter.DownloadWeeklyLetter();
            // Assert
        }
    }
}
