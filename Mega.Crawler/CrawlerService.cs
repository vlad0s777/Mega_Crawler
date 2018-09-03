namespace Mega.Crawler
{
    using DasMulli.Win32.ServiceUtils;

    using Mega.Services;

    using Microsoft.Extensions.Logging;

    public class CrawlerService : IWin32Service
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<CrawlerService>();

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback) => Logger.LogInformation("Start service");

        public void Stop() => Logger.LogInformation("Stop service");

        public string ServiceName { get; } = "MegaCrawler";
    }
}