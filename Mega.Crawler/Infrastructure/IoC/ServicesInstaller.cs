namespace Mega.Crawler.Infrastructure.IoC
{
    using System;

    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using StructureMap;
    using StructureMap.AutoFactory;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();
            Forward<IMessageBroker<string>, IMessageBroker>();

            var random = new Random();
            ForConcreteType<ZadolbaliClient>().Configure.Ctor<int>("seed").Is(random.Next());

            For<IMessageProcessor<UriRequest>>().Use<UriRequestProcessor>();
            For<IMessageProcessor<string>>().Use<TagRequestProcessor>();

            For<IUriRequestProcessorFactory>().CreateFactory();
            For<ITagRequestProcessorFactory>().CreateFactory();
        }
    }
}