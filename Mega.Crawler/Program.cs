namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Services;
    using Mega.Services.InfoParser;

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
            registry.IncludeRegistry<SettingsInstaller>();
            registry.IncludeRegistry<ServicesInstaller>();

            try
            {
                using (var container = new Container(registry))
                {
                    ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);
                    Logger.LogInformation($"Starting with {container.GetInstance<Settings>().RootUriString}");
                    using (var client = new WebClient())
                    {
                        container.Configure(
                            r => r.For<Func<Uri, string>>().Use(
                                (Func<Uri, string>)(uri =>
                                                           {
                                                               Thread.Sleep(new Random().Next(5000, 15000));
                                                               return client.DownloadString(uri);
                                                           })));

                        var runner = container.GetInstance<Runner>();
                        runner.Run();

                        Logger.LogInformation(
                            $"All {container.GetInstance<HashSet<Uri>>().Count} urls done! "
                            + $"All {container.GetInstance<Dictionary<string, ArticleInfo>>().Count} articles done!");

                        container.Release(runner);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            Console.ReadLine();
            Logger.LogDebug("Exit Application");
        }
    }
}