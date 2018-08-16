using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Mega.Crawler.Infrastructure.IoC;
using Mega.Messaging;
using Mega.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StructureMap;
using StructureMap.Pipeline;

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
                .AddJsonFile(
                    $"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = builder.Build();
            ApplicationLogging.LoggerFactory
                .AddConsole((category, level) =>
                {
                    if (category.Contains("CollectContent") || category.Contains("ArticleUrlParcer") ||
                        category.Contains("ArticleInfoFinder") && level >= LogLevel.Debug)
                    {
                        return true;
                    }

                    if (category.Contains("Program") && level >= LogLevel.Information)
                    {
                        return true;
                    }

                    return false;
                })
                .AddEventLog(LogLevel.Debug);
            var depth = Convert.ToInt32(settings["depth"]);
            var limit = Convert.ToInt32(settings["count"]);
            var attempt = Convert.ToInt32(settings["attempt"]);
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
                var cont = new Container();
                var brok = cont.TryGetInstance<IMessageBroker>();
                var colCon = cont.TryGetInstance<IMessageProcessor>();
                colCon.Run();
                var collectPageContent = new CollectContent(pageMessages, pageReports, visitedUrls, rootUri,
                    client.DownloadString,true, limit, attempt);
                var uriFinderArticle = new ArticleUrlParcer(pageMessages, pageReports, articleMessages, depth);
                var collectArticleContent = new CollectContent(articleMessages, articleReports, visitedUrls, rootUri,
                    client.DownloadString, true, limit, attempt);
                var infoFinderArticle = new ArticleInfoParcer(infoDictionary, articleReports, depth);
                while (!pageReports.IsEmpty() || !pageMessages.IsEmpty() || !articleMessages.IsEmpty() ||
                       !articleReports.IsEmpty())
                {
                    if (!collectPageContent.Run() || !uriFinderArticle.Run() ||
                        !collectArticleContent.Run() || !infoFinderArticle.Run())
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