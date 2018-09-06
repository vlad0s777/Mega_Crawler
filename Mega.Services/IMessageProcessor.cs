namespace Mega.Services
{
    using System.Threading.Tasks;

    public interface IMessageProcessor
    {
        void Run();
    }

    public interface IMessageProcessor<in TMessage> : IMessageProcessor
    {
        Task Handle(TMessage message);
    }
}
