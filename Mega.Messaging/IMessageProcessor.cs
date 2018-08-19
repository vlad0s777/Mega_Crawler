namespace Mega.Messaging
{
    public interface IMessageProcessor<T>
    {
        bool Run();
    }
}
