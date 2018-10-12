namespace Mega.Services.UriRequest
{
    public interface IUriRequestProcessorFactory
    {
        IMessageProcessor<UriRequest> Create(string proxy);
    }
}