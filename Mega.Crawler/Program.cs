namespace Mega.Crawler
{
    using System;
    using System.Net;
    using System.Threading;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Services;

    using Microsoft.Extensions.Logging;

    using StructureMap;

    internal class Program
    {
        private static readonly ILogger Logger = GetLogger();

        private static ILogger GetLogger()
        {
            var logger = ApplicationLogging.CreateLogger<Program>();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => logger.LogCritical(e.ExceptionObject.ToString());
            return logger;
        }

        private static void Main()
        {
            var registry = new Registry();

            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);

            try
            {
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();

                var random = new Random();

                using (var container = new Container(registry))
                {
                    using (var client = new WebClient())
                    {
                        container.Configure(
                            r => r.For<Func<Uri, string>>().Use(
                                (Func<Uri, string>)(uri =>
                                                           {
                                                               Thread.Sleep(random.Next(5000, 15000));
                                                               return client.DownloadString(uri);
                                                           })));
                    }

                    var runner = container.GetInstance<Runner>();

                    runner.Run();
                    Console.ReadLine();
                    container.Release(runner);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            
            Logger.LogDebug("Exit Application");
        }
    }
}