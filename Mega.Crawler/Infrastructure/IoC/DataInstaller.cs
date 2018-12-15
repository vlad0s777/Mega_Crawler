namespace Mega.Crawler.Infrastructure.IoC
{
    using System.Data;
    using System.IO;

    using Mega.Data.Migrations;
    using Mega.Data.Repositories;
    using Mega.Domain.Repositories;

    using Microsoft.Extensions.Configuration;

    using Npgsql;

    using StructureMap;

    public class DataInstaller : Registry
    {
        public DataInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true).AddJsonFile($"Mega.Crawler.appsettings.development.json", true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            For<IDbConnection>().Use<NpgsqlConnection>().Ctor<string>().Is(connectionString);

            ForConcreteType<Migrator>();
      
            Scan(y =>
                {
                    y.Assembly(typeof(TagRepository).Assembly.GetName().Name);
                    y.WithDefaultConventions();
                    y.ConnectImplementationsToTypesClosing(typeof(IRepository<>));                   
                });
        }
    }
}