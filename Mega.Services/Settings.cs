namespace Mega.Services
{
    using System;

    using Microsoft.Extensions.Configuration;

    public class Settings
    {
        public int DepthLimit { get; }

        public int CountLimit { get; }

        public int AttemptLimit { get; }

        public string RootUriString { get; set; }

        public int Timeout { get; set; }

        public int[] Delay { get; set; }

        public Uri ProxyServer { get; set; }

        public Settings(string rootUriString, int depthLimit = -1, int countLimit = -1, int attemptLimit = 0, Uri proxyServer = null, int delayBegin = 0, int delayEnd = 0, int timeout = 0)
        {
            this.DepthLimit = depthLimit;
            this.CountLimit = countLimit;
            this.AttemptLimit = attemptLimit;
            this.RootUriString = rootUriString;
            this.ProxyServer = proxyServer;
            this.Delay = new int[] { delayBegin, delayEnd };
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
                this.DepthLimit = Convert.ToInt32(settings["depthLimit"]);
            }
            catch
            {
                this.DepthLimit = -1;
            }

            try
            {
                this.CountLimit = Convert.ToInt32(settings["countLimit"]);
            }
            catch
            {
                this.CountLimit = -1;
            }

            try
            {
                this.AttemptLimit = Convert.ToInt32(settings["attemptLimit"]);
            }
            catch
            {
                this.AttemptLimit = 0;
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
                this.Delay = new int[] { Convert.ToInt32(settings["delayBegin"]), Convert.ToInt32(settings["delayEnd"]) };
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
        }
    }
}
