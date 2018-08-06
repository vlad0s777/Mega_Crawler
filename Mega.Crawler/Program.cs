using System;
using System.Collections.Generic;
using System.Linq;
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
            const string hrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])"; //|(?<uri>\\S+)

            var visitedUrls = new HashSet<Uri>();
            var consumer = new Consumer(messages, reports, visitedUrls, rootUri);
            var producer = new Producer(messages, reports, hrefPattern);
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