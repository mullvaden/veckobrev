﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using DownloadWeekly.Dtos;
using EbaySweden.Trading.DatabaseAccess;

namespace DownloadWeekly
{
    public class FileGetter : IFileGetter
    {
        private readonly IClock _clock;
        private readonly IDbAccessor _dbAccessor;

        public FileGetter(IClock clock, IDbAccessor dbAccessor)
        {
            _clock = clock;
            _dbAccessor = dbAccessor;
        }

        public string DownloadWeeklyLetter()
        {
            var listOfDocsToDownload = GetDocsToDownload();
            var weekNumberStart = GetWeekNumberDaysFromNow(4);
            var counter = 0;
            for (var weekNumber = weekNumberStart; weekNumber < weekNumberStart + 1; weekNumber++)
            {
                foreach (var doc in listOfDocsToDownload)
                {
                    var filename = string.Format(doc.BaseUrl, weekNumber);
                    if (IsFileAlreadyDownloaded(filename))
                        continue;

                    var file = DoDownload(filename);
                    if (file == null) continue;
                    counter++;
                    SaveFile(filename, file, weekNumber, doc.Id);
                }
            }

            return "Downloaded " + counter;
        }

        private IEnumerable<DocumentToDownload> GetDocsToDownload()
        {
            var reader = new ParametersAndReader<DocumentToDownload>
            {
                RecordReader = r => new DocumentToDownload
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"].ToString(),
                    BaseUrl = r["BaseUrl"].ToString(),
                }
            };
            return _dbAccessor.PerformSpRead(reader, "[dbo].[GetDocsToDownload]");
        }

        private void SaveFile(string filename, byte[] file, int weekNumber, int documentId)
        {
            var parameters = new SqlParameterHandler(p =>
            {
                p.AddWithValue("@filename", filename);
                p.AddWithValue("@documentToDownloadId", documentId);
                p.AddWithValue("@filecontent", file);
                p.AddWithValue("@weeknumber", weekNumber);
            });
            _dbAccessor.PerformSpNonQuery("[dbo].[SaveFile]", parameters);
        }

        public bool IsFileAlreadyDownloaded(string fileName)
        {
            var reader = new ParametersAndReader<bool>
                        {
                            Parameters = parameters => parameters.AddWithValue("@filename", fileName),
                            RecordReader = r => Convert.ToBoolean(r["FileIsDownloaded"])
                        };
            return _dbAccessor.PerformSpReadSingle(reader, "IsFileDownloaded");
        }

        private int GetWeekNumberDaysFromNow(int dayOffset)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(_clock.Now.AddDays(dayOffset), dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        private byte[] DoDownload(string url)
        {
            try
            {
                var stream = WebRequest.Create(url)
                    .GetResponse()
                    .GetResponseStream();

                var bytes = ReadFully(stream);
                //var read = new StreamReader(sStream).ReadToEnd();
                return bytes;

            }
            catch (Exception)
            {
                //Url not valid
                return null;
            }

        }
        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
