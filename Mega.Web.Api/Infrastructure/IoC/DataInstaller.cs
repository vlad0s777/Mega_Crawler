namespace Mega.Web.Api.Infrastructure.IoC
{
    using System.Data;

    using Mega.Domain.Repositories;
    using Mega.Web.Api.Mappers;

    using Microsoft.Extensions.Configuration;

    using Npgsql;

    using StructureMap;

    public class DataInstaller : Registry
    {
        public DataInstaller(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            For<IDbConnection>().Use<NpgsqlConnection>().Ctor<string>().Is(connectionString);

            Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.ConnectImplementationsToTypesClosing(typeof(IMapper<,>));
                });

            Scan(y =>
                {
                    y.Assembly("Mega.Data");
                    y.WithDefaultConventions();
                    y.ConnectImplementationsToTypesClosing(typeof(IRepository<>));
                });
        }
    }
}
