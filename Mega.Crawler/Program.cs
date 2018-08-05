namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using System.Text.RegularExpressions;

    using Messaging;
    using Mega.Services;
    class Program
    {
        static void Main(string[] args)
        {
            var rootUriString = args.FirstOrDefault();

            while (string.IsNullOrWhiteSpace(rootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl:");
                rootUriString = Console.ReadLine();
            }

            Consumer cons = new Consumer();
            Producer prod = new Producer();
            
            Console.WriteLine($"Starting with {rootUriString}");

            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);
            const string hrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])"; //|(?<uri>\\S+)
            prod.Do_task(rootUri);
            Worker work = new Worker();

            while (!cons.Reports.Task_done() || !prod.Tasks.Task_done()) 
            {
                work.Consuming(cons, prod, rootUri);
                work.Producing(cons, prod, hrefPattern);
            }

            Console.ResetColor();
            Console.WriteLine($"All {work.visitedUrls.Count} urls done!");

        
        }
    }
}
