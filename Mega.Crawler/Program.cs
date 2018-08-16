using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                .AddJsonFile($"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = builder.Build();
            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);
            var limitations = new Limitations(settings["depth"], settings["count"], settings["attempt"]);
            
            var rootUriString = args.FirstOrDefault();
            while (string.IsNullOrWhiteSpace(rootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl: ");
                rootUriString = Console.ReadLine();
            }
            var conteiner = new InstallClass(limits:limitations).Container;

            var pageReports = conteiner.GetInstance<IMessageBroker>("BodyBroker");
            var pageMessages = conteiner.GetInstance<IMessageBroker>("LimitsBroker");
            var articleMessages = conteiner.GetInstance<IMessageBroker>("LimitsBroker");
            var articleReports = conteiner.GetInstance<IMessageBroker>("BodyBroker");
            Logger.LogInformation($"Starting with {rootUriString}");

            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);

            var visitedUrls = new HashSet<Uri>();
            var infoDictionary = new Dictionary<string, ArticleInfo>();
            using (var client = new WebClient())
            {
                foreach (var mb in conteiner.With(visitedUrls).With(rootUri).With(new Func <Uri, string>(client.DownloadString)).With(infoDictionary).GetAllInstances<IMessageProcessor>())
                {
                    mb.Run();
                }
                /*var collectPageContent = new CollectContent(pageMessages, pageReports, visitedUrls, rootUri,
                    client.DownloadString,true, limitations.countLimit, limitations.attemptLimit);

                var uriFinderArticle = new ArticleUrlParcer(pageMessages, pageReports, articleMessages, limitations.depthLimit);

                var collectArticleContent = new CollectContent(articleMessages, articleReports, visitedUrls, rootUri, client.DownloadString, true, limitations.countLimit, limitations.attemptLimit);

                var infoFinderArticle = new ArticleInfoParcer(infoDictionary, articleReports, limitations.depthLimit);*/

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