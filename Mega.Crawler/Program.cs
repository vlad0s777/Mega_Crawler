namespace Mega.Crawler
{
    using System;
    using System.Net;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

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

        private static void Main()
        {
            var registry = new Registry();
            registry.IncludeRegistry<ClassInstaller>();

            try
            {
                using (var container = new Container(registry))
                {
                    
                    ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);
                    Logger.LogInformation($"Starting with {container.GetInstance<Settings>().RootUriString}");
                    using (var client = new WebClient())
                    {
                        container.Configure(r => r.For<Func<Uri, string>>().AddInstance(uri =>
                            {
                                return client.DownloadString(uri);
                            }));
                        var runner = container.GetInstance<Runner>();
                        runner.Run();
                    }

                    Logger.LogInformation(
                        $"All {container.GetInstance<WrapperUries>().Uries.Count} urls done! "
                        + $"All {container.GetInstance<WrapperArticles>().Articles.Count} articles done!");

                    container.Release(runner);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            Console.ReadLine();
            Logger.LogDebug("Exit Application");
        }
    }
}