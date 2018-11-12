namespace Mega.Crawler.Jobs
{
    using System.Threading.Tasks;

    using Mega.Domain.Repositories;
    using Mega.Messaging;
    using Mega.Services.UriRequest;

    using Microsoft.Extensions.Logging;

    using Quartz;

    public class MessageSenderJob : IJob
    {
        public IMessageBroker<UriRequest> Broker { private get; set; }

        public ITagRepository TagRepository { private get; set; }

        public ILogger Logger { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            this.Broker.Send(await this.TagRepository.CountTags() != 0 ? new UriRequest(string.Empty) : new UriRequest("tags"));
            this.Logger.LogInformation("MessageSenderJob is executing.");
        }
    }
}
