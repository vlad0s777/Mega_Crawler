using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mega.Messaging;
using Mega.Services;

namespace Mega.Crawler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
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
                    if (!collectContent.Work() || !uriFinder.Work())
                        break;
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                            break;
                }
            }

            Console.ResetColor();
            Console.WriteLine($"All {visitedUrls.Count} urls done!");
            Console.ReadLine();
        }
    }
}