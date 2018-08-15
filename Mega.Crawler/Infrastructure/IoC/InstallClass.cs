using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using Mega.Messaging;
using Mega.Services;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace Mega.Crawler.Infrastructure.IoC
{

    public interface IClass { }

    public interface IClass1 : IClass { }

    public interface IClass2 : IClass { }

    public class Class1 : IClass1 { }

    public class Class2 : IClass2 { }
    public class RegisterByScanWithNaming
    {
        public IContainer Container;

        public RegisterByScanWithNaming()
        {
            this.Container = new Container(x => x.Scan(s =>
            {
                s.AddAllTypesOf(typeof(IClass)).NameBy(t => t.Name);
                s.AssembliesFromPath(".");
                s.WithDefaultConventions();
                s.LookForRegistries();
            }));
        }
    }

}
   


