namespace Mega.Crawler.Infrastructure.IoC
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

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

            Scan(
                s =>
                    {
                        s.AssembliesFromPath(".");
                        s.AddAllTypesOf<IProcessorFabric>();
                    });
        }
    }
}