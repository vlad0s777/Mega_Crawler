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

        [Test]
        public void taskDoneTest() //проверка функции isEmpty класса MessageBroker
        {
            //arrange
            var queue = new MessageBroker<object>();
            var queue2 = new MessageBroker<object>();
            //act
            queue2.Send(new object());
            //assert
            Assert.True(queue.isEmpty());
            Assert.False(queue2.isEmpty());
        }

    }
}
