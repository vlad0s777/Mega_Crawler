namespace Mega.Crawler
{
    using Microsoft.Extensions.Configuration;

    public class Settings
    {
        public Settings(string[] proxyServers = null)
        {
            this.ProxyServers = proxyServers;
        }

        public Settings(IConfiguration configuration)
        {
            try
            {
                this.ProxyServers = configuration.GetSection("proxyServers").Get<string[]>();
            }
            catch
            {
                this.ProxyServers = null;
            }
        }

        public string[] ProxyServers { get; }
    }
}
