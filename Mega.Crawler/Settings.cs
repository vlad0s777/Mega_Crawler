namespace Mega.Crawler
{
    using Microsoft.Extensions.Configuration;

    public class Settings
    {
        public string[] ProxyServers { get; }

        public Settings(
            string[] proxyServers = null)
        {
            this.ProxyServers = proxyServers;
        }

        public Settings(IConfiguration settings)
        {
            try
            {
                this.ProxyServers = settings.GetSection("proxyServers").Get<string[]>();
            }
            catch
            {
                this.ProxyServers = null;
            }
        }
    }
}
