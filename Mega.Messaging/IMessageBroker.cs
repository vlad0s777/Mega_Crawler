namespace Mega.Messaging
{
    public interface IMessageBroker
    {
        bool IsEmpty();
    }

    public interface IMessageBroker<TMessage> : IMessageBroker
    {
        void Send(TMessage message);

        bool TryReceive(out TMessage message);
    }
}