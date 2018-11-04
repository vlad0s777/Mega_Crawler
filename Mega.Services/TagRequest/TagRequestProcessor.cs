namespace Mega.Services.TagRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    public class TagRequestProcessor : IMessageProcessor<string>
    {
        private readonly ILogger logger;

        private readonly IMessageBroker<string> requests;

        private readonly ISomeReportDataProvider someReportDataProvider;

        private readonly ZadolbaliClient client;

        private readonly string rootUriString;

        public TagRequestProcessor(
            ILoggerFactory loggerFactory,
            IMessageBroker<string> requests,
            ISomeReportDataProvider someReportDataProvider,
            ZadolbaliClient client)
        {
            this.logger = loggerFactory.CreateLogger<TagRequestProcessor>();
            this.requests = requests;
            this.someReportDataProvider = someReportDataProvider;
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
                        await this.someReportDataProvider.AddAsync(new Tag { Name = tag.Name, TagKey = tag.TagKey });
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
