using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly int attempt;
        private readonly int limit;
        private readonly MessageBroker<UriLimits> messages;
        private readonly MessageBroker<UriBody> reports;


        public CollectContent(MessageBroker<UriLimits> messages, MessageBroker<UriBody> reports,
            HashSet<Uri> visitedUrls,
            Uri rootUri, Func<Uri, string> clientDelegate, int limit = -1, int attempt = 0)
        {
            this.messages = messages;
            this.reports = reports;
            this.VisitedUrls = visitedUrls;
            this.RootUri = rootUri;
            messages.Send(new UriLimits(rootUri));
            this.ClientDelegate = clientDelegate;
            this.limit = limit;
            this.attempt = attempt;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work()
        {
            Logger.LogDebug("Start Work..");
            while (this.messages.TryReceive(out var uri))
            {
                if (this.VisitedUrls.Count == this.limit)
                {
                    Logger.LogDebug($"You have reached the limit of visited pages: {this.limit}");
                    return false;
                }

                if (this.RootUri.IsBaseOf(uri.Uri) && this.VisitedUrls.Add(uri.Uri))
                {
                    try
                    {
                        Thread.Sleep(new Random().Next(5000, 15000));
                        var documentBody = this.ClientDelegate.Invoke(uri.Uri);
                        Logger.LogDebug($"OK {uri.Uri} Depth: {uri.Depth}");
                        this.reports.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception e)
                    {
                        this.VisitedUrls.Remove(uri.Uri);
                        var att = uri.Attempt + 1;
                        if (att < this.attempt)
                        {
                            this.messages.Send(new UriLimits(uri.Uri, att, uri.Depth));
                            Logger.LogDebug(
                                $"{e.Message} in {uri.Uri}. There are still attempts: {this.attempt - uri.Attempt}");
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