using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mega.Crawler.Infrastructure.IoC;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    public class CollectContentTests
    {
        [Test]
        public void AddReports()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            new CollectContent(messages, reports,
                visitedUrls: new HashSet<Uri>(),
                rootUri: new Uri("https://docs.microsoft.com/ru-ru"),
                clientDelegate: body => "8").Run();

            Assert.IsTrue(messages.IsEmpty());
            Assert.IsFalse(reports.IsEmpty());
        }

        [Test]
        public void FalseContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var collectContent = new CollectContent(messages, reports,
                visitedUrls: new HashSet<Uri>(),
                rootUri: new Uri("https://docs.microsoft.com/ru-ru"),
                clientDelegate: body => "8");

            messages.Send(new UriLimits("http://someurl"));

            collectContent.Run();

            Assert.IsTrue(reports.TryReceive(out var receiveMessage1));
            Assert.IsFalse(reports.TryReceive(out var receiveMessage2));
        }

        [Test]
        public void LimitTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var visitedUrls = new HashSet<Uri>();

            var childUri = "https://docs.microsoft.com/ru-ru/";

            for (var i = 0; i < 10; i++)
            {
                messages.Send(new UriLimits(childUri+i));
            }

            new CollectContent(messages, reports, visitedUrls, 
                rootUri:new Uri(childUri),
                clientDelegate:uri => "8", 
                limit:6).Run();
         
            Assert.AreEqual(6, visitedUrls.Count);
        }

        [Test]
        public void TrueContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");

            new CollectContent(messages, reports,
                visitedUrls: new HashSet<Uri>(),
                rootUri: rootUri,
                clientDelegate: body => "8").Run();
           
            Assert.IsTrue(reports.TryReceive(out var receiveMessage));
            Assert.AreEqual(rootUri, receiveMessage.Uri);
            Assert.AreEqual("8", receiveMessage.Body);
        }

        [Test]
        public void ContainerTest()
        {
          
            var cont = new InstallClass(new Limitations(2,100,3)).Container;
            //var reports = (MessageBroker<UriBody>) cont.GetInstance<IMessageBroker>("uribody");
            //var messages = (MessageBroker<UriLimits>)cont.GetInstance<IMessageBroker>("urilimits");
            //cont.GetAllInstances<IMessageBroker>();
            var messages = cont.GetInstance<IMessageBroker>("pageMessages");
            var reports = (MessageBroker<UriBody>)cont.GetInstance<IMessageBroker>("pageReports");
            var proc = cont/*.With(messages).With(reports)*/.With(new HashSet<Uri>()).With(new Uri("https://docs.microsoft.com/ru-ru"))
                .With(new Func<Uri, string>(uri =>"8" )).GetInstance<IMessageProcessor>();
            proc.Run();
            
            Assert.IsTrue(messages.IsEmpty());
            Assert.IsFalse(reports.IsEmpty());
            Assert.IsTrue(reports.TryReceive(out var receiveMessage));
            Assert.AreEqual(new Uri("https://docs.microsoft.com/ru-ru"), receiveMessage.Uri);
            Assert.AreEqual("8", receiveMessage.Body);
        }
    }
}