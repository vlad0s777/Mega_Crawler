namespace Mega.Crawler
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Services;

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

            var pathBin = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            Directory.SetCurrentDirectory(pathBin);

            var registry = new Registry();

            try
            {
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();

                using (var container = new Container(registry))
                {
                    var runner = container.GetInstance<Runner>();
                    try
                    {
                        runner.Run();
                        if (!(Debugger.IsAttached || args.Contains("--console")))
                        {
                            new Win32ServiceHost(new CrawlerService()).Run();
                        }
                        else
                        {
                            Console.ReadLine();
                        }
                    }
                    finally
                    {
                        container.Release(runner);
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