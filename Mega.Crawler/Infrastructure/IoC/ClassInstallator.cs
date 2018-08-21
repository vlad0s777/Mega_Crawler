namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Configuration;

    using StructureMap;

    public class ClassInstaller : Registry
    {

        public ClassInstaller()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../../Properties"))
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile($"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = new Settings(builder.Build());

            ForSingletonOf<IMessageBroker<UriBody>>().Use<MessageBroker<UriBody>>();
            ForSingletonOf<IMessageBroker<UriRequest>>().Use<MessageBroker<UriRequest>>();

            ForConcreteType<Settings>().Configure.Singleton().Ctor<Settings>("settings").Is(settings);

            ForSingletonOf<WrapperUries>();
            ForSingletonOf<WrapperArticles>();

            Forward<IMessageBroker<UriBody>, IMessageBroker>();
            Forward<IMessageBroker<UriRequest>, IMessageBroker>();

            For<IMessageProcessor>().Use<ServiceContentCollector>().Ctor<Func<Uri, string>>("clientDelegate").Is(
                uri =>
                    {
                        Thread.Sleep(new Random().Next(5000, 15000));
                        return new WebClient().DownloadString(uri);
                    });
            For<IMessageProcessor>().Use<ServiceInfoParser>();

            ForConcreteType<Runner>();
        }
    }
}