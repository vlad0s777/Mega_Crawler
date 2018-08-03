namespace Mega.Messaging
{
    public interface IMessageBroker<TMessage>
    {
        void Send(TMessage message);

        bool TryReceive(out TMessage message);
    }
}
