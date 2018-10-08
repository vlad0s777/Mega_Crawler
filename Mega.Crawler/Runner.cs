namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.UriRequest;

    using Microsoft.Extensions.Logging;

    public class Runner : IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Runner>();

        private readonly IMessageBroker[] brokers;

        private readonly IProcessorFactory processorFabric;

        private readonly IDataContext dataContext;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Initial initial;

        public Runner(IMessageBroker[] brokers, IProcessorFactory processorFabric, Initial initial, IDataContext dataContext)
        {
            this.brokers = brokers;
            this.processorFabric = processorFabric;
            this.initial = initial;
            this.dataContext = dataContext;
        }
        
        public async Task Run()
        {
            var token = this.cts.Token;

            if (Environment.GetCommandLineArgs().Contains("--migrate"))
            {
                this.dataContext.Migrate();
                return;
            }

            await this.initial.AddTagInBase();

            if (this.brokers.All(broker => broker.IsEmpty()))
            {               
                foreach (var id in this.initial.GenerateIDs(new DateTime(2009, 9, 8)))
                {
                    this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(id));
                }
            }

            Logger.LogInformation($"Popular tags: {(this.dataContext.GetPopularTags(2).First()).TagKey}, {(this.dataContext.GetPopularTags(2).Last()).TagKey}");

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
