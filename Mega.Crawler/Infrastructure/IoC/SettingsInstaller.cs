namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Microsoft.Extensions.Configuration;

    using StructureMap;
    using StructureMap.Pipeline;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.development.json", true); 

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var settings = new Settings(config);
            var servicesSettings = new Services.Settings(config);

            For<IDataContext>().Use(new DataContext(connectionString));

            ForSingletonOf<Settings>().Use(settings);
            ForSingletonOf<Services.Settings>().Use(servicesSettings);
        }
    }
}