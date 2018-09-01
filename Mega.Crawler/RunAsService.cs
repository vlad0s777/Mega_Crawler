namespace Mega.Crawler
{
    using DasMulli.Win32.ServiceUtils;

    using Mega.Services;

    using Microsoft.Extensions.Logging;

    public class RunAsService : IWin32Service
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<RunAsService>();

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Logger.LogDebug("Start service");
        }

        public void Stop()
        {
            Logger.LogDebug("Stop service");
        }

        public string ServiceName { get; } = "MegaCrawler";
    }
}