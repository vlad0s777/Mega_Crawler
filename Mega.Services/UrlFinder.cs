using System;
using System.Text.RegularExpressions;
using Mega.Messaging;

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

        public bool Work()
        {
            this.depth++;
            if (this.depth == this.chech_depth)
            {
                this.depth--;
                return false;
            }

            while (this.reports.TryReceive(out var uri))
            {
                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    try
                    {
                        var absUri = new Uri(uri.Uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
                        this.messages.Send(new UriAttempt(absUri));
                    }
                    catch (Exception)
                    {
                        Console.ResetColor();
                        Console.WriteLine($"Ignoring {m.Value}");
                    }

                    m = m.NextMatch();
                }
            }

            return true;
        }
    }
}