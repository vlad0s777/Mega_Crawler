namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;

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
                        
                        r.For<IMessageBroker<UriBody>>().Singleton().Use<MessageBroker<UriBody>>();
                        
                        r.For<IMessageBroker<UriRequest>>().Singleton().Use<MessageBroker<UriRequest>>();
                        r.For<ICollection<Uri>>().Singleton().Use<HashSet<Uri>>();
                        r.Forward<IMessageBroker<UriRequest>, IMessageBroker>();
                    });

            this.Container.Configure(
                r =>
                    {
                        r.For<IMessageProcessor<UriBody>>().Use<ServiceInfoParser>()
                            .Ctor<IMessageBroker>("bodies").Is(this.Container.GetInstance<IMessageBroker<UriBody>>())
                            .Ctor<IMessageBroker>("requests").Is(this.Container.GetInstance<IMessageBroker<UriRequest>>())
                            .Ctor<Settings>("settings").Is(settings);

                        r.For<IMessageProcessor<UriRequest>>().Use<ServiceContentCollector>().Ctor<IMessageBroker>("bodies")
                            .Is(this.Container.GetInstance<IMessageBroker<UriBody>>()).Ctor<IMessageBroker>("requests")
                            .Is(this.Container.GetInstance<IMessageBroker<UriRequest>>()).Ctor<Settings>("settings").Is(settings)
                            .Ctor<Func<Uri, string>>("clientDelegate").Is(uri => new WebClient().DownloadString(uri));
                        // .Ctor<HashSet<Uri>>("visitedUrls").Is(this.Container.GetInstance<ICollection<Uri>>());  
                    });
        }
    }
}