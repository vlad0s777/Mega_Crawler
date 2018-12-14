namespace Mega.Crawler.Infrastructure.IoC
{
    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Shedules;
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    using Quartz;
    using Quartz.Spi;

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

            ForSingletonOf<ILoggerFactory>().Use<LoggerFactory>().SetProperty(x => x.AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug));
            For<IWin32Service>().Use<CrawlerService>();
            ForConcreteType<Win32ServiceHost>();

            For<IJobFactory>().Use<StructureMapJobFactory>();

            For<ISchedulerFactory>().Use<StructureMapShedulerFactory>();
        }
    }
}