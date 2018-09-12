namespace Mega.Crawler
{
    using System;

    using Microsoft.Extensions.Configuration;

    public class Settings
    {
        public int DepthLimit { get; }

        public int CountLimit { get; }

        public string RootUriString { get; set; }

        public Settings(string rootUriString, int depthLimit = -1, int countLimit = -1)
        {
            this.DepthLimit = depthLimit;
            this.CountLimit = countLimit;
            this.RootUriString = rootUriString;
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
        }
    }
}
