using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } =
            new LoggerFactory().AddConsole().AddDebug();
        public static ILogger CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();
    }
}