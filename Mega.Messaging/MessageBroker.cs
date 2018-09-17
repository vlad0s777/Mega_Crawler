namespace Mega.Messaging
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class MessageBroker<T> : IMessageBroker<T>
    {
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public bool TryReceive(out T message) => this.queue.TryDequeue(out message);

        public void ConsumeWith(Func<T, Task> onReceive, CancellationToken token)
        {           
            while (TryReceive(out T message))
            {
                onReceive(message);
            }
        }

        public void Send(T message) => this.queue.Enqueue(message);

        public bool IsEmpty() => this.queue.Count == 0;
    }
}