namespace Mega.Tests.Messaging
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services;
    using Mega.Services.ContentCollector;
    using Mega.Services.InfoParser;

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
            var bodies = new MessageBroker<UriBody>();

            var rootUri = "https://docs.microsoft.com/ru-ru/";

            var colCon = new ContentCollector(
                new MessageBroker<UriRequest>(),
                bodies,
                new HashSet<Uri>(),
                clientDelegate: uri => "8",
                settings: new Settings(rootUri));

            for (var i = 0; i < 10; i++)
            {
                requests.Send(new UriRequest(rootUri + i));
            }

            requests.ConsumeWith(colCon.Handle);

            Assert.IsFalse(requests.TryReceive(out var _));
            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(bodies.TryReceive(out var _));
            }

            Assert.IsFalse(bodies.TryReceive(out var _));
        }
    }
}