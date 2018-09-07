namespace Mega.Crawler
{
    using System;
    using System.Linq;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.UriRequest;

    using Microsoft.Extensions.Logging;

    public class Runner
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Runner>();

        private readonly IMessageBroker[] brokers;

        private readonly IMessageProcessor[] processors;

        private readonly Settings settings;

        public Runner(IMessageBroker[] brokers, IMessageProcessor[] processors, Settings settings)
        {
            this.settings = settings;
            this.brokers = brokers;
            this.processors = processors;     
        }

        public void Run()
        {
            while (string.IsNullOrWhiteSpace(this.settings.RootUriString))
            {
                Console.WriteLine("Please enter absolute root url to crawl: ");
                this.settings.RootUriString = Console.ReadLine();
            }

            if (this.brokers.All(broker => broker.IsEmpty()))
            {
                var rootUri = new Uri(this.settings.RootUriString, UriKind.Absolute);
                this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(rootUri));                        
            }

            foreach (var messageProcessor in this.processors)
            {
                messageProcessor.Run();
            }

            Console.ReadLine();
        }
    }
}
