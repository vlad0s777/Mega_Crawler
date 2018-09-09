namespace Mega.Services
{
    using System;

    using Microsoft.Extensions.Configuration;

    public class Settings
    {
        public int Timeout { get; }

        public int[] Delay { get; }

        public Uri ProxyServer { get; }

        public int AttemptLimit { get; }

        public string RootUriString { get; }

        public Settings(string rootUriString, int attemptLimit = 0, Uri proxyServer = null, int delayBegin = 0, int delayEnd = 0, int timeout = 0)
        {
            this.AttemptLimit = attemptLimit;
            this.RootUriString = rootUriString;
            this.ProxyServer = proxyServer;
            this.Delay = new[] { delayBegin, delayEnd };
            this.Timeout = timeout;
        }

        public Settings(IConfiguration settings)
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
                this.ProxyServer = new Uri(settings["proxyServer"]);
            }
            catch
            {
                this.ProxyServer = null;
            }

            try
            {
                this.Delay = new[] { Convert.ToInt32(settings["delayBegin"]), Convert.ToInt32(settings["delayEnd"]) };
            }
            catch
            {
                this.Delay = null;
            }

            try
            {
                this.Timeout = Convert.ToInt32(settings["timeout"]);
            }
            catch
            {
                this.Timeout = 0;
            }

            try
            {
                this.AttemptLimit = Convert.ToInt32(settings["attemptLimit"]);
            }
            catch
            {
                this.AttemptLimit = 0;
            }
        }
    }
}
