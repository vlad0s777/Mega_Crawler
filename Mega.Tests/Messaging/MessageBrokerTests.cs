namespace Mega.Tests.Messaging
{
    using NUnit.Framework;

    using Mega.Messaging;

    [TestFixture]
    public class MessageBrokerTests
    {
        [Test]
        public void Empty()
        {
            var queue = new MessageBroker<object>();
            Assert.IsFalse(queue.TryReceive(out var message));
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

    }
}
