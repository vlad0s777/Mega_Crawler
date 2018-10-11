namespace Mega.Services.TagRequest
{
    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    public class TagRequestProcessorFactory : ITagRequestProcessorFactory
    {
        private readonly IMessageBroker<string> requests;

        private readonly IDataContext dataContext;

        private readonly ZadolbaliClient client;

        public TagRequestProcessorFactory(IMessageBroker<string> requests, IDataContext dataContext, ZadolbaliClient client)
        {
            this.requests = requests;
            this.dataContext = dataContext;
            this.client = client;
        }

        public IMessageProcessor<string> Create(string proxy)
        {
            return new TagRequestProcessor(this.requests, this.dataContext, this.client, proxy);
        }
    }
}