namespace Mega.Services.ZadolbaliClient
{
    using System;
    using System.Diagnostics;

    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    public class ProxyWebClient : WebClient
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ProxyWebClient>();

        private static readonly Stopwatch Watch = new Stopwatch();

        private readonly int timeout;

        private readonly string rootUriString;

        private WebProxy proxyServer;

        public string ProxyServer
        {
            get => this.proxyServer.ToString();
            set => this.proxyServer = value != string.Empty ? new WebProxy(value) : new WebProxy();
        }

        public int Delay { get; set; }

        public ProxyWebClient(string rootUriString, int timeout = 0, string proxy = "")
        {
            this.Delay = 0;
            this.timeout = timeout;
            this.ProxyServer = proxy;
            this.rootUriString = rootUriString;
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
                await Task.Delay(this.Delay);
                var watchDelay = Watch.Elapsed.TotalMilliseconds;
                Watch.Restart();

                var completeDownloadString = await DownloadStringTaskAsync(new Uri(this.rootUriString + id, UriKind.Absolute));

                DownloadStatistic.Inсrement();

                Logger.LogDebug(
                    $"Delay: {watchDelay} ms. Downloading: {Watch.Elapsed.TotalMilliseconds} ms. Speed: {DownloadStatistic.Speed()}");
                Watch.Reset();
                Logger.LogDebug($"This proxy {this.proxyServer.Address} delay : {this.Delay}");
                return completeDownloadString;
            }
            catch (Exception e)
            {             
                Watch.Reset();
                throw new Exception($"This proxy {this.proxyServer.Address} in id: {id} error: {e.Message}. {e.GetType().FullName}.");
            }
        }
    }
}
