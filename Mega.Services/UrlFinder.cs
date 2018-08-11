using System;
using System.Text.RegularExpressions;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class UrlFinder
    {
        private const string HrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";
        private readonly int chech_depth;

        private readonly MessageBroker<UriAttempt> messages;
        private readonly MessageBroker<UriBody> reports;
        private int depth;

        public UrlFinder(MessageBroker<UriAttempt> messages, MessageBroker<UriBody> reports, int checkDepth = -1)
        {
            this.messages = messages;
            this.reports = reports;
            this.chech_depth = checkDepth;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();

        public bool Work(int depth = -1)
        {
            Logger.LogDebug("Start Work..");
            this.depth++;
            if (this.depth == depth)
            {
                this.depth--;
                Logger.LogDebug($"You have reached the depth of visited pages: {depth}");
                return false;
            }

            while (this.reports.TryReceive(out var uri))
            {
                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    try
                    {
                        var absUri = new UriAttempt(new Uri(uri.Uri,
                            new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute)));
                        this.messages.Send(absUri);
                    }
                    catch (Exception)
                    {
                        Logger.LogWarning($"Ignoring {m.Value}");
                    }

                    m = m.NextMatch();
                }
            }
            Logger.LogDebug("End Work.");
            return true;
        }
    }
}