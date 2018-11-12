namespace Mega.Crawler.Jobs
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Mega.Domain.Repositories;
    using Mega.Messaging;
    using Mega.Services.UriRequest;

    using Microsoft.Extensions.Logging;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Logging;

    using LogLevel = Quartz.Logging.LogLevel;

    public class MessageSheduler
    {
        public class ConsoleEventLogProvider : ILogProvider
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

                        var message = typeof(MessageSheduler).FullName + " " + func();
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

        private readonly IMessageBroker<UriRequest> broker;

        private readonly ITagRepository tagRepository;

        private readonly ILoggerFactory loggerFactory;

        public string CronExpression { private get; set; }

        public MessageSheduler(IMessageBroker<UriRequest> broker, ITagRepository tagRepository, ILoggerFactory loggerFactory)
        {
            this.broker = broker;
            this.tagRepository = tagRepository;
            this.loggerFactory = loggerFactory;
            this.CronExpression = string.Empty;
        }

        public async Task Start()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleEventLogProvider());

            var props = new NameValueCollection
                            {
                                { "quartz.serializer.type", "binary" }
                            };
            try
            {
                var factory = new StdSchedulerFactory(props);
                var scheduler = await factory.GetScheduler();

                await scheduler.Start();

                var job = JobBuilder.Create<MessageSenderJob>()
                    .WithIdentity("<MessageSenderJob", "CrawlerGroup")
                    .Build();

                job.JobDataMap["broker"] = this.broker;
                job.JobDataMap["tagRepository"] = this.tagRepository;
                job.JobDataMap["logger"] = this.loggerFactory.CreateLogger<MessageSenderJob>();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "CrawlerGroup")
                    .WithCronSchedule(this.CronExpression)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }        
        }
    }
}
