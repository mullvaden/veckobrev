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
            if (args[0].Equals("download", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.Write(new FileGetter(new Clock(), new DbAccessor(_connectionString)).DownloadWeeklyLetter());
            }
            else if (args[0].Equals("email", StringComparison.InvariantCultureIgnoreCase))
                Console.WriteLine(new MailSender(new DbAccessor(_connectionString), _smtp, _smtpPort).SendMail());
            else if (args.Length > 0)
                Console.WriteLine("Unknown arg! {0}", args[0]);
            else
                Console.WriteLine("Specify argument: \"email\" or \"download\" ");
        }
    }
}
