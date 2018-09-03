namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Crawler.Services;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.WebClient;

    using Microsoft.Extensions.Logging;

    using StructureMap;

    internal class Program
    {
        private static readonly ILogger Logger = GetLogger();

        private static ILogger GetLogger()
        {
            var logger = ApplicationLogging.CreateLogger<Program>();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => logger.LogCritical(e.ExceptionObject.ToString());
            return logger;
        }

        private static void Main(string[] args)
        {
            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var pathBin = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            Directory.SetCurrentDirectory(pathBin);

            var registry = new Registry();

            try
            {
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();

                using (var container = new Container(registry))
                {
                    using (var client = container.GetInstance<ProxyWebClient>())
                    //using (var client = new WebClient())
                    {
                        container.Configure(r => r.For<Func<Uri, string>>().Use((Func<Uri, string>)(uri => client.DownloadStringTaskAsync(uri))));
                        var itHappensClient = container.GetInstance<IthappensClient>();

                        container.Configure(r => r.For<Func<Uri, Uri>>().Use((Func<Uri, Uri>)(uri => itHappensClient.Handle(uri))));

                        var runner = container.GetInstance<Runner>();
                        try
                        {
                            runner.Run();

                            if (isService)
                            {
                                new Win32ServiceHost(new CrawlerService()).Run();
                            }
                        }
                        finally
                        {
                            container.Release(runner);
                            container.Release(itHappensClient);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            Logger.LogDebug("Exit Application");
        }
    }
}