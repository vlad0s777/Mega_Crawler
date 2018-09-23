namespace Mega.Services.UriRequest
{
    using System.Collections.Generic;

    using Mega.Messaging;

    public interface IProcessorFabric
    {
        IEnumerable<IMessageProcessor> Create();
    }

    public class UriRequestProcessorFabric : IProcessorFabric
    {
        private readonly IMessageBroker<UriRequest> requests;

        private readonly Settings settings;

        public UriRequestProcessorFabric(IMessageBroker<UriRequest> requests, Settings settings)
        {
            this.requests = requests;
            this.settings = settings;
        }

        public IEnumerable<IMessageProcessor> Create()
        {
            foreach (var proxy in this.settings.ProxyServers)
            {
                this.settings.CurrentProxyServer = proxy;
                yield return new UriRequestProcessor(this.requests, this.settings);
            }
        }
    }
}