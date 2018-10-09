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
    using Mega.Services.UriRequest;
    using Mega.WebClient.ZadolbaliClient;

    public class Runner : IDisposable
    {
        private readonly IMessageBroker[] brokers;

        private readonly IProcessorFactory processorFabric;

        private readonly IDataContext dataContext;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly ZadolbaliClient client;

        public Runner(IMessageBroker[] brokers, IProcessorFactory processorFabric, IDataContext dataContext, ZadolbaliClient client)
        {
            this.brokers = brokers;
            this.processorFabric = processorFabric;
            this.dataContext = dataContext;
            this.client = client;
        }
        
        public async Task Run()
        {
            var token = this.cts.Token;

            if (Environment.GetCommandLineArgs().Contains("--migrate"))
            {
                this.dataContext.Migrate();
                return;
            }

            if (await this.dataContext.CountTags() == 0)
            {
                foreach (var tag in await this.client.GetTags())
                {
                    await this.dataContext.AddAsync(new Tag { Name = tag.Name, TagKey = tag.TagKey }, token);
                }
            }

            await this.dataContext.SaveChangesAsync(token);

            if (this.brokers.All(broker => broker.IsEmpty()))
            {               
                foreach (var id in await this.client.GenerateIDs())
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
            this.client?.Dispose();
        }
    }
}
