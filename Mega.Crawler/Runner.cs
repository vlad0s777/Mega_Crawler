namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Shedules;
    using Mega.Migrations;
    using Mega.Services;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    using Quartz;
    using Quartz.Logging;

    public class Runner : IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Runner>();

        private readonly ITagRequestProcessorFactory tagRequestProcessorFactory;

        private readonly IUriRequestProcessorFactory uriRequestProcessorFactory;

        private readonly IZadolbaliClientFactory zadolbaliClientFactory;

        private readonly Migrator migrator;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Settings settings;

        private readonly Win32ServiceHost win32ServiceHost;

        private readonly ISchedulerFactory shedFactory;

        public Runner(
            Settings settings,
            ITagRequestProcessorFactory tagRequestProcessorFactory,
            IUriRequestProcessorFactory uriRequestProcessorFactory,
            IZadolbaliClientFactory zadolbaliClientFactory,
            Migrator migrator,
            Win32ServiceHost win32ServiceHost,
            ISchedulerFactory shedFactory)
        {
            this.settings = settings;
            this.tagRequestProcessorFactory = tagRequestProcessorFactory;
            this.uriRequestProcessorFactory = uriRequestProcessorFactory;
            this.zadolbaliClientFactory = zadolbaliClientFactory;
            this.migrator = migrator;
            this.win32ServiceHost = win32ServiceHost;
            this.shedFactory = shedFactory;
        }
        
        public async Task Run()
        {
            var isService = !(Debugger.IsAttached || Environment.GetCommandLineArgs().Contains("--console"));

            var token = this.cts.Token;

            if (Environment.GetCommandLineArgs().Contains("--migrate"))
            {
                Logger.LogInformation(await this.migrator.Migrate());
                return;
            }
            
            var random = new Random();
            
            foreach (var proxy in this.settings.ProxyServers)
            {
                var client = this.zadolbaliClientFactory.Create(proxy, random.Next());
                this.uriRequestProcessorFactory.Create(client).Run(token);
                this.tagRequestProcessorFactory.Create(client).Run(token);
            }

            LogProvider.SetCurrentLogProvider(new ConsoleEventILogProvider());

            try
            {
                var scheduler = await this.shedFactory.GetScheduler(token);
                await scheduler.Start(token);
            }
            catch (SchedulerException se)
            {
                Logger.LogError(se.Message + " " + se.StackTrace);
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