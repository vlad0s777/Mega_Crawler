namespace Mega.Crawler.Infrastructure.IoC
{
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    using StructureMap;
    using StructureMap.AutoFactory;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            ForSingletonOf(typeof(IMessageBroker<>)).Use(typeof(RabbitMqMessageBroker<>));

            Forward<IMessageBroker<UriRequest>, IMessageBroker>();
            Forward<IMessageBroker<string>, IMessageBroker>();

            ForConcreteType<ZadolbaliClient>();
            For<IZadolbaliClientFactory>().CreateFactory();

            For<IMessageProcessor<UriRequest>>().Use<UriRequestProcessor>();
            For<IMessageProcessor<string>>().Use<TagRequestProcessor>();

            For<IUriRequestProcessorFactory>().CreateFactory();
            For<ITagRequestProcessorFactory>().CreateFactory();

            ForSingletonOf<ILoggerFactory>().Use(new LoggerFactory().AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug));
        }
    }
}