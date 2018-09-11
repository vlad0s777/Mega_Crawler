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
            if (this.brokers.All(broker => broker.IsEmpty()))
            {
                this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(string.Empty));                        
            }

            foreach (var messageProcessor in this.processors)
            {
                messageProcessor.Run();
            }
        }
    }
}
