using System;
using System.Collections.Generic;
using System.Threading;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class ServiceCollectContent
    {
        private static ILogger Logger { get; } = ApplicationLogging.CreateLogger<ServiceCollectContent>();

        private readonly int countAttempt;

        private readonly int countLimit;

        private readonly MessageBroker<UriLimits> messages;

        private readonly MessageBroker<UriBody> reports;

        private readonly bool is_timeout;

        public ServiceCollectContent(MessageBroker<UriLimits> messages, MessageBroker<UriBody> reports, HashSet<Uri> visitedUrls, Uri rootUri,
            Func<Uri, string> clientDelegate, int countLimit = -1, int countAttempt = 0, bool timeout = false)
        {
            this.messages = messages;
            this.reports = reports;

            this.VisitedUrls = visitedUrls;

            this.RootUri = rootUri;

            messages.Send(new UriLimits(rootUri));

            this.ClientDelegate = clientDelegate;

            this.countLimit = countLimit;
            this.countAttempt = countAttempt;
            this.is_timeout = timeout;
        }

        private HashSet<Uri> VisitedUrls { get; }

        private Uri RootUri { get; }

        private Func<Uri, string> ClientDelegate { get; }

        public bool Work()
        {
            if (this.messages.TryReceive(out var uri))
            {
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
                        Logger.LogInformation($"OK {uri.Uri} Depth: {uri.Depth}");
                        this.reports.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception e)
                    {
                        this.VisitedUrls.Remove(uri.Uri);
                        var att = uri.Attempt + 1;
                        if (att < this.countAttempt)
                        {
                            this.messages.Send(new UriLimits(uri.Uri, att, uri.Depth));
                            Logger.LogDebug($"{e.Message} in {uri.Uri}. There are still attempts: {this.countAttempt - uri.Attempt}");
                        }
                        else
                        {
                            Logger.LogWarning($"{e.Message} in {uri.Uri}. Attempts are no more!");
                        }
                    }
                }
            }

            if (this.is_timeout)
            {
                Thread.Sleep(new Random().Next(5000, 15000));
            }

            return true;
        }
    }
}