using System;
using Mega.Messaging;
using Mega.Services;
using NUnit.Framework;

namespace Mega.Tests.Services
{
    [TestFixture]
    public class UrlFinderTests
    {
        [Test]
        public void AddMessage()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = "href='https://docs.microsoft.com/ru-ru/kenguru'";
            reports.Send(new UriBody(rootUri, body));
            var uriFinder = new UrlFinder(messages, reports);
            uriFinder.Work();
            Assert.IsFalse(messages.IsEmpty());
            Assert.IsTrue(reports.IsEmpty());
        }

        [Test]
        public void DepthTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = "csdcdscdscsdhref='https://docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf";
            var sendMessage = new UriBody(rootUri, body);
            reports.Send(sendMessage);
            var limit = 3;
            var uriFinder = new UrlFinder(messages, reports, limit);
            uriFinder.Work();
            messages.TryReceive(out var checkDepth);
            Assert.AreEqual(checkDepth.Depth, 2);
        }

        [Test]
        public void FalseUriTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = "csdcdscdscsdhref='http:/docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf";
            var sendMessage = new UriBody(rootUri, body);
            reports.Send(sendMessage);
            var uriFinder = new UrlFinder(messages, reports);
            uriFinder.Work();
            Assert.IsFalse(messages.TryReceive(out var receiveMessage));
        }

        [Test]
        public void TrueUriTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var rootUri = new Uri("https://docs.microsoft.com/ru-ru");
            var body = "csdcdscdscsdhref='https://docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf";
            var checkUrl = "https://docs.microsoft.com/ru-ru/kenguru";
            var sendMessage = new UriBody(rootUri, body);
            reports.Send(sendMessage);
            var uriFinder = new UrlFinder(messages, reports);
            uriFinder.Work();
            messages.TryReceive(out var receiveMessage);
            Assert.AreEqual(receiveMessage.Uri, new Uri(checkUrl));
        }
    }
}