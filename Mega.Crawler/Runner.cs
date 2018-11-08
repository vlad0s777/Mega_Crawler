namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Data.Migrations;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    public class Runner : IDisposable
    {
        private readonly ILoggerFactory loggerFactory;

        private readonly ITagRequestProcessorFactory tagRequestProcessorFactory;

        private readonly IUriRequestProcessorFactory uriRequestProcessorFactory;

        private readonly IZadolbaliClientFactory zadolbaliClientFactory;

        private readonly Migrator migrator;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Settings settings;

        public Runner(Settings settings, ITagRequestProcessorFactory tagRequestProcessorFactory, IUriRequestProcessorFactory uriRequestProcessorFactory, IZadolbaliClientFactory zadolbaliClientFactory, ILoggerFactory loggerFactory, Migrator migrator)
        {
            this.settings = settings;
            this.tagRequestProcessorFactory = tagRequestProcessorFactory;
            this.uriRequestProcessorFactory = uriRequestProcessorFactory;
            this.zadolbaliClientFactory = zadolbaliClientFactory;
            this.loggerFactory = loggerFactory;
            this.migrator = migrator;
        }
        
        public async Task Run()
        {
            var isService = !(Debugger.IsAttached || Environment.GetCommandLineArgs().Contains("--console"));

            var token = this.cts.Token;

            if (Environment.GetCommandLineArgs().Contains("--migrate"))
            {
                await this.migrator.Migrate();
                return;
            }

            var random = new Random();
            foreach (var proxy in this.settings.ProxyServers)
            {
                var client = this.zadolbaliClientFactory.Create(proxy, random.Next());
                this.uriRequestProcessorFactory.Create(client).Run(token);
                this.tagRequestProcessorFactory.Create(client).Run(token);
            }

            if (isService)
            {
                new Win32ServiceHost(new CrawlerService(this.loggerFactory)).Run();
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