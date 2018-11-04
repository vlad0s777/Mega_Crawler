namespace Mega.Services.UriRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.Extensions.Logging;

    public class UriRequestProcessor : IMessageProcessor<UriRequest>
    {
        private readonly ILogger logger;

        private readonly int countAttempt;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly ISomeReportDataProvider someReportDataProvider;

        private readonly Uri rootUri;

        private readonly ZadolbaliClient client;

        public UriRequestProcessor(
            ILoggerFactory loggerFactory,
            IMessageBroker<UriRequest> requests,
            ISomeReportDataProvider someReportDataProvider,
            ZadolbaliClient client)
        {
            this.logger = loggerFactory.CreateLogger<UriRequestProcessor>();

            this.requests = requests;

            this.someReportDataProvider = someReportDataProvider;

            this.client = client;

            this.rootUri = new Uri(ZadolbaliClient.RootUriString, UriKind.Absolute);

            this.countAttempt = ZadolbaliClient.CountAttempt;
        }

        public async Task Handle(UriRequest message)
        {
            this.logger.LogInformation($"Processing {this.rootUri + message.Id}.");

            try
            {
                if (message.Id == string.Empty)
                {
                    foreach (var id in await this.client.GenerateIDs())
                    {
                        this.requests.Send(new UriRequest(id));
                    }

                    return;
                }

                if (message.Id == "tags")
                {
                    foreach (var tag in await this.client.GetTags())
                    {
                        await this.someReportDataProvider.AddAsync(new Tag { Name = tag.Name, TagKey = tag.TagKey });
                    }

                    this.logger.LogInformation($"All tags added");

                    this.requests.Send(new UriRequest(string.Empty));

                    return;
                }

                var articles = await this.client.GetArticles(message.Id);

                foreach (var article in articles)
                {
                    try
                    {
                        var domainArticle = (Article)await this.someReportDataProvider.AddAsync(new Article
                                            {
                                                OuterArticleId = article.Id,
                                                DateCreate = article.DateCreate,
                                                Head = article.Head,
                                                Text = article.Text
                                            });
                    
                        foreach (var tag in article.Tags)
                        {
                            var domainTag = await this.someReportDataProvider.GetTag(tag.TagKey);
                            await this.someReportDataProvider.AddAsync(new ArticleTag { ArticleId = domainArticle.ArticleId, TagId = domainTag.TagId });
                        }

                        this.logger.LogInformation($"Added from the page {message.Id} to the database {domainArticle.Head}.");
                    }
                    catch
                    {
                        this.logger.LogWarning($"Article id {article.Id} alreydy exists!");
                    }
                }
            }
            catch (Exception e)
            {
                var att = message.Attempt + 1;
                if (att < this.countAttempt)
                {
                    this.requests.Send(new UriRequest(message.Id, att, message.Depth));
                    this.logger.LogWarning(
                        $"{e.Message}. There are still attempts: {this.countAttempt - message.Attempt}");
                }
                else
                {
                    this.logger.LogWarning($"{e.Message}. Attempts are no more!");
                }
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