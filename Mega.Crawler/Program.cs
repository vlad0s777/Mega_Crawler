namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using System.Text.RegularExpressions;

    using Messaging;

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

            var messageBroker = new MessageBroker<Uri>();

            Console.WriteLine($"Starting with {rootUriString}");

            // Preload
            var rootUri = new Uri(rootUriString, UriKind.Absolute);
            messageBroker.Send(rootUri);
            
            const string hrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"']|(?<uri>\\S+))";

            // The Producer
            void AnalyzeDocument(Uri documentUri, string documentBody)
            {
                var m = Regex.Match(documentBody, hrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

                while (m.Success)
                {
                    try
                    {
                        var absUri = new Uri(documentUri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
                        messageBroker.Send(absUri);
                    }
                    catch (Exception)
                    {
                        Console.ResetColor();
                        Console.WriteLine($"Ignoring {m.Value}");
                    }

                    m = m.NextMatch();
                }
            }
            // end of the Producer

            var visitedUrls = new HashSet<Uri>();
            
            // The Consumer
            using (var client = new WebClient())
            {
                while (messageBroker.TryReceive(out var uri))
                { 
                    if (rootUri.IsBaseOf(uri) && visitedUrls.Add(uri))
                    {
                        var documentBody = client.DownloadString(uri);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"OK {uri}");

                        AnalyzeDocument(uri, documentBody);
                    }
                }
            }
            // end of the Consumer

            Console.ResetColor();
            Console.WriteLine($"All {visitedUrls.Count} urls done!");
        }
    }
}
