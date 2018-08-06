using System;
using System.Collections.Generic;
using System.Net;
using Mega.Messaging;

namespace Mega.Services
{
    public class Consumer
    {
        public Consumer(MessageBroker<Uri> messages, MessageBroker<UriBody> reports, HashSet<Uri> visitedUrls,
            Uri rootUri)
        {
            Messages = messages;
            Reports = reports;
            VisitedUrls = visitedUrls;
            RootUri = rootUri;
            messages.Send(rootUri);
        }

        public MessageBroker<Uri> Messages { get; set; }
        public MessageBroker<UriBody> Reports { get; set; }
        public HashSet<Uri> VisitedUrls { get; set; }
        public Uri RootUri { get; set; }

        public void Work()
        {
            using (var client = new WebClient())
            {
                while (Messages.TryReceive(out var uri))
                    if (RootUri.IsBaseOf(uri) && VisitedUrls.Add(uri))
                        try
                        {
                            var documentBody = client.DownloadString(uri);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"OK {uri}");
                            Reports.Send(new UriBody(uri, documentBody));
                        }
                        catch (Exception)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"NO {uri}");
                        }
            }
        }
    }
}