using System;
using System.Collections.Generic;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly int limit;
        private readonly MessageBroker<UriAttempt> messages;
        private readonly MessageBroker<UriBody> reports;


        public CollectContent(MessageBroker<UriAttempt> messages, MessageBroker<UriBody> reports,
            HashSet<Uri> visitedUrls,
            Uri rootUri, Func<Uri, string> clientDelegate, int limit = -1)
        {
            this.messages = messages;
            this.reports = reports;
            this.VisitedUrls = visitedUrls;
            this.RootUri = rootUri;
            messages.Send(rootUri);
            this.ClientDelegate = clientDelegate;
            this.limit = limit;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work()
        {
            while (this.messages.TryReceive(out var uri))
            {
                if (this.VisitedUrls.Count == this.limit)
                {
                    Logger.LogDebug($"You have reached the limit of visited pages: {limit}");
                    return false;
                }

                if (this.RootUri.IsBaseOf(uri) && this.VisitedUrls.Add(uri))
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