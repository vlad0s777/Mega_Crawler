namespace Mega.Services.UriRequest
{
    using System;
    using System.Collections.Generic;

    using Mega.Domain;
    using Mega.Messaging;

    public interface IProcessorFactory
    {
        IEnumerable<IMessageProcessor> Create();
    }

    public class UriRequestProcessorFactory : IProcessorFactory, IDisposable
    {
        private readonly IMessageBroker<UriRequest> requests;

        private readonly Settings settings;

        private readonly IDataContext dataContext;

        public UriRequestProcessorFactory(IMessageBroker<UriRequest> requests, Settings settings, IDataContext dataContext)
        {
            this.requests = requests;
            this.settings = settings;
            this.dataContext = dataContext;
        }

        public IEnumerable<IMessageProcessor> Create()
        {
            foreach (var proxy in this.settings.ProxyServers)
            {
                this.settings.CurrentProxyServer = proxy;
                yield return new UriRequestProcessor(this.requests, this.settings, this.dataContext.CreateNewContext());
            }
        }

        public void Dispose()
        {
            this.dataContext?.Dispose();
        }
    }
}