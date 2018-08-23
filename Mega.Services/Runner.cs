namespace Mega.Services
{
    using System;
    using System.Linq;

    using Mega.Messaging;
    using Mega.Services.ContentCollector;

    public class Runner
    {
        private readonly IMessageBroker[] brokers;

        private readonly IMessageProcessor[] handlers;

        private readonly Settings settings;

        public Runner(IMessageBroker[] brokers, IMessageProcessor[] handlers, Settings settings)
        {
            this.settings = settings;

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
            var rootUri = new Uri(this.settings.RootUriString, UriKind.Absolute);

            foreach (var broker in this.brokers)
            {
                if (broker is IMessageBroker<UriRequest> requestBroker)
                {
                    requestBroker.Send(new UriRequest(rootUri));
                    break;
                }
            }

            while (!this.brokers.All(broker => broker.IsEmpty()))
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
        }
    }
}
