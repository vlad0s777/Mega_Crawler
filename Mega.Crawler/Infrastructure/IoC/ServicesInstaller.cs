namespace Mega.Crawler.Infrastructure.IoC
{
    using System.IO;

    using DasMulli.Win32.ServiceUtils;

    using Mega.Crawler.Jobs;
    using Mega.Messaging;
    using Mega.Messaging.External;
    using Mega.Services;
    using Mega.Services.TagRequest;
    using Mega.Services.UriRequest;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using StructureMap;
    using StructureMap.AutoFactory;

    public class ServicesInstaller : Registry
    {
        public ServicesInstaller()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory() + "\\Properties")
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true).AddJsonFile($"Mega.Crawler.appsettings.development.json", true);

            var config = builder.Build();

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

            var cronExpression = config.GetSection("CronExpression").Value;
            ForConcreteType<MessageSheduler>().Configure.SetProperty(x => x.CronExpression = cronExpression);
        }
    }
}