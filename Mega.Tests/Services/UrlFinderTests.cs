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
            reports.Send(new UriBody(
                uri: new Uri("https://docs.microsoft.com/ru-ru"), 
                body: "href='https://docs.microsoft.com/ru-ru/kenguru'"));
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

            var uriBody = new UriBody(
                uri: new Uri("https://docs.microsoft.com/ru-ru"), 
                body: "csdcdscdscsdhref='https://docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf");

            reports.Send(uriBody);

            new UrlFinder(messages, reports, checkDepth: 3).Work();

            Assert.IsTrue(messages.TryReceive(out var uriLimits));
            Assert.AreEqual(2, uriLimits.Depth);
        }

        [Test]
        public void FalseUriTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            reports.Send(new UriBody(
                uri: new Uri("https://docs.microsoft.com/ru-ru"),
                body: "csdcdscdscsdhref='http:/docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf"));
            var uriFinder = new UrlFinder(messages, reports);
            uriFinder.Work();
            Assert.IsFalse(messages.TryReceive(out var receiveMessage));
        }

        [Test]
        public void TrueUriTest()
        {
            var reports = new MessageBroker<UriBody>();
            var messages = new MessageBroker<UriLimits>();
            var checkUrl = "https://docs.microsoft.com/ru-ru/kenguru";
            reports.Send(new UriBody(
                uri: new Uri("https://docs.microsoft.com/ru-ru"),
                body: "csdcdscdscsdhref='https://docs.microsoft.com/ru-ru/kenguru'dcsdsfdsfsfsfdsf"));
            var uriFinder = new UrlFinder(messages, reports);
            uriFinder.Work();
            messages.TryReceive(out var receiveMessage);
            Assert.AreEqual(new Uri(checkUrl), receiveMessage.Uri);
        }
    }
}