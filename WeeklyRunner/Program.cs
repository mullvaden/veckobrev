using System;
using System.Configuration;
using DownloadWeekly;
using EbaySweden.Trading.DatabaseAccess;

namespace WeeklyRunner
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["WeeklyDownloaderDb"].ConnectionString;
        private static string _smtp = ConfigurationManager.AppSettings["Smtp"];
        private static int _smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

        static void Main(string[] args)
        {
            if (args[0].Equals("download", StringComparison.InvariantCultureIgnoreCase)){
              Console.Write(new FileGetter(new Clock(), new DbAccessor(_connectionString)).DownloadWeeklyLetter());
}
            else if (args[0].Equals("email", StringComparison.InvariantCultureIgnoreCase))
                new MailSender(new DbAccessor(_connectionString), _smtp, _smtpPort).SendMail();
            else
            Console.WriteLine("Unknown arg!");
        }
    }
}
