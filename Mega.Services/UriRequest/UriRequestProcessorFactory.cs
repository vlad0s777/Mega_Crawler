namespace Mega.Services.UriRequest
{
    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    public class UriRequestProcessorFactory : IUriRequestProcessorFactory
    {
        private readonly IMessageBroker<UriRequest> requests;

        private readonly ZadolbaliClient client;

        private readonly IDataContext dataContext;

        public UriRequestProcessorFactory(IMessageBroker<UriRequest> requests, IDataContext dataContext, ZadolbaliClient client)
        {
            this.requests = requests;
            this.dataContext = dataContext;
            this.client = client;
        }

        public IMessageProcessor<UriRequest> Create(string proxy)
        {
            return new UriRequestProcessor(this.requests, this.dataContext, this.client, proxy);
        }
    }
}