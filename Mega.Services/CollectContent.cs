using System;
using System.Collections.Generic;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class CollectContent
    {
        private readonly MessageBroker<UriAttempt> _messages;
        private readonly MessageBroker<UriBody> _reports;
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();
        public CollectContent(MessageBroker<UriAttempt> messages, MessageBroker<UriBody> reports,
            HashSet<Uri> visitedUrls,
            Uri rootUri, Func<Uri, string> clientDelegate)
        {
            _messages = messages;
            _reports = reports;
            VisitedUrls = visitedUrls;
            RootUri = rootUri;
            messages.Send(new UriAttempt(rootUri));
            ClientDelegate = clientDelegate;
        }

        private HashSet<Uri> VisitedUrls { get; }
        private Uri RootUri { get; }
        private Func<Uri, string> ClientDelegate { get; }

        public bool Work(int attempt = 0, int limit = -1)
        {
            while (_messages.TryReceive(out var uri))
            {
                if (VisitedUrls.Count == limit) return false;
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                        return false;
                if (RootUri.IsBaseOf(uri.Uri) && VisitedUrls.Add(uri.Uri))
                    try
                    {
                        var documentBody = ClientDelegate.Invoke(uri.Uri);
                        Logger.LogInformation($"OK {uri.Uri}");
                        _reports.Send(new UriBody(uri.Uri, documentBody));
                    }
                    catch (Exception)
                    {
                        VisitedUrls.Remove(uri.Uri);
                        uri.Attempt++;
                        if (uri.Attempt < attempt)
                            _messages.Send(uri);
                        Logger.LogWarning($"NO {uri.Uri}. Attempt №{uri.Attempt}");
                    }
            }

            return true;
        }
    }
}