namespace Mega.Services.ContentCollector
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Mega.Messaging;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class ServiceContentCollector : IMessageProcessor<UriRequest>
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ServiceContentCollector>();

        private readonly int count_attempt;

        private readonly int count_limit;

        private readonly MessageBroker<UriRequest> requests;

        private readonly MessageBroker<UriBody> bodies;

        public ServiceContentCollector(
            MessageBroker<UriRequest> requests,
            MessageBroker<UriBody> bodies,
            HashSet<Uri> visitedUrls,
            Func<Uri, string> clientDelegate,
            Settings settings)
        {
            this.requests = requests;

            this.bodies = bodies;

            this.VisitedUrls = visitedUrls;

            this.RootUri = new Uri(settings.RootUriString, UriKind.Absolute);

            this.ClientDelegate = clientDelegate;

            this.count_limit = settings.CountLimit;

            this.count_attempt = settings.AttemptLimit;
        }

        private HashSet<Uri> VisitedUrls { get; }

        private Uri RootUri { get; }

        private Func<Uri, string> ClientDelegate { get; }

        public bool Run()
        {
            if (this.requests.TryReceive(out var uri))
            {
                if (this.VisitedUrls.Count == this.count_limit)
                {
                    Logger.LogDebug($"You have reached the limit of visited pages: {this.count_limit}");
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
                        if (att < this.count_attempt)
                        {
                            this.requests.Send(new UriRequest(uri.Uri, att, uri.Depth));
                            Logger.LogDebug($"{e.Message} in {uri.Uri}. There are still attempts: {this.count_attempt - uri.Attempt}");
                        }
                        else
                        {
                            Logger.LogWarning($"{e.Message} in {uri.Uri}. Attempts are no more!");
                        }
                    }
                }
            }

            Thread.Sleep(new Random().Next(5000, 15000));

            return true;
        }
    }
}