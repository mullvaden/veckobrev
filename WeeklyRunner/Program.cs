using System;
using System.Configuration;
using System.Reflection;
using Autofac;
using DownloadWeekly;
using DownloadWeekly.Dtos;
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
            var container = SetupIoC();
            if (args[0].Equals("download", StringComparison.InvariantCultureIgnoreCase))
                container.Resolve<IFileGetter>().DownloadWeeklyLetter();
            else if (args[0].Equals("email", StringComparison.InvariantCultureIgnoreCase))
                Console.WriteLine(container.Resolve<IMailSender>().SendMail());
            else if (args.Length > 0)
                Console.WriteLine("Unknown arg! {0}", args[0]);
            else
                Console.WriteLine("Specify argument: \"email\" or \"download\" ");
        }

        private static IContainer SetupIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.Load("DownloadWeekly"))
                .AsImplementedInterfaces();
            builder.Register(c => new DbAccessor(_connectionString)).As<IDbAccessor>();
            builder.Register(c => new MailSettings(_smtp, _smtpPort)).AsSelf();

            return builder.Build();
        }
    }
}
