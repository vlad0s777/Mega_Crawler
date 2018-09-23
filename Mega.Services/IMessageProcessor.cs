namespace Mega.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageProcessor
    {
        void Run(CancellationToken token);
    }

    public interface IMessageProcessor<in TMessage> : IMessageProcessor
    {
        Task Handle(TMessage message);
    }
}
