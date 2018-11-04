namespace Mega.Services.ZadolbaliClient
{
    using System;
    using System.Diagnostics;

    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    public class ProxyWebClient : WebClient
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        private readonly ILogger logger;

        private readonly int timeout;

        private readonly string rootUriString;

        private readonly Random random;

        private readonly int delayBegin;

        private readonly int delayEnd;

        private WebProxy proxyServer;       

        public string ProxyServer
        {
            get => this.proxyServer.ToString();
            set => this.proxyServer = value != string.Empty ? new WebProxy(value) : new WebProxy();
        }

        public ProxyWebClient(ILoggerFactory loggerFactory, Random random, string rootUriString, int timeout = 0, int delayBegin = 0, int delayEnd = 0, string proxy = "")
        {
            this.timeout = timeout;
            this.ProxyServer = proxy;
            this.rootUriString = rootUriString;
            this.logger = loggerFactory.CreateLogger(typeof(ProxyWebClient).FullName + " " + proxy);
            this.random = random;
            this.delayBegin = delayBegin;
            this.delayEnd = delayEnd;
        }
       
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = WebRequest.Create(address);
            webRequest.Timeout = this.timeout;

            webRequest.Proxy = this.proxyServer;
            webRequest.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            webRequest.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
            webRequest.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            webRequest.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");

            return webRequest;
        }

        public async Task<string> GetStringAsync(string id)
        {
            try
            {
                Watch.Start();
                DownloadStatistic.Start();

                var delay = this.random.Next(this.delayBegin, this.delayEnd);
                await Task.Delay(delay);

                var watchDelay = Watch.Elapsed.TotalMilliseconds;
                Watch.Restart();

                var completeDownloadString = await DownloadStringTaskAsync(new Uri(this.rootUriString + id, UriKind.Absolute));

                DownloadStatistic.Inсrement();

                this.logger.LogDebug($"Delay: {watchDelay} ms. Downloading: {Watch.Elapsed.TotalMilliseconds} ms. Speed: {DownloadStatistic.Speed()}");
                Watch.Reset();
                this.logger.LogDebug($"Delay : {delay}");
                return completeDownloadString;
            }
            catch
            {             
                Watch.Reset();
                throw;
            }
        }
    }
}
