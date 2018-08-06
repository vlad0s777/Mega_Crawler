﻿namespace Mega.Messaging
{
    using System.Collections.Generic;

    public class MessageBroker<T> : IMessageBroker<T>
    {

        private readonly Queue<T> queue = new Queue<T>();

        public bool TryReceive(out T message) => this.queue.TryDequeue(out message);

        public void Send(T message) => this.queue.Enqueue(message);

        public bool isEmpty()
        {
            if (queue.Count == 0)
                return true;
            else return false;
        }
    }
}