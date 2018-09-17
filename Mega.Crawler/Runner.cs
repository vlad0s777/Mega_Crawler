namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Messaging;
    using Mega.Services.UriRequest;

    public class Runner
    {
        private readonly IMessageBroker[] brokers;

        private readonly IProcessorFabric processorFabric;

        public Runner(IMessageBroker[] brokers, IProcessorFabric processorFabric)
        {
            this.brokers = brokers;
            this.processorFabric = processorFabric;
        }

        public static IEnumerable<string> GenerateIDs(DateTime start)
        {
            var current = DateTime.Now;
            while (current >= start)
            {
                yield return current.Date.ToString("yyyyMMdd");
                current = current.AddDays(-1);
            }
        }

        public void Run()
        {
            if (this.brokers.All(broker => broker.IsEmpty()))
            {
                foreach (var id in GenerateIDs(new DateTime(2009, 9, 8)))
                {
                    this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(id));
                }
            }

            foreach (var processor in this.processorFabric.Create())
            {
                processor.Run();
            }
        }
    }
}
