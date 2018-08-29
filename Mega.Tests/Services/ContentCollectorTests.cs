namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using NUnit.Framework;

    [TestFixture]
    public class ContentCollectorTests
    {
        [Test]
        public void Addbodies()
        {
            var bodies = new MessageBroker<UriBody>();
            var requests = new MessageBroker<UriRequest>();

            var rootUri = "https://docs.microsoft.com/ru-ru";

            new ContentCollector(
                requests,
                bodies,
                visitedUrls: new HashSet<Uri>(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Handle(new UriRequest(rootUri));

            Assert.IsTrue(requests.IsEmpty());
            Assert.IsFalse(bodies.IsEmpty());
        }

        [Test]
        public void FalseContentTest()
        {
            var bodies = new MessageBroker<UriBody>();

            var rootUri = "https://docs.microsoft.com/ru-ru";

            var contentCollector = new ContentCollector(
                new MessageBroker<UriRequest>(),
                bodies,
                visitedUrls: new HashSet<Uri>(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri));


            contentCollector.Handle(new UriRequest("http://someurl"));

            Assert.IsFalse(bodies.TryReceive(out var _));
        }

        [Test]
        public void TrueContentTest()
        {
            var bodies = new MessageBroker<UriBody>();
            var rootUri = "https://docs.microsoft.com/ru-ru";

            new ContentCollector(
                new MessageBroker<UriRequest>(),
                bodies,
                visitedUrls: new HashSet<Uri>(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Handle(new UriRequest(rootUri));
           
            Assert.IsTrue(bodies.TryReceive(out var receiveMessage));
            Assert.AreEqual(rootUri, receiveMessage.Uri.AbsoluteUri);
            Assert.AreEqual("8", receiveMessage.Body);
        }
    }
}