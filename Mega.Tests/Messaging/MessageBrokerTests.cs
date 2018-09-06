namespace Mega.Tests.Messaging
{
    using Mega.Messaging;
    using Mega.Services.BrokerHandler;

    using NUnit.Framework;

    [TestFixture]
    public class MessageBrokerTests
    {
        [Test]
        public void Empty()
        {
            var queue = new MessageBroker<object>();
            Assert.IsFalse(queue.TryReceive(out var _));
        }

        [Test]
        public void IsEmptyFunctionTest()
        {
            var queue = new MessageBroker<object>();
            var queue2 = new MessageBroker<object>();
            queue2.Send(new object());
            Assert.IsTrue(queue.IsEmpty());
            Assert.IsFalse(queue2.IsEmpty());
        }

        [Test]
        public void ReturnsTheSame()
        {
            var queue = new MessageBroker<object>();

            var original = new { Url = "http://someurl", Body = 8 };

            queue.Send(original);

            Assert.IsTrue(queue.TryReceive(out var dequeued));
            Assert.AreSame(original, dequeued);
        }

        [Test]
        public void ConsumeWithTest()
        {
            var requests = new MessageBroker<UriRequest>();
            var rootUri = "https://docs.microsoft.com/ru-ru/";

            var testRequests = new MessageBroker<UriRequest>();

            for (var i = 0; i < 10; i++)
            {
                requests.Send(new UriRequest(rootUri + i));
            }

            requests.ConsumeWith(async uri => testRequests.Send(uri));

            Assert.IsFalse(requests.TryReceive(out var _));
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(testRequests.TryReceive(out var _));
            }

            Assert.IsFalse(testRequests.TryReceive(out var _));
        }
    }
}