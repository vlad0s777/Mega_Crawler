namespace Mega.Tests.Services
{
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

            var someUri = "/123";

            await new UriRequestProcessor(
                requests,
                visitedUrls: new HashSet<string>(),
                client: new ZadolbaliClient(url => Task.FromResult("<li class='prev'><a href='" + url + "'></a></li>")),
                settings: new Settings("https://someurl")).Handle(new UriRequest(someUri));

            Assert.IsFalse(requests.IsEmpty());
            Assert.IsTrue(requests.TryReceive(out var uri));
            Assert.AreEqual("123", uri.Id);
        }

        [Test]
        public async Task SameUrlTest()
        {
            var someUri = "/123";

            var requests = new MessageBroker<UriRequest>();
            var contentCollector = new UriRequestProcessor(
                requests,
                visitedUrls: new HashSet<string>(),
                client: new ZadolbaliClient(url => Task.FromResult("<li class='prev'><a href='" + url + "'></a></li>")),
                settings: new Settings("https://someurl"));

            await contentCollector.Handle(new UriRequest(someUri));
            Assert.IsTrue(requests.TryReceive(out var _));
            await contentCollector.Handle(new UriRequest(someUri));
            Assert.IsFalse(requests.TryReceive(out var _));
        }
    }
}