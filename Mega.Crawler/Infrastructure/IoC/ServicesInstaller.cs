namespace Mega.Crawler.Infrastructure.IoC
{
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services;
    using Mega.Services.UriRequest;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf<HashSet<string>>().Use(new HashSet<string>());

            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();

            Scan(
                s =>
                    {
                        s.AssembliesFromPath(".");
                        s.AddAllTypesOf<IMessageProcessor>();
                    });
        }
    }
}