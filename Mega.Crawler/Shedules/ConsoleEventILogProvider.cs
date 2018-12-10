namespace Mega.Crawler.Shedules
{
    using System;
    using System.Diagnostics;

    using Quartz.Logging;

    using LogLevel = Quartz.Logging.LogLevel;

    public class ConsoleEventILogProvider : ILogProvider
    {
        private EventLogEntryType ConvertLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return EventLogEntryType.Information;
                case LogLevel.Debug:
                    return EventLogEntryType.Information;
                case LogLevel.Info:
                    return EventLogEntryType.Information;
                case LogLevel.Warn:
                    return EventLogEntryType.Warning;
                case LogLevel.Error:
                    return EventLogEntryType.Error;
                case LogLevel.Fatal:
                    return EventLogEntryType.Error;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
                {
                    if (level < LogLevel.Info || func == null)
                    {
                        return true;
                    }

                    var message = typeof(ConsoleEventILogProvider).FullName + " " + func();
                    Console.WriteLine(level.ToString() + ": " + message, parameters);

                    using (var eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry(message, ConvertLogLevel(level), 0, 1);
                    }

                    return true;
                };
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
