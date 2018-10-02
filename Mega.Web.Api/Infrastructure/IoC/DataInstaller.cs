namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Data;
    using Mega.Domain;
    using Mega.Web.Api.Mappers;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class DataInstaller : Registry
    {
        public DataInstaller(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            For<IDataContext>().Use(new DataContext(connectionString));

            Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.ConnectImplementationsToTypesClosing(typeof(IMapper<,>));
                });
        }
    }
}
