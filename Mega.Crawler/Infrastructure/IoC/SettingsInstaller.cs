namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Mega.Services;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.development.json", true); // переменную окружения служба почему то не видит, пришлось написать так

            var settings = new Settings(builder.Build());

            ForSingletonOf<Settings>().Use(settings);
        }
    }
}