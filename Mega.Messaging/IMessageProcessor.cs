namespace Mega.Messaging
{
    public interface IMessageProcessor
    {
        void Run();
    }

    public interface IMessageProcessor<in TMessage> : IMessageProcessor
    {
        void Handle(TMessage message);
    }
}
