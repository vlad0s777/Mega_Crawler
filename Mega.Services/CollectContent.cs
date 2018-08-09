using System;
using System.Collections.Generic;
using Mega.Messaging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly MessageBroker<Uri> _messages;
        private readonly MessageBroker<UriBody> _reports;

        public CollectContent(MessageBroker<Uri> messages, MessageBroker<UriBody> reports, HashSet<Uri> visitedUrls,
            Uri rootUri, Func<Uri, string> clientDelegate)
        {
            _messages = messages;
            _reports = reports;
            VisitedUrls = visitedUrls;
            RootUri = rootUri;
            messages.Send(rootUri);
            ClientDelegate = clientDelegate;
        }

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work()
        {
            while (_messages.TryReceive(out var uri))
                if (RootUri.IsBaseOf(uri) && VisitedUrls.Add(uri))
                    try
                    {
                        var documentBody = ClientDelegate.Invoke(uri);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"OK {uri}");
                        _reports.Send(new UriBody(uri, documentBody));
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"NO {uri}");
                    }

            return true;
        }
    }
}