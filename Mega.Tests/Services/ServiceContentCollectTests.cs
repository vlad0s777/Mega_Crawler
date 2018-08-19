namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using Mega.Crawler.Infrastructure.IoC;
    using Mega.Messaging;
    using Mega.Services;

    using NUnit.Framework;

    [TestFixture]
    public class ServiceContentCollectTests
    {
        [Test]
        public void AddReports()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            messages.Send(new UriLimits(rootUri));

            new ServiceContentCollect(
                messages,
                reports,
                visitedUrls: new HashSet<Uri>(),
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Run();

            Assert.IsTrue(messages.IsEmpty());
            Assert.IsFalse(reports.IsEmpty());
        }

        [Test]
        public void FalseContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            messages.Send(new UriLimits(rootUri));

            var contentCollect = new ServiceContentCollect(
                messages,
                reports,
                visitedUrls: new HashSet<Uri>(),
                clientDelegate: body => "8",
                settings: new Settings(rootUri));

            messages.Send(new UriLimits("http://someurl"));

            contentCollect.Run();

            Assert.IsTrue(reports.TryReceive(out var receiveMessage1));
            Assert.IsFalse(reports.TryReceive(out var receiveMessage2));
        }

        [Test]
        public void LimitTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var visitedUrls = new HashSet<Uri>();

            var rootUri = "https://docs.microsoft.com/ru-ru/";
            
            for (var i = 0; i < 10; i++)
            {
                messages.Send(new UriLimits(rootUri + i));
            }

            messages.Send(new UriLimits(rootUri));

            var colCon = new ServiceContentCollect(
                messages,
                reports,
                visitedUrls,
                clientDelegate: uri => "8", 
                settings: new Settings(rootUri, countLimit: 6));

            while (!messages.IsEmpty())
            {
                colCon.Run();
            }
         
            Assert.AreEqual(6, visitedUrls.Count);
        }

        [Test]
        public void TrueContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            messages.Send(new UriLimits(rootUri));
            new ServiceContentCollect(
                messages,
                reports,
                visitedUrls: new HashSet<Uri>(),
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Run();
           
            Assert.IsTrue(reports.TryReceive(out var receiveMessage));
            Assert.AreEqual(rootUri, receiveMessage.Uri.AbsoluteUri);
            Assert.AreEqual("8", receiveMessage.Body);
        }

        [Test]
        public void ContainerTest()
        {
            var rootUri = "https://docs.microsoft.com/ru-ru";
            var container = new InstallClass(new Settings(rootUri)).Container;
            var reports = container.GetInstance<IMessageBroker>("reports");
            var messages = (IMessageBroker<UriLimits>)container.GetInstance<IMessageBroker>("messages");
            messages.Send(new UriLimits(rootUri));
            var visitedUrls = new HashSet<Uri>();
            container.With(visitedUrls).GetInstance<IMessageProcessor>("ServiceContentCollect").Run();
            Assert.IsTrue(messages.IsEmpty());
            Assert.IsFalse(reports.IsEmpty());
        }
    }
}