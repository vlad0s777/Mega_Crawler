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

                /* var pageReports = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("pageReports");
                 var pageMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>().Named("pageMessages");
                 var articleReports = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("articleReports");
                 var articleMessages = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>().Named("articleMessages");*/
                var BodyBroker = r.For<IMessageBroker>().Use<MessageBroker<UriBody>>().Named("BodyBroker");
                var LimitsBroker = r.For<IMessageBroker>().Use<MessageBroker<UriLimits>>().Named("LimitsBroker");

                r.For<IMessageProcessor>().Use<CollectContent>()
                    .Ctor<int>("limit").Is(limits.countLimit)
                    .Ctor<int>("attempt").Is(limits.attemptLimit);

                r.For<IMessageProcessor>().Use<ArticleInfoParcer>()
                    .Ctor<int>("maxdepth").Is(limits.depthLimit);

                r.For<IMessageProcessor>().Use<ArticleUrlParcer>()
                    .Ctor<int>("maxdepth").Is(limits.depthLimit);

                r.For<IMessageProcessor>().Use<CollectContent>()
                    .Ctor<int>("limit").Is(limits.countLimit)
                    .Ctor<int>("attempt").Is(limits.attemptLimit);
            });
            

        }
        
    }

}
   


