﻿namespace Mega.Services.TagRequest
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
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<TagRequestProcessor>();

        private readonly IMessageBroker<string> requests;

        private readonly IRepository<Tag> tagRepository;

        private readonly ZadolbaliClient client;

        private readonly string rootUriString;

        public TagRequestProcessor(
            IMessageBroker<string> requests,
            IRepository<Tag> tagRepository,
            ZadolbaliClient client)
        {
            this.requests = requests;
            this.tagRepository = tagRepository;
            this.rootUriString = ZadolbaliClient.RootUriString;
            this.client = client;
        }

        public async Task Handle(string message)
        {
            Logger.LogInformation($"Processing {this.rootUriString + message}.");
            try
            {
                if (message == "tags")
                {
                    foreach (var tag in await this.client.GetTags())
                    {
                        await this.tagRepository.Create(new Tag { Name = tag.Name, TagKey = tag.TagKey });
                    }                  
                }

                Logger.LogInformation($"All tags added");
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
                Logger.LogWarning(e.Message);
            }
        }
    }
}
