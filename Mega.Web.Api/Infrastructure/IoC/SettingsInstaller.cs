namespace Mega.Web.Api.Infrastructure.IoC
{
    using Mega.Data;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class SettingsInstaller : Registry
    {
        public SettingsInstaller(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            For<DataContext>().Use(new DataContext(connectionString));
        }
    }
}
