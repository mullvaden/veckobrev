using System;
using System.Globalization;
using System.IO;
using System.Net;
using EbaySweden.Trading.DatabaseAccess;

namespace DownloadWeekly
{
    public class FileGetter
    {
        private IClock _clock;
        private readonly IDbAccessor _dbAccessor;
        private const string BaseUrl = "http://arstadalsskolan.stockholm.se/sites/default/files/vecka_{0}_vinbar_f-3.pdf";

        public FileGetter(IClock clock, IDbAccessor dbAccessor)
        {
            _clock = clock;
            _dbAccessor = dbAccessor;
        }

        public string DownloadWeeklyLetter()
        {
            var weekNumber = GetWeekNumber();
            var filename = string.Format(BaseUrl, weekNumber);
            if (IsFileAlreadyDownloaded(filename))
                return "Already downloaded " + filename;

            var file = DoDownload(filename);
            if (file != null)
            {
                SaveFile(filename, file, weekNumber);
            }
            return "Downloaded " + filename;
        }

        private void SaveFile(string filename, byte[] file, int weekNumber)
        {
            var parameters = new SqlParameterHandler(p =>
            {
                p.AddWithValue("@filename", filename);
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

        private int GetWeekNumber()
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(_clock.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
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
            catch (Exception ex)
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
