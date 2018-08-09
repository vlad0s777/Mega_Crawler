using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Mega.Messaging;
using Mega.Services;
using Microsoft.Extensions.Configuration;

namespace Mega.Crawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile(
                    $"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);
            var settings = builder.Build();
            var depth = Convert.ToInt32(settings["depth"]);
            var limit = Convert.ToInt32(settings["count"]);
            var rootUriString = args.FirstOrDefault();

            while (string.IsNullOrWhiteSpace(rootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl:");
                rootUriString = Console.ReadLine();
            }

            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<Uri>();

            Console.WriteLine($"Starting with {rootUriString}");

            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);

            var visitedUrls = new HashSet<Uri>();

            using (var client = new WebClient())
            {
                var collectContent = new CollectContent(messages, reports, visitedUrls, rootUri,
                    client.DownloadString);
                var uriFinder = new UrlFinder(messages, reports);
                while (!reports.IsEmpty() || !messages.IsEmpty())
                {
                    if (!collectContent.Work(limit) || !uriFinder.Work(depth))
                    {
                        break;
                    }

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            }

            Console.ResetColor();
            Console.WriteLine($"All {visitedUrls.Count} urls done!");
            Console.ReadLine();
        }
    }
}