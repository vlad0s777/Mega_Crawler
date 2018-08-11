using System;
using System.Collections.Generic;
using Mega.Messaging;

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

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work()
        {
            while (this.messages.TryReceive(out var uri))
            {
                if (this.VisitedUrls.Count == this.limit)
                {
                    return false;
                }

                if (this.RootUri.IsBaseOf(uri.Uri) && this.VisitedUrls.Add(uri.Uri))
                {
                    try
                    {
                        var documentBody = this.ClientDelegate.Invoke(uri.Uri);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"OK {uri.Uri} Depth: {uri.Depth}");
                        this.reports.Send(new UriBody(uri.Uri, documentBody, uri.Depth));
                    }
                    catch (Exception)
                    {
                        this.VisitedUrls.Remove(uri.Uri);
                        var att = uri.Attempt + 1;
                        if (att < this.attempt)
                        {
                            this.messages.Send(new UriLimits(uri.Uri, att, uri.Depth));
                        }

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"NO {uri.Uri}. There are still attempts: {this.attempt - uri.Attempt}");
                    }
                }
            }

            return true;
        }
    }
}