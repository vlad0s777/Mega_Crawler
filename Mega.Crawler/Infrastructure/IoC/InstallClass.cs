namespace Mega.Crawler.Infrastructure.IoC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Text;

    using Mega.Messaging;
    using Mega.Services;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StructureMap;

    public class InstallClass
    {
        public IContainer Container { get; }

        public InstallClass(Settings settings)
        {
            this.Container = new Container();
            this.Container.Configure(
                r =>
                    {
                        r.For<IMessageBroker>().Singleton().Use<MessageBroker<UriBody>>().Named("reports");
                        r.For<IMessageBroker>().Singleton().Use<MessageBroker<UriLimits>>().Named("messages");                       
                    });
            this.Container.Configure(
                r =>
                    {
                        r.For<IMessageProcessor>().Use<ServiceInfoParser>().Ctor<IMessageBroker>("reports")
                            .Is(this.Container.GetInstance<IMessageBroker>("reports")).Ctor<IMessageBroker>("messages")
                            .Is(this.Container.GetInstance<IMessageBroker>("messages")).Ctor<Settings>("settings").Is(settings)
                            .Named("ServiceInfoParser");

                        r.For<IMessageProcessor>().Use<ServiceContentCollect>()
                            .Ctor<IMessageBroker>("reports").Is(this.Container.GetInstance<IMessageBroker>("reports"))
                            .Ctor<IMessageBroker>("messages").Is(this.Container.GetInstance<IMessageBroker>("messages"))
                            .Ctor<Settings>("settings").Is(settings)
                            .Ctor<Func<Uri, string>>("clientDelegate").Is(uri => new WebClient().DownloadString(uri))
                            .Named("ServiceContentCollect");
                    });

            //                r.Scan(
            //                    s =>
            //                        {
            //                            s.AddAllTypesOf<IMessageProcessor>().NameBy(t => t.Name);
            //                            s.Include(t => t.Name.StartsWith("Service"));
            //                        });
        }
    }

}
   


