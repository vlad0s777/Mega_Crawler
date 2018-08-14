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
            var visitedUrls = new HashSet<Uri>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var collectContent = new CollectContent(messages, reports,
                visitedUrls, rootUri, body => "8");
            collectContent.Work();
            Assert.IsTrue(messages.IsEmpty() && !reports.IsEmpty());
        }

        [Test]
        public void FalseContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var visitedUrls = new HashSet<Uri>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var collectContent = new CollectContent(messages, reports,
                visitedUrls, rootUri, uri => "8");
            var someUrl = new Uri("http://someurl");
            messages.Send(new UriLimits(someUrl));
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
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var childUri = "https://docs.microsoft.com/ru-ru/";
            var limit = 6;
            var total = 10;
            for (var i = 0; i < total; i++)
            {
                messages.Send(new UriLimits(new Uri(childUri + i)));
            }

            var collectContent = new CollectContent(messages, reports,
                visitedUrls, rootUri, uri => "8", limit);
            while (!messages.IsEmpty())
            {
                collectContent.Work();
            }

            Assert.AreNotEqual(visitedUrls.Count, total);
            Assert.AreEqual(visitedUrls.Count, limit);
        }

        [Test]
        public void TrueContentTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var visitedUrls = new HashSet<Uri>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var collectContent = new CollectContent(messages, reports,
                visitedUrls, rootUri, uri => "8");
            collectContent.Work();
            reports.TryReceive(out var receiveMessage);
            Assert.AreEqual(receiveMessage.Uri, rootUri);
            Assert.AreEqual(receiveMessage.Body, "8");
        }
    }
}