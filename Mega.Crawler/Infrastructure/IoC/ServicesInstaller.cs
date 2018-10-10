namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using Mega.Messaging;
    using Mega.Messaging.External;

    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Configuration;

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
        }
    }
}