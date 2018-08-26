namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class Runner
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Runner>();

        private readonly IMessageBroker[] brokers;

        private readonly IMessageProcessor[] handlers;

        private readonly HashSet<Uri> visited_urls;

        private readonly Dictionary<string, ArticleInfo> articles;

        private readonly Settings settings;

        public Runner(IMessageBroker[] brokers, IMessageProcessor[] handlers, Settings settings, HashSet<Uri> visitedUrls, Dictionary<string, ArticleInfo> articles)
        {
            this.settings = settings;
            this.visited_urls = visitedUrls;
            this.articles = articles;

            while (string.IsNullOrWhiteSpace(settings.RootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl: ");
                settings.RootUriString = Console.ReadLine();
            }

            this.brokers = brokers;
            this.handlers = handlers;     
        }

        public void Run()
        {
            if (this.brokers.All(broker => broker.IsEmpty()))
            {
                var rootUri = new Uri(this.settings.RootUriString, UriKind.Absolute);
                foreach (var broker in this.brokers)
                {
                    if (broker is IMessageBroker<UriRequest> requestBroker)
                    {
                        requestBroker.Send(new UriRequest(rootUri));
                    }
                }                             
            }
         
            while (true)
            {
                if (this.handlers.Any(handler => !handler.Run()))
                {
                    break;
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            Logger.LogInformation(
                $"All {this.visited_urls.Count} urls done! "
                + $"All {this.articles.Count} articles done!");
        }
    }
}
