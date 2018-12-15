namespace Mega.Crawler.Shedules.Jobs
{
    using System.Threading.Tasks;
    using Mega.Domain.Repositories;
    using Mega.Messaging;
    using Mega.Messaging.MessageTypes;
    using Mega.Services;

    using Microsoft.Extensions.Logging;
    using Quartz;

    public class CrawlerStartJob : IJob
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<Runner>();

        private readonly IMessageBroker<UriRequest> broker;

        private readonly ITagRepository tagRepository;

        public CrawlerStartJob(IMessageBroker<UriRequest> broker, ITagRepository tagRepository)
        {
            this.broker = broker;
            this.tagRepository = tagRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this.broker.Send(await this.tagRepository.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));
            Logger.LogInformation("CrawlerLauncherJob is executing.");
        }
    }
}
