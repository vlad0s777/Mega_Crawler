namespace Mega.Services.ZadolbaliClient
{
    using Microsoft.Extensions.Logging;

    public static class ApplicationLogging
    {
        public static readonly ILoggerFactory LoggerFactory = new LoggerFactory().AddConsole(LogLevel.Information).AddEventLog(LogLevel.Debug);

        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();     
    }
}