using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using Mega.Messaging;
using Mega.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace Mega.Crawler.Infrastructure.IoC
{


    public class InstallClass
    {
        public IContainer Container;

        public InstallClass(Limitations limits)
        {
            this.Container = new Container();
            this.Container.Configure(r =>
            {
                /* r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("pageReports");
                 var pageMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>();
                 var articleReports = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>();
                 var articleMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>();

                 r.For<IMessageProcessor>().Use<CollectContent>()
                     .Ctor<IMessageBroker>("pageMessages").Is(pageMessages)
                     .Ctor<IMessageBroker>("pageReports").Is(i => i.)
                     .Ctor<HashSet<Uri>>("visitedUrls").Is(uries)
                     .Ctor<string>("rootUri").Is(rootUri)
                     .Ctor<Func<Uri, string>>("clientDelegate").Is(clientDelegate)
                     ;*/
       
                var pageReports = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("pageReports");
                var pageMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>().Named("pageMessages");
                var articleReports = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("articleReports");
                var articleMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>().Named("articleMessages");

                r.For<IMessageProcessor>().Use<CollectContent>()
                    .Ctor<IMessageBroker>("messages").Is(pageMessages)
                    .Ctor<IMessageBroker>("reports").Is(pageReports)
                    .Ctor<int>("limit").Is(limits.countLimit)
                    .Ctor<int>("attempt").Is(limits.attemptLimit);
            });
            

        }
        
    }

}
   


