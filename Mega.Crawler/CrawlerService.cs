namespace Mega.Crawler
{
    using DasMulli.Win32.ServiceUtils;

    using Microsoft.Extensions.Logging;

    public class CrawlerService : IWin32Service
    {
        private readonly ILogger logger;

        public CrawlerService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<CrawlerService>();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback) => this.logger.LogInformation("Start service");

        public void Stop() => this.logger.LogInformation("Stop service");

        public string ServiceName { get; } = "MegaCrawler";
    }
}