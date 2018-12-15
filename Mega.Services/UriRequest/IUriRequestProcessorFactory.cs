namespace Mega.Services.UriRequest
{
    using Mega.Messaging.MessageTypes;
    using Mega.Services.ZadolbaliClient;

    public interface IUriRequestProcessorFactory
    {
        IMessageProcessor<UriRequest> Create(ZadolbaliClient client);
    }
}