namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using StructureMap;

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

        private static void Main()
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

            var installClass = new ClassInstallator
            {
                Container = new Container()
            };
            installClass.InstallClass(settings);
            var container = installClass.Container;

            var requests = container.GetInstance<IMessageBroker<UriRequest>>();
            Logger.LogInformation($"Starting with {settings.RootUriString}");

            try
            {
                var rootUri = new Uri(settings.RootUriString, UriKind.Absolute);
                requests.Send(new UriRequest(rootUri));
                var visitedUrls = new HashSet<Uri>();
                var articles = new Dictionary<string, ArticleInfo>();

                    var pageCollector = container.With(visitedUrls).GetInstance<IMessageProcessor<UriRequest>>();
                    var infoParser = container.With(articles).GetInstance<IMessageProcessor<UriBody>>();

                var brokers = container.GetAllInstances<IMessageBroker>();
                while (!brokers.All(broker => broker.IsEmpty()))
                {
                    if (!pageCollector.Run() || !infoParser.Run())
                    {
                        break;
                    }

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
                //container.EjectAllInstancesOf<>();
                Logger.LogInformation($"All {visitedUrls.Count} urls done! All {articles.Count} articles done!");
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