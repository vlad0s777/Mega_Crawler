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
        static ILogger getLogger()
        {
            ILogger logger =  ApplicationLogging.CreateLogger<Program>();
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
                    if (category.Contains("CollectContent") && level >= LogLevel.Warning)
                        return true;
                    else if (category.Contains("Program") && level >= LogLevel.Information)
                        return true;
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

            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriAttempt>(); 

            Logger.LogInformation($"Starting with {rootUriString}");
           
            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);

            var visitedUrls = new HashSet<Uri>();
            using (var client = new WebClient())
            {
                var collectContent = new CollectContent(messages, reports, visitedUrls, rootUri,
                    client.DownloadString, limit);
                var uriFinder = new UrlFinder(messages, reports, depth);
                while (!reports.IsEmpty() || !messages.IsEmpty())
                {
                    if (!collectContent.Work() || !uriFinder.Work())
                    {
                        break;
                    }

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            }

            Logger.LogInformation($"All {visitedUrls.Count} urls done!");
            Console.ReadLine();
            Logger.LogDebug("Exit Application");
        }
    }
}