﻿namespace Mega.Services.UriRequest
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
            HashSet<string> visitedUrls,
            ZadolbaliClient client,
            Settings settings)
        {
            this.requests = requests;

            this.VisitedUrls = visitedUrls;

            this.RootUri = new Uri(settings.RootUriString, UriKind.Absolute);

            this.client = client;

            this.countAttempt = settings.AttemptLimit;
        }

        private HashSet<string> VisitedUrls { get; }

        private Uri RootUri { get; }

        private readonly ZadolbaliClient client;

        public async Task Handle(UriRequest message)
        {
            Logger.LogInformation($"Processing {this.RootUri + message.Id}");

            if (!this.VisitedUrls.Add(message.Id))
            {
                return;
            }

            try
            {
                var page = await this.client.GetArticles(message.Id);
                var t = page.PrevPage.Id;
                Logger.LogInformation($"OK {this.RootUri + t}");
                this.requests.Send(new UriRequest(t));
                

//                foreach (var _ in page)
//                {
//                    //что-то делаем со статьями
//                }
            }
            catch (Exception e)
            {
                this.VisitedUrls.Remove(message.Id);
                var att = message.Attempt + 1;
                if (att < this.countAttempt)
                {
                    this.requests.Send(new UriRequest(message.Id, att, message.Depth));
                    Logger.LogWarning($"{e.Message} in {message.Id}. There are still attempts: {this.countAttempt - message.Attempt}");
                }
                else
                {
                    Logger.LogWarning($"{e.Message} in {message.Id}. Attempts are no more!");
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