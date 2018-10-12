namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.development.json", true); 

            var config = builder.Build();

            ForSingletonOf<Settings>().Use(new Settings(config));
        }
    }
}