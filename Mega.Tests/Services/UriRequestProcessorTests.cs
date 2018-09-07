namespace Mega.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.UriRequest;
    using Mega.Services.WebClient;

    using NUnit.Framework;

    [TestFixture]
    public class UriRequestProcessorTests
    {
        [Test]
        public async Task Addrequest()
        {
            var requests = new MessageBroker<UriRequest>();

            var rootUri = "https://docs.microsoft.com/ru-ru";

            await new UriRequestProcessor(
                requests,
                visitedUrls: new HashSet<Uri>(),
                client: new ZadolbaliClient(url => Task.FromResult("<li class='prev'><a href='" + url.AbsoluteUri + "'></a></li>")),
                settings: new Settings(rootUri)).Handle(new UriRequest(new Uri(rootUri)));

            Assert.IsFalse(requests.IsEmpty());
            Assert.IsTrue(requests.TryReceive(out var uri));
            Assert.AreEqual(rootUri, uri.Uri.AbsoluteUri);
        }

        [Test]
        public async Task ValideUrlTest()
        {
            var rootUri = "https://docs.microsoft.com/ru-ru";

            var requests = new MessageBroker<UriRequest>();
            var contentCollector = new UriRequestProcessor(
                requests,
                visitedUrls: new HashSet<Uri>(),
                client: new ZadolbaliClient(url => Task.FromResult("<li class='prev'><a href='" + url.AbsoluteUri + "'></a></li>")),
                settings: new Settings(rootUri));

            await contentCollector.Handle(new UriRequest("http://someurl"));
            Assert.IsFalse(requests.TryReceive(out var _));
            await contentCollector.Handle(new UriRequest("https://docs.microsoft.com/ru-ru/tra"));
            Assert.IsTrue(requests.TryReceive(out var _));
        }
    }
}