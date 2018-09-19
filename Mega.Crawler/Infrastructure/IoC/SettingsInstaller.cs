namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

//    using Mega.Data;

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
            //var dataContextFactory = new DataContextFactory(config.GetConnectionString("DefaultConnection"));

            //For<DataContextFactory>().Use(dataContextFactory);

            ForSingletonOf<Settings>().Use(settings);
            ForSingletonOf<Services.Settings>().Use(servicesSettings);
        }
    }
}