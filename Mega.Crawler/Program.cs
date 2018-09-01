﻿namespace Mega.Crawler
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

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

        private static void Main()
        {
            ApplicationLogging.LoggerFactory.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);

            Directory.SetCurrentDirectory("C:\\Users\\Admin\\Source\\Repos\\mega-martykhin2\\Mega.Crawler\\bin\\Release\\netcoreapp2.1\\publish");

            var registry = new Registry();

            try
            {
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();

                var random = new Random();

                using (var container = new Container(registry))
                {
                    using (var client = new WebClient())
                    {
                        container.Configure(
                            r => r.For<Func<Uri, string>>().Use(
                                (Func<Uri, string>)(uri =>
                                                           {
                                                               Thread.Sleep(random.Next(5000, 15000));
                                                               return client.DownloadString(uri);
                                                           })));


                        var runner = container.GetInstance<Runner>();
                        try
                        {
                            runner.Run();
                        }
                        finally
                        {
                            container.Release(runner);
                        }

                        var myService = new RunAsService();
                        var serviceHost = new Win32ServiceHost(myService);
                        serviceHost.Run();
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