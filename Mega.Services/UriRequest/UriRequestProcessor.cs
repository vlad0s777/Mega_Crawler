namespace Mega.Services.UriRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.ZadolbaliClient;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class UriRequestProcessor : IMessageProcessor<UriRequest>
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<UriRequestProcessor>();

        private readonly int countAttempt;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly IDataContext dataContext;

        private readonly Uri rootUri;

        private readonly ZadolbaliClient client;

        public UriRequestProcessor(
            IMessageBroker<UriRequest> requests,
            IDataContext dataContext,
            ZadolbaliClient client,
            string proxy = "")
        {
            this.requests = requests;

            this.dataContext = dataContext;

            this.client = client;

            this.client.Proxy = proxy;

            this.rootUri = new Uri(ZadolbaliClient.RootUriString, UriKind.Absolute);

            this.countAttempt = ZadolbaliClient.CountAttempt;
        }

        public async Task Handle(UriRequest message)
        {
            Logger.LogInformation($"Processing {this.rootUri + message.Id}.");

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
                        await this.dataContext.AddAsync(new Tag { Name = tag.Name, TagKey = tag.TagKey });
                        await this.dataContext.SaveChangesAsync();
                    }

                    Logger.LogInformation($"All tags added");

                    this.requests.Send(new UriRequest(string.Empty));

                    return;
                }

                var articles = await this.client.GetArticles(message.Id);

                foreach (var article in articles)
                {
                    var domainArticle = new Article()
                                            {
                                                OuterArticleId = article.Id,
                                                DateCreate = article.DateCreate,
                                                Head = article.Head,
                                                Text = article.Text
                                            };
                    try
                    {
                        foreach (var tag in article.Tags)
                        {
                            var domainTag = await this.dataContext.GetTag(tag.TagKey);
                            await this.dataContext.AddAsync(new ArticleTag { Article = domainArticle, Tag = domainTag });
                        }

                        await this.dataContext.SaveChangesAsync();
                        Logger.LogInformation($"Added from the page {message.Id} to the database {domainArticle.Head}.");
                    }
                    catch (DbUpdateException)
                    {
                        Logger.LogWarning($"Article id {article.Id} alreydy exists!");
                    }
                }
            }
            catch (Exception e)
            {
                var att = message.Attempt + 1;
                if (att < this.countAttempt)
                {
                    this.requests.Send(new UriRequest(message.Id, att, message.Depth));
                    Logger.LogWarning(
                        $"{e.Message}. There are still attempts: {this.countAttempt - message.Attempt}");
                }
                else
                {
                    Logger.LogWarning($"{e.Message}. Attempts are no more!");
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
                Logger.LogWarning(e.Message);
            }
        }
    }
}