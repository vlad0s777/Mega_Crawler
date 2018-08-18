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

        public bool IsTimeout { get; }

        public Settings(string rootUriString, int depthLimit = -1, int countLimit = -1, int attemptLimit = 0, bool isTimeout = false)
        {
            this.DepthLimit = depthLimit;
            this.CountLimit = countLimit;
            this.AttemptLimit = attemptLimit;
            this.RootUriString = rootUriString;
            this.IsTimeout = isTimeout;
        }

        public Settings(IConfiguration settings)
        {
            try
            {
                this.IsTimeout = Convert.ToBoolean(settings["isTimeout"]);
            }
            catch
            {
                this.IsTimeout = false;
            }
            
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
        }
    }
}
