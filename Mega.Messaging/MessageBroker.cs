namespace Mega.Messaging
{
    using System;
    using System.Collections.Generic;

    public class MessageBroker<T> : IMessageBroker<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();

        public bool TryReceive(out T message)
        {
            return _queue.TryDequeue(out message);
        }

        public void ConsumeWith(Action<T> onReceive)
        {
            throw new NotSupportedException();
        }

        public void Send(T message)
        {
            _queue.Enqueue(message);
        }

        public bool IsEmpty()
        {
            if (_queue.Count == 0)
                return true;
            return false;
        }
    }
}