namespace Mega.Services.UriRequest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Messaging;
    using Mega.Services.WebClient;

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
            Settings settings,
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
                    Article domainArticle;
                    try
                    {
                        await this.dataContext.Articles.FirstAsync(t => t.ArticleId == Convert.ToInt32(article.Url));
                        continue;
                    }
                    catch (Exception)
                    {
                        domainArticle = new Article()
                                            {
                                                ArticleId = Convert.ToInt32(article.Url),
                                                DateCreate = article.DateCreate,
                                                Head = article.Head,
                                                Text = article.Text
                                            };
                    }

                    foreach (var tag in article.Tags)
                    {
                        Tag domainTag;
                        try
                        {
                            domainTag = await this.dataContext.Tags.FirstAsync(t => t.TagKey == tag.Key);
                        }
                        catch (Exception)
                        {
                            domainTag = new Tag() { Name = tag.Value, TagKey = tag.Key };
                        }

                        await this.dataContext.AddAsync(new ArticleTag { Article = domainArticle, Tag = domainTag });
                        Logger.LogInformation($"{await this.dataContext.SaveChangesAsync()}. Speed: {DownloadStatistic.Speed()}");
                    }
                }

                
            }
            catch (Exception e)
            {
                var att = message.Attempt + 1;
                if (att < this.countAttempt)
                {
                    this.requests.Send(new UriRequest(message.Id, att, message.Depth));
                    Logger.LogWarning($"{e.Message}. There are still attempts: {this.countAttempt - message.Attempt} Speed: {DownloadStatistic.Speed()}");
                }
                else
                {
                    Logger.LogWarning($"{e.Message}. Attempts are no more! Speed: {DownloadStatistic.Speed()}");
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
            this.dataContext?.Dispose();
        }
    }
}