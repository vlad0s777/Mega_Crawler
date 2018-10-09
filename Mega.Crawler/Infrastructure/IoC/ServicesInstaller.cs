namespace Mega.Crawler.Infrastructure.IoC
{
    using Mega.Messaging;
    using Mega.Messaging.External;

    using Mega.Services.UriRequest;
    using Mega.WebClient.ZadolbaliClient;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();

            ForSingletonOf<IProcessorFactory>().Use<UriRequestProcessorFactory>();
                        Scan(
                            s =>
                                {
                                    s.AssembliesFromPath(".");
                                    s.AddAllTypesOf<IProcessorFactory>();
                                });

            ForConcreteType<ZadolbaliClient>().Configure.SelectConstructor(() => new ZadolbaliClient(null));
        }
    }
}