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


    public class GenericTypes
    {
        public IContainer Container;

        public GenericTypes()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(@"../../../Properties"))
                .AddJsonFile("Mega.Crawler.appsettings.json", false, true)
                .AddJsonFile(
                    $"Mega.Crawler.appsettings.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT")}.json", true);

            var settings = builder.Build();
            this.Container = new Container();
            this.Container.Configure(r => r
            .For(typeof(IMessageBroker<>))
            .Use(typeof(MessageBroker<>))
            .Named("MessageBroker"));
            /*this.Container.Configure(r =>
                r.Scan(s =>
                {
                    //s.AssemblyContainingType<Steak>();
                    s.AddAllTypesOf<IMessageProcessor>();
                }));*/
            this.Container.Configure(r => r
                .For<IMessageProcessor>()
                .Use<CollectContent>()
                /*.Ctor<MessageBroker<UriLimits>>("messages")
                .IsNamedInstance("MessageBroker")
                .Ctor<MessageBroker<UriBody>>("reports")
                .IsNamedInstance("MessageBroker")
                .Ctor<HashSet<Uri>>("visitedUrls")
                .IsTheDefault()*/
            );
        }
        
    }

}
   


