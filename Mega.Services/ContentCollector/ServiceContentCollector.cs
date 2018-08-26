﻿namespace Mega.Services.ContentCollector
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class ServiceContentCollector : IMessageProcessor
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ServiceContentCollector>();

        private readonly int countAttempt;

        private readonly int countLimit;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly IMessageBroker<UriBody> bodies;

        public ServiceContentCollector(
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

            this.countLimit = settings.CountLimit;

            this.countAttempt = settings.AttemptLimit;
        }

        private HashSet<Uri> VisitedUrls { get; }

        private Uri RootUri { get; }

        private Func<Uri, string> ClientDelegate { get; }

        public bool Run()
        {
            if (this.requests.TryReceive(out var uri))
            {
                Logger.LogInformation($"Processed is {uri.Uri}");
                if (this.VisitedUrls.Count == this.countLimit)
                {
                    Logger.LogDebug($"You have reached the limit of visited pages: {this.countLimit}");
                    return false;
                }

                if (this.RootUri.IsBaseOf(uri.Uri) && this.VisitedUrls.Add(uri.Uri))
                {
                    try
                    {
                        var documentBody = this.ClientDelegate.Invoke(uri.Uri);
                        Logger.LogInformation($"OK {uri.Uri}");
                        this.bodies.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception e)
                    {
                        this.VisitedUrls.Remove(uri.Uri);
                        var att = uri.Attempt + 1;
                        if (att < this.countAttempt)
                        {
                            this.requests.Send(new UriRequest(uri.Uri, att, uri.Depth));
                            Logger.LogDebug($"{e.Message} in {uri.Uri}. There are still attempts: {this.countAttempt - uri.Attempt}");
                        }
                        else
                        {
                            Logger.LogWarning($"{e.Message} in {uri.Uri}. Attempts are no more!");
                        }
                    }
                }
            }

            return true;
        }
    }
}