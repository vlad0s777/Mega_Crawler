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
            var client = new WebClient();
            var clientDelegate = new WebClientDelegate(client.DownloadString);
            var consumer = new CollectContent(messages, reports, visitedUrls, rootUri, clientDelegate);
            var producer = new UrlFinder(messages, reports);
            while (!reports.IsEmpty() || !messages.IsEmpty())
            {
                consumer.Work();
                producer.Work();
            }

            Console.ResetColor();
            Console.WriteLine($"All {visitedUrls.Count} urls done!");
        }
    }
}