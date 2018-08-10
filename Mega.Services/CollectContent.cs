using System;
using System.Collections.Generic;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly MessageBroker<UriAttempt> messages;
        private readonly MessageBroker<UriBody> reports;

        public CollectContent(MessageBroker<UriAttempt> messages, MessageBroker<UriBody> reports,
            HashSet<Uri> visitedUrls,
            Uri rootUri, Func<Uri, string> clientDelegate)
        {
            this.messages = messages;
            this.reports = reports;
            this.VisitedUrls = visitedUrls;
            this.RootUri = rootUri;
            messages.Send(new UriAttempt(rootUri));
            this.ClientDelegate = clientDelegate;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work(int attempt = 0, int limit = -1)
        {
            Logger.LogDebug("Start Work..");
            while (this.messages.TryReceive(out var uri))
            {
                if (this.VisitedUrls.Count == limit)
                {
                    Logger.LogDebug($"You have reached the limit of visited pages: {limit}");
                    return false;
                }

                if (this.RootUri.IsBaseOf(uri.Uri) && this.VisitedUrls.Add(uri.Uri))
                {
                    try
                    {
                        var documentBody = this.ClientDelegate.Invoke(uri.Uri);
                        Logger.LogDebug($"OK {uri.Uri}");
                        this.reports.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception e)
                    {
                        this.VisitedUrls.Remove(uri.Uri);
                        uri.Attempt++;
                        if (uri.Attempt < attempt)
                        {
                            this.messages.Send(uri);
                            Logger.LogDebug($"{e.Message} in {uri.Uri}. Еhere are still attempts: {attempt-uri.Attempt}");
                        }
                        else
                        {
                            Logger.LogWarning($"{e.Message} in {uri.Uri}. Attempts are no more!");
                        }
                        
                    }
                }
            }
            Logger.LogDebug("End Work.");
            return true;
        }
    }
}