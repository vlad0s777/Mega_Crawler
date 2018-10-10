namespace Mega.Services.UriRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    public interface IProcessorFactory
    {
        IEnumerable<IMessageProcessor> Create();
    }

    public class UriRequestProcessorFactory : IProcessorFactory
    {
        private readonly IMessageBroker<UriRequest> requests;

        private readonly ProxySettings settings;

        private readonly IDataContext dataContext;

        private readonly Random random;

        public UriRequestProcessorFactory(IMessageBroker<UriRequest> requests, ProxySettings settings, IDataContext dataContext)
        {
            this.requests = requests;
            this.settings = settings;
            this.dataContext = dataContext;
            this.random = new Random();
        }

        public IEnumerable<IMessageProcessor> Create()
        {
            foreach (var proxy in this.settings.ProxyServers)
            {
                var delay = this.random.Next(this.settings.Delay.First(), this.settings.Delay.Last());
                yield return new UriRequestProcessor(this.requests, this.dataContext.CreateNewContext(), this.settings.RootUriString, this.settings.AttemptLimit, this.settings.Timeout, delay, proxy);
            }
        }
    }
}