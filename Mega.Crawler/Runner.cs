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

    public class Runner : IDisposable
    {
        private readonly ITagRequestProcessorFactory tagRequestProcessorFactory;

        private readonly IUriRequestProcessorFactory uriRequestProcessorFactory;

        private readonly IZadolbaliClientFactory zadolbaliClientFactory;

        private readonly Migrator migrator;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Settings settings;

        private readonly Win32ServiceHost win32ServiceHost;

        public Runner(Settings settings, ITagRequestProcessorFactory tagRequestProcessorFactory, IUriRequestProcessorFactory uriRequestProcessorFactory, IZadolbaliClientFactory zadolbaliClientFactory,  Migrator migrator, Win32ServiceHost win32ServiceHost)
        {
            this.settings = settings;
            this.tagRequestProcessorFactory = tagRequestProcessorFactory;
            this.uriRequestProcessorFactory = uriRequestProcessorFactory;
            this.zadolbaliClientFactory = zadolbaliClientFactory;
            this.migrator = migrator;
            this.win32ServiceHost = win32ServiceHost;
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
                this.win32ServiceHost.Run();
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