namespace Mega.Services.TagRequest
{
    public interface ITagRequestProcessorFactory
    {
        IMessageProcessor<string> Create(string proxy);
    }
}