﻿namespace Mega.Messaging
{
    using System;
    using System.Collections.Generic;

    public class MessageBroker<T> : IMessageBroker<T>
    {
        private readonly Queue<T> queue = new Queue<T>();

        public bool TryReceive(out T message) => this.queue.TryDequeue(out message);

        public void ConsumeWith(Action<T> onReceive)
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