namespace Mega.Services.WebClient
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;

    public class ProxyWebClient : WebClient
    {

        private readonly int timeout;

        private readonly int[] delay;

        private readonly WebProxy proxyServer;

        private readonly Random random;

        public ProxyWebClient(Settings settings)
        {
            this.timeout = settings.Timeout;
            this.delay = settings.Delay;
            this.proxyServer = new WebProxy(settings.ProxyServer);
            this.random = new Random();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if (this.delay != null)
            {
                Thread.Sleep(this.random.Next(this.delay.First(), this.delay.Last()));
            }

            var webRequest = WebRequest.Create(address);
            webRequest.Timeout = this.timeout;
            webRequest.Proxy = this.proxyServer;
            return webRequest;
        }
    }
}
