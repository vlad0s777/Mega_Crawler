namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Mega.Messaging;
    using Mega.Services;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    internal class Program
    {
        private static readonly ILogger Logger = GetLogger();

        private static ILogger GetLogger()
        {
            var logger = ApplicationLogging.CreateLogger<Program>();
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, e) => logger.LogCritical(e.ExceptionObject.ToString());
            return logger;
        }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../../Properties"))
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = new Settings(builder.Build());

            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);
            
            while (string.IsNullOrWhiteSpace(settings.RootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl: ");
                settings.RootUriString = Console.ReadLine();
            }

            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            Logger.LogInformation($"Starting with {settings.RootUriString}");
            try
            {
                var rootUri = new Uri(settings.RootUriString, UriKind.Absolute);
                messages.Send(new UriLimits(rootUri));

                var visitedUrls = new HashSet<Uri>();
                var infoDictionary = new Dictionary<string, ArticleInfo>();
                using (var client = new WebClient())
                {
                    var pageCollect = new ServiceContentCollect(messages, reports, visitedUrls, client.DownloadString, settings);

                    var infoParser = new ServiceInfoParser(messages, reports, infoDictionary, settings);

                    while (!reports.IsEmpty() || !messages.IsEmpty())
                    {
                        if (!pageCollect.Work() || !infoParser.Work())
                        {
                            break;
                        }

                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                }

                Logger.LogInformation($"All {visitedUrls.Count} urls done! All {infoDictionary.Count} articles done!");
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