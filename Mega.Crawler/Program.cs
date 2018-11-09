namespace Mega.Crawler
{
    using System;
    using System.IO;
    using System.Reflection;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Crawler.Jobs;
    using Mega.Services;

    using Microsoft.Extensions.Logging;

    using StructureMap;

    internal class Program
    {
        private static readonly ILogger Logger = GetLogger();

        private static ILogger GetLogger()
        {
            var logger = new LoggerFactory().AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug).CreateLogger<Program>();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => logger.LogCritical(e.ExceptionObject.ToString());
            return logger;
        }

        private static void Main()
        {
            var pathBin = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            Directory.SetCurrentDirectory(pathBin);

            var registry = new Registry();

            try
            {
                registry.IncludeRegistry<DataInstaller>();
                registry.IncludeRegistry<SettingsInstaller>();
                registry.IncludeRegistry<ServicesInstaller>();
                MessageSheduler.Start().GetAwaiter().GetResult();
                using (var container = new Container(registry))
                {
                    Logger.LogDebug(container.WhatDoIHave());
                    var runner = container.GetInstance<Runner>();
                    try
                    {
                        runner.Run().GetAwaiter().GetResult();
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