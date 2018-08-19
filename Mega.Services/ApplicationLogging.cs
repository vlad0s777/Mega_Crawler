namespace Mega.Services
{
    using Microsoft.Extensions.Logging;

    public static class ApplicationLogging
    {
        public static readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();     
    }
}