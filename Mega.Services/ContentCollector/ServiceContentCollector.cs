namespace Mega.Services.ContentCollector
{
    using System;
    using System.Collections.Generic;

    using Mega.Messaging;
    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class ServiceContentCollector : IMessageProcessor
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ServiceContentCollector>();

        private readonly int count_attempt;

        private readonly int count_limit;

        private readonly IMessageBroker<UriRequest> requests;

        private readonly IMessageBroker<UriBody> bodies;

        public ServiceContentCollector(
            IMessageBroker<UriRequest> requests,
            IMessageBroker<UriBody> bodies,
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

        public void Handle(UriRequest message)
        {
            if (this.RootUri.IsBaseOf(message.Uri) && this.VisitedUrls.Add(message.Uri))
            {
                try
                {
                    var documentBody = this.ClientDelegate.Invoke(message.Uri);
                    Logger.LogInformation($"OK {message.Uri}");
                    this.bodies.Send(new UriBody(message.Uri, documentBody));
                }
                catch (Exception e)
                {
                    this.VisitedUrls.Remove(message.Uri);
                    var att = message.Attempt + 1;
                    if (att < this.count_attempt)
                    {
                        this.requests.Send(new UriRequest(message.Uri, att, message.Depth));
                        Logger.LogDebug($"{e.Message} in {message.Uri}. There are still attempts: {this.count_attempt - message.Attempt}");
                    }
                    else
                    {
                        Logger.LogWarning($"{e.Message} in {message.Uri}. Attempts are no more!");
                    }
                }
            }
        }

        public bool Run()
        {
            if (this.VisitedUrls.Count == this.count_limit)
            {
                Logger.LogDebug($"You have reached the limit of visited pages: {this.count_limit}");
                return false;
            }

            this.requests.ConsumeWith(Handle);
            return true;
        }
    }
}