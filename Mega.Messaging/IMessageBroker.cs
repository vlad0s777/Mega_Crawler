namespace Mega.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageBroker
    {
        bool IsEmpty();
    }

    public interface IMessageBroker<TMessage> : IMessageBroker
    {
        void Send(TMessage message);

        bool TryReceive(out TMessage message);

        void ConsumeWith(Func<TMessage, Task> onReceive, CancellationToken token);
    }
}