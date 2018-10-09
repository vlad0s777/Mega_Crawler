namespace Mega.WebClient.ZadolbaliClient
{
    using Microsoft.Extensions.Configuration;

    public class ProxySettings
    {
        public int Timeout { get; }

        public int[] Delay { get; }

        public string[] ProxyServers { get; }

        public string CurrentProxyServer { get; set; }

        public int AttemptLimit { get; }

        public string RootUriString { get; }

        public ProxySettings(
            string rootUriString,
            string[] proxyServers = null,
            int attemptLimit = 0,
            int delayBegin = 0,
            int delayEnd = 0,
            int timeout = 0)
        {
            this.AttemptLimit = attemptLimit;
            this.RootUriString = rootUriString;
            this.ProxyServers = proxyServers;
            this.Delay = new[] { delayBegin, delayEnd };
            this.Timeout = timeout;
        }

        public ProxySettings(IConfiguration settings)
        {
            try
            {
                this.RootUriString = settings["rootUrl"];
            }
            catch
            {
                this.RootUriString = string.Empty;
            }

            try
            {
                this.ProxyServers = settings.GetSection("proxyServers").Get<string[]>();
            }
            catch
            {
                this.ProxyServers = null;
            }

            this.Delay = int.TryParse(settings["delayBegin"], out var delayBegin) && int.TryParse(settings["delayEnd"], out var delayEnd)
                             ? new[] { delayBegin, delayEnd }
                             : null;

            this.Timeout = int.TryParse(settings["timeout"], out var val) ? val : 0;

            this.AttemptLimit = int.TryParse(settings["attemptLimit"], out val) ? val : 0;
        }
    }
}
