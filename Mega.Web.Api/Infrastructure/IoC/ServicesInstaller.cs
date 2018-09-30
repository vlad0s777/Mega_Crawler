namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Data;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            For<DataContext>().Use(new DataContext(connectionString));
        }
    }
}
