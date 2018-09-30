namespace Mega.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Messaging;
    using Mega.Services.UriRequest;

    public class Runner : IDisposable
    {
        private readonly IMessageBroker[] brokers;

        private readonly IProcessorFactory processorFabric;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public Runner(IMessageBroker[] brokers, IProcessorFactory processorFabric)
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
            var token = this.cts.Token;

            if (this.brokers.All(broker => broker.IsEmpty()))
            {
                foreach (var id in GenerateIDs(new DateTime(2009, 9, 8)))
                {
                    this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(id));
                }
            }

            foreach (var processor in this.processorFabric.Create())
            {
                processor.Run(token);
            }

            if (!(Debugger.IsAttached || Environment.GetCommandLineArgs().Contains("--console")))
            {
                new Win32ServiceHost(new CrawlerService()).Run();
            }
            else
            {
                Console.ReadLine();
                this.cts.Cancel();
            }
        }

        public void Dispose()
        {
            this.cts?.Dispose();
        }
    }
}
