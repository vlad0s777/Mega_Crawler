namespace Mega.Services.UriRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.WebClient.ZadolbaliClient;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class UriRequestProcessor : IMessageProcessor<UriRequest>, IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<UriRequestProcessor>();

        private readonly int countAttempt;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly IDataContext dataContext; 

        public UriRequestProcessor(
            IMessageBroker<UriRequest> requests,
            ProxySettings settings,
            IDataContext dataContext)
        {
            this.requests = requests;

            this.dataContext = dataContext;

            this.RootUri = new Uri(settings.RootUriString, UriKind.Absolute);

            this.client = new ZadolbaliClient(settings);

            this.countAttempt = settings.AttemptLimit;
        }

        private Uri RootUri { get; }

        private readonly ZadolbaliClient client;

        public async Task Handle(UriRequest message)
        {
            Logger.LogInformation($"Processing {this.RootUri + message.Id}.");

            try
            {
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

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}