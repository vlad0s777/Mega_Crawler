namespace Mega.Services.TagRequest
{
    using Mega.Services.ZadolbaliClient;

    public interface ITagRequestProcessorFactory
    {
        IMessageProcessor<string> Create(ZadolbaliClient client);
    }
}