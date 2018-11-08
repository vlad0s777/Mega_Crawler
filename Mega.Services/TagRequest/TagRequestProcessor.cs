namespace Mega.Services.TagRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    public class TagRequestProcessor : IMessageProcessor<string>
    {
        private readonly ILogger logger;

        private readonly IMessageBroker<string> requests;

        private readonly IRepository<Tags> tagRepository;

        private readonly ZadolbaliClient client;

        private readonly string rootUriString;

        public TagRequestProcessor(
            ILoggerFactory loggerFactory,
            IMessageBroker<string> requests,
            IRepository<Tags> tagRepository,
            ZadolbaliClient client)
        {
            this.logger = loggerFactory.CreateLogger<TagRequestProcessor>();
            this.requests = requests;
            this.tagRepository = tagRepository;
            this.rootUriString = ZadolbaliClient.RootUriString;
            this.client = client;
        }

        public async Task Handle(string message)
        {
            this.logger.LogInformation($"Processing {this.rootUriString + message}.");
            try
            {
                if (message == "tags")
                {
                    foreach (var tag in await this.client.GetTags())
                    {
                        await this.tagRepository.Create(new Tags { Name = tag.Name, Tag_Key = tag.TagKey });
                    }                  
                }

                this.logger.LogInformation($"All tags added");
            }
            catch
            {
                this.requests.Send(message);
            }
        }

        public void Run(CancellationToken token)
        {
            try
            {
                this.requests.ConsumeWith(Handle, token);
            }
            catch (Exception e)
            {
                this.logger.LogWarning(e.Message);
            }
        }
    }
}
