namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Mega.Services.ZadolbaliClient;

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

            var settings = new Settings(config);
            var proxySettings = new ProxySettings(config);

            ForSingletonOf<Settings>().Use(settings);
            ForSingletonOf<ProxySettings>().Use(proxySettings);
        }
    }
}