namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.Net;
    using System.Threading;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using StructureMap;

    public class ClassInstallator
    {
        public IContainer Container { get; set; }

        public void InstallClass(Settings settings)
        {
            this.Container.Configure(
                r =>
                    {
                        r.ForConcreteType<MessageBroker<UriBody>>().Configure.Singleton();
                        r.ForConcreteType<MessageBroker<UriRequest>>().Configure.Singleton();

                        r.ForConcreteType<Settings>().Configure.Singleton().Ctor<Settings>("settings").Is(settings);

                        r.ForConcreteType<WrapperUries>().Configure.Singleton();
                        r.ForConcreteType<WrapperArticles>().Configure.Singleton();

                        r.Forward<MessageBroker<UriBody>, IMessageBroker>();
                        r.Forward<MessageBroker<UriRequest>, IMessageBroker>();
                    });
            this.Container.Configure(
                r =>
                    {
                        r.For<IMessageProcessor>().Use<ServiceContentCollector>()
                            .Ctor<Func<Uri, string>>("clientDelegate").Is(
                                uri =>
                                    {
                                        Thread.Sleep(new Random().Next(5000, 15000));
                                        return new WebClient().DownloadString(uri);
                                    });
                        r.For<IMessageProcessor>().Use<ServiceInfoParser>();
                    });
        }
    }
}