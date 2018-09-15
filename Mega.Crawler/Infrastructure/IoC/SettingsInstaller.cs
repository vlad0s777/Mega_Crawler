namespace Mega.Crawler.Infrastructure.IoC
{
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.development.json", true); // переменную окружения служба почему то не видит, пришлось написать так

            var config = builder.Build();
            
            var settings = new Settings(config);
            var servicesSettings = new Services.Settings(config);

            ForSingletonOf<Settings>().Use(settings);
            ForSingletonOf<Services.Settings>().Use(servicesSettings);
        }
    }
}