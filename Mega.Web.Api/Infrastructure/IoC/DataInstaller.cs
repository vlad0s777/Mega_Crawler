namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Data;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class DataInstaller : Registry
    {
        public DataInstaller(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            ForConcreteType<DataContext>().Configure.Ctor<string>("connectionString").Is(connectionString);
        }
    }
}
