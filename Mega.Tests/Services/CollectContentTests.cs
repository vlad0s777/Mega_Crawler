using System;
using System.Collections.Generic;
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
            var collectContent = new CollectContent(messages, reports, 
                visitedUrls: new HashSet<Uri>(), 
                rootUri: new Uri("https://docs.microsoft.com/ru-ru"), 
                clientDelegate: body => "8");
            collectContent.Work();
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
            collectContent.Work();
            reports.TryReceive(out var receiveMessage1);
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
                messages.Send(new UriLimits(new Uri(childUri + i)));
            }
            var collectContent = new CollectContent(messages, reports, visitedUrls, 
                rootUri:new Uri(childUri),
                clientDelegate:uri => "8", 
                limit:6);
            collectContent.Work();
            Assert.AreNotEqual(10, visitedUrls.Count);
            Assert.AreEqual(6, visitedUrls.Count);
        }

        [Test]
        public void TrueContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var collectContent = new CollectContent(messages, reports,
                visitedUrls: new HashSet<Uri>(),
                rootUri: rootUri,
                clientDelegate: body => "8");
            collectContent.Work();
            reports.TryReceive(out var receiveMessage);
            Assert.AreEqual(rootUri, receiveMessage.Uri);
            Assert.AreEqual("8", receiveMessage.Body);
        }
    }
}