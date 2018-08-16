using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Mega.Messaging;
using Mega.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mega.Crawler
{
    internal class Program
    {
        private static readonly ILogger Logger = getLogger();

        private static ILogger getLogger()
        {
            var logger = ApplicationLogging.CreateLogger<Program>();
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, e) => logger.LogCritical(e.ExceptionObject.ToString());
            return logger;
        }

        //   private static int ttt = 0;
        //   private static int vvv = 10/ttt;
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../../Properties"))
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = builder.Build();
            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);
            var depthLimit = Convert.ToInt32(settings["depthLimit"]);
            var countLimit = Convert.ToInt32(settings["countLimit"]);
            var attemptLimit = Convert.ToInt32(settings["attemptLimit"]);
            var rootUriString = args.FirstOrDefault();
            while (string.IsNullOrWhiteSpace(rootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl: ");
                rootUriString = Console.ReadLine();
            }

            var pageReports = new MessageBroker<UriBody>();
            var pageMessages = new MessageBroker<UriLimits>();
            var articleMessages = new MessageBroker<UriLimits>();
            var articleReports = new MessageBroker<UriBody>();

            Logger.LogInformation($"Starting with {rootUriString}");

            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);

            var visitedUrls = new HashSet<Uri>();
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            using (var client = new WebClient())
            {
                var collectPageContent = new ServiceCollectContent(pageMessages, pageReports, visitedUrls, rootUri,
                    client.DownloadString, countLimit, attemptLimit, timeout: true);

                var uriFinderArticle = new ServiceUrlParcer(pageMessages, pageReports, articleMessages, depthLimit);

                var collectArticleContent = new ServiceCollectContent(articleMessages, articleReports, visitedUrls, rootUri,
                    client.DownloadString, countLimit, attemptLimit, timeout: true);

                var infoFinderArticle = new ServiceInfoParcer(infoDictionary, articleReports, depthLimit);

                while (!pageReports.IsEmpty() || !pageMessages.IsEmpty() || !articleMessages.IsEmpty() ||
                       !articleReports.IsEmpty())
                {
                    if (!collectPageContent.Work() || !uriFinderArticle.Work() ||
                        !collectArticleContent.Work() || !infoFinderArticle.Work())
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
            Console.ReadLine();
            Logger.LogDebug("Exit Application");
        }
    }
}