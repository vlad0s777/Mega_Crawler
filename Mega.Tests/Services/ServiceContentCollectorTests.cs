namespace Mega.Tests.Services
{
    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

    using NUnit.Framework;

    [TestFixture]
    public class ServiceContentCollectorTests
    {
        [Test]
        public void Addbodies()
        {
            var bodies = new MessageBroker<UriBody>();
            var requests = new MessageBroker<UriRequest>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            requests.Send(new UriRequest(rootUri));

            new ServiceContentCollector(
                requests,
                bodies,
                visitedUrls: new WrapperUries(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Run();

            Assert.IsTrue(requests.IsEmpty());
            Assert.IsFalse(bodies.IsEmpty());
        }

        [Test]
        public void FalseContentTest()
        {
            var bodies = new MessageBroker<UriBody>();
            var requests = new MessageBroker<UriRequest>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            requests.Send(new UriRequest(rootUri));

            var contentCollector = new ServiceContentCollector(
                requests,
                bodies,
                visitedUrls: new WrapperUries(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri));

            requests.Send(new UriRequest("http://someurl"));

            contentCollector.Run();

            Assert.IsTrue(bodies.TryReceive(out var _));
            Assert.IsFalse(bodies.TryReceive(out var _));
        }

        [Test]
        public void LimitTest()
        {
            var bodies = new MessageBroker<UriBody>();
            var requests = new MessageBroker<UriRequest>();

            var visitedUrls = new WrapperUries();

            var rootUri = "https://docs.microsoft.com/ru-ru/";
            
            for (var i = 0; i < 10; i++)
            {
                requests.Send(new UriRequest(rootUri + i));
            }

            requests.Send(new UriRequest(rootUri));

            var colCon = new ServiceContentCollector(
                requests,
                bodies,
                visitedUrls,
                clientDelegate: uri => "8", 
                settings: new Settings(rootUri, countLimit: 6));

            while (!requests.IsEmpty())
            {
                colCon.Run();
            }
         
            Assert.AreEqual(6, visitedUrls.Uries.Count);
        }

        [Test]
        public void TrueContentTest()
        {
            var bodies = new MessageBroker<UriBody>();
            var requests = new MessageBroker<UriRequest>();

            var rootUri = "https://docs.microsoft.com/ru-ru";
            requests.Send(new UriRequest(rootUri));
            new ServiceContentCollector(
                requests,
                bodies,
                visitedUrls: new WrapperUries(), 
                clientDelegate: body => "8",
                settings: new Settings(rootUri)).Run();
           
            Assert.IsTrue(bodies.TryReceive(out var receiveMessage));
            Assert.AreEqual(rootUri, receiveMessage.Uri.AbsoluteUri);
            Assert.AreEqual("8", receiveMessage.Body);
        }
    }
}