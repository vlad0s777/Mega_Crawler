namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Mega.Data;
    using Mega.Domain;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class DataInstaller : Registry
    {
        public DataInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true).AddJsonFile($"Mega.Crawler.appsettings.development.json", true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            For<IDataContext>().Use<DataContext>().Ctor<string>().Is(connectionString);
        }
    }
}