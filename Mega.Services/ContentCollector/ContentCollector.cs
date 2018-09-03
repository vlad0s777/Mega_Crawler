﻿namespace Mega.Services.ContentCollector
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class ContentCollector : IMessageProcessor<UriRequest>
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ContentCollector>();

        private readonly int countAttempt;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly IMessageBroker<UriBody> bodies;

        public ContentCollector(
            IMessageBroker<UriRequest> requests,
            IMessageBroker<UriBody> bodies,
            HashSet<Uri> visitedUrls,
            Func<Uri, string> clientDelegate,
            Settings settings)
        {
            this.requests = requests;

            this.bodies = bodies;

            this.VisitedUrls = visitedUrls;

            this.RootUri = new Uri(settings.RootUriString, UriKind.Absolute);

            this.ClientDelegate = clientDelegate;

            this.countAttempt = settings.AttemptLimit;
        }

        private HashSet<Uri> VisitedUrls { get; }

        private Uri RootUri { get; }

        private Func<Uri, string> ClientDelegate { get; }

        public void Handle(UriRequest message)
        {
            Logger.LogInformation($"Processing {message.Uri}");

            if (this.RootUri.IsBaseOf(message.Uri) && this.VisitedUrls.Add(message.Uri))
            {
                try
                {
                    var documentBody = this.ClientDelegate.Invoke(message.Uri);
                    Logger.LogInformation($"OK {message.Uri}");
                    this.bodies.Send(new UriBody(message.Uri, documentBody));
                }
                catch (Exception e)
                {
                    this.VisitedUrls.Remove(message.Uri);
                    var att = message.Attempt + 1;
                    if (att < this.countAttempt)
                    {
                        this.requests.Send(new UriRequest(message.Uri, att, message.Depth));
                        Logger.LogDebug($"{e.Message} in {message.Uri}. There are still attempts: {this.countAttempt - message.Attempt}");
                    }
                    else
                    {
                        Logger.LogWarning($"{e.Message} in {message.Uri}. Attempts are no more!");
                    }
                }
            }
        }

        public void Run() => this.requests.ConsumeWith(this.Handle);
    }
}