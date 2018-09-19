namespace Mega.Services.UriRequest
{
    using System.Collections.Generic;

    using Mega.Messaging;

    public interface IProcessorFactory
    {
        IEnumerable<IMessageProcessor> Create();
    }

    public class UriRequestProcessorFactory : IProcessorFactory
    {
        private readonly IMessageBroker<UriRequest> requests;

        private readonly Settings settings;

        public UriRequestProcessorFactory(IMessageBroker<UriRequest> requests, Settings settings)
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