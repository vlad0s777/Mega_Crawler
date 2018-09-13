namespace Mega.Services.WebClient
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    public class ProxyWebClient : WebClient
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ProxyWebClient>();

        private static readonly Stopwatch Watch = new Stopwatch();

        private readonly int timeout;

        private readonly int[] delay;

        private readonly WebProxy proxyServer;

        private readonly Random random;

        private readonly string rootUriString;

        public ProxyWebClient(Settings settings)
        {
            this.timeout = settings.Timeout;
            this.delay = settings.Delay;
            this.proxyServer = new WebProxy(settings.ProxyServer);
            this.random = new Random();
            this.rootUriString = settings.RootUriString;
        }
       
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = WebRequest.Create(address);
            webRequest.Timeout = this.timeout;

            //webRequest.Proxy = this.proxyServer;
            webRequest.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            webRequest.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
            webRequest.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            webRequest.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");

            return webRequest;
        }

        public async Task<string> GetStringAsync(string id)
        {
            Watch.Start();
            if (this.delay != null)
            {

                await Task.Delay(this.random.Next(this.delay.First(), this.delay.Last()));
            }
            var watchDelay = Watch.Elapsed.TotalMilliseconds;
            Watch.Restart();
            var completeDownloadString = await DownloadStringTaskAsync(new Uri(this.rootUriString + id, UriKind.Absolute));
            Logger.LogDebug($"Delay: {watchDelay} ms. Downloading: { Watch.Elapsed.TotalMilliseconds} ms.");
            Watch.Reset();
            return completeDownloadString;
        }
    }
}
