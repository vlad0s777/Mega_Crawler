namespace Mega.Crawler
{
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

            this.DepthLimit = int.TryParse(settings["depthLimit"], out var val) ? val : -1;

            this.CountLimit = int.TryParse(settings["countLimit"], out val) ? val : -1;
        }
    }
}
