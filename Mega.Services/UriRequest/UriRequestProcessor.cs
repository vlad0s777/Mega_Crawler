namespace Mega.Services.UriRequest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Mega.Messaging;
    using Mega.Services.WebClient;

    using Microsoft.Extensions.Logging;

    public class UriRequestProcessor : IMessageProcessor<UriRequest>, IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<UriRequestProcessor>();

        private readonly int countAttempt;

        private readonly IMessageBroker<UriRequest> requests;

        public UriRequestProcessor(
            IMessageBroker<UriRequest> requests,
            HashSet<Uri> visitedUrls,
            ZadolbaliClient client,
            Settings settings)
        {
            this.requests = requests;

            this.VisitedUrls = visitedUrls;

            this.RootUri = new Uri(settings.RootUriString, UriKind.Absolute);

            this.client = client;

            this.countAttempt = settings.AttemptLimit;
        }

        private HashSet<Uri> VisitedUrls { get; }

        private Uri RootUri { get; }

        private readonly ZadolbaliClient client;

        public async Task Handle(UriRequest message)
        {
            Logger.LogInformation($"Processing {message.Uri}");

            if (!this.RootUri.IsBaseOf(message.Uri) || !this.VisitedUrls.Add(message.Uri))
            {
                return;
            }

            try
            {
                var body = await this.client.DownloadUrl(message.Uri);
                     
                var prevPage = await this.client.GetPrevPage(body); 
                this.requests.Send(new UriRequest(new Uri(this.RootUri, prevPage)));
                Logger.LogInformation($"OK {message.Uri}");

                foreach (var _ in await this.client.GetArticle(body))
                {
                    //что-то делаем со статьями
                }
            }
            catch (Exception e)
            {
                this.VisitedUrls.Remove(message.Uri);
                var att = message.Attempt + 1;
                if (att < this.countAttempt)
                {
                    this.requests.Send(new UriRequest(message.Uri, att, message.Depth));
                    Logger.LogWarning($"{e.Message} in {message.Uri}. There are still attempts: {this.countAttempt - message.Attempt}");
                }
                else
                {
                    Logger.LogWarning($"{e.Message} in {message.Uri}. Attempts are no more!");
                }
            }
        }

        public void Run() => this.requests.ConsumeWith(Handle);

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}