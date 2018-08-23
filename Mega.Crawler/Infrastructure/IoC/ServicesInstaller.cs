namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using StructureMap;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf<HashSet<Uri>>().Use(new HashSet<Uri>());
            ForSingletonOf<Dictionary<string, ArticleInfo>>().Use(new Dictionary<string, ArticleInfo>());

            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriBody>, IMessageBroker>();
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