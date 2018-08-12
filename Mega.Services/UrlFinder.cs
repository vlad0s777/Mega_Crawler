using System;
using System.Text.RegularExpressions;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class UrlFinder
    {
        private const string HrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";
        private readonly int maxdepth;

        private readonly MessageBroker<UriLimits> messages;
        private readonly MessageBroker<UriBody> reports;

        public UrlFinder(MessageBroker<UriLimits> messages, MessageBroker<UriBody> reports, int checkDepth = -1)
        {
            this.messages = messages;
            this.reports = reports;
            this.maxdepth = checkDepth;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<CollectContent>();

        public bool Work()
        {
            Logger.LogDebug("Start Work..");
            while (this.reports.TryReceive(out var uri))
            {
                if (uri.Depth == this.maxdepth)
                {
                    Logger.LogDebug($"In {uri.Uri} max depth. Next report..");
                    continue;
                }

                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    try
                    {
                        var depth = uri.Depth + 1;
                        var absUri = new Uri(uri.Uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
                        this.messages.Send(new UriLimits(absUri, 0, depth));
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