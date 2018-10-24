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

            For<ISomeReportDataProvider>().Use<SomeReportDataProvider>().Ctor<string>().Is(connectionString);

            Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.ConnectImplementationsToTypesClosing(typeof(IMapper<,>));
                });
        }
    }
}
