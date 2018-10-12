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
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;

    public class Runner : IDisposable
    {
        private readonly IMessageBroker[] brokers;

        private readonly ITagRequestProcessorFactory tagRequestProcessorFactory;

        private readonly IUriRequestProcessorFactory uriRequestProcessorFactory;

        private readonly IDataContext dataContext;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Settings settings;

        public Runner(IMessageBroker[] brokers, IDataContext dataContext, Settings settings, ITagRequestProcessorFactory tagRequestProcessorFactory, IUriRequestProcessorFactory uriRequestProcessorFactory)
        {
            this.brokers = brokers;
            this.dataContext = dataContext;
            this.settings = settings;
            this.tagRequestProcessorFactory = tagRequestProcessorFactory;
            this.uriRequestProcessorFactory = uriRequestProcessorFactory;
        }
        
        public async Task Run()
        {
            var token = this.cts.Token;

            if (Environment.GetCommandLineArgs().Contains("--migrate"))
            {
                this.dataContext.Migrate();
                return;
            }

            var tagsCount = await this.dataContext.CountTags();
            var isEmptyQueues = this.brokers.All(broker => broker.IsEmpty());

            if (tagsCount == 0 && isEmptyQueues)
            {
                this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest("tags"));
            }
            else if (isEmptyQueues)
            {
                this.brokers.OfType<IMessageBroker<UriRequest>>().First().Send(new UriRequest(string.Empty));
            }
            else if (tagsCount == 0)
            {
                this.brokers.OfType<IMessageBroker<string>>().First().Send("tags");
            }

            foreach (var proxy in this.settings.ProxyServers)
            {
                this.uriRequestProcessorFactory.Create(proxy).Run(token);
                this.tagRequestProcessorFactory.Create(proxy).Run(token);
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