namespace Mega.Crawler.Infrastructure.IoC
{
    using Mega.Data;
    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services.UriRequest;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();

            For<IDataContext>().Use<DataContext>();

            Scan(
                s =>
                    {
                        s.AssembliesFromPath(".");
                        s.AddAllTypesOf<IProcessorFactory>();
                    });
        }
    }
}