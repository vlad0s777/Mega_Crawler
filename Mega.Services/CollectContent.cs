using System;
using System.Collections.Generic;
using Mega.Messaging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly int limit;
        private readonly MessageBroker<UriAttempt> messages;
        private readonly MessageBroker<UriBody> reports;

        public CollectContent(MessageBroker<Uri> messages, MessageBroker<UriBody> reports, HashSet<Uri> visitedUrls,
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

                if (this.RootUri.IsBaseOf(uri) && this.VisitedUrls.Add(uri))
                {
                    try
                    {
                        var documentBody = ClientDelegate.Invoke(uri.Uri);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"OK {uri}");
                        this.reports.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception)
                    {
                        VisitedUrls.Remove(uri.Uri);
                        uri.Attempt++;
                        if (uri.Attempt < attempt)
                            _messages.Send(uri);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"NO {uri.Uri}. Attempt №{uri.Attempt}");
                    }
                }
            }

            return true;
        }
    }
}