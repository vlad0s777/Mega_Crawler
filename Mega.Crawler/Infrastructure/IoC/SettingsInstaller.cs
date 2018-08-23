namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.IO;

    using Mega.Services;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(@"../../../Properties"))
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true).AddJsonFile(
                    $"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json",
                    true);

            var settings = new Settings(builder.Build());

            ForSingletonOf<Settings>().Use(settings);
        }
    }
}