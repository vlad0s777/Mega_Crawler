namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Infrastructure.IoC;
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

            if (isService)
            {
                Directory.SetCurrentDirectory("C:\\Users\\Admin\\Source\\Repos\\mega-martykhin2\\Mega.Crawler\\bin\\Release\\netcoreapp2.1\\publish");
            }

            var registry = new Registry();

            try
            {
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();

                using (var container = new Container(registry))
                {
                    using (var client = container.GetInstance<ProxyWebClient>())
                    {
                        container.Configure(r => r.For<Func<Uri, string>>().Use((Func<Uri, string>)(uri => client.DownloadString(uri))));

                        using (var itHappensClient = container.GetInstance<IthappensClient>())
                        container.Configure(r => r.For<Func<Uri, UriRequest>>().Use((Func<Uri, UriRequest>)(uri => )));

                        var runner = container.GetInstance<Runner>();
                        try
                        {
                            runner.Run();
                        }
                        finally
                        {
                            container.Release(runner);
                        }

                        if (isService)
                        {
                            var myService = new RunAsService();
                            var serviceHost = new Win32ServiceHost(myService);
                            serviceHost.Run();
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