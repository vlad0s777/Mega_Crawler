using System;
using System.Text.RegularExpressions;
using Mega.Messaging;

namespace Mega.Services
{
    public class UrlFinder
    {
        private const string HrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";

        private readonly MessageBroker<UriAttempt> _messages;
        private readonly MessageBroker<UriBody> _reports;
        private int _depth;

        public UrlFinder(MessageBroker<UriAttempt> messages, MessageBroker<UriBody> reports)
        {
            _messages = messages;
            _reports = reports;
        }

        public bool Work(int depth = -1)
        {
            _depth++;
            if (_depth == depth)
            {
                _depth--;
                return false;
            }

            while (_reports.TryReceive(out var uri))
            {
                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    try
                    {
                        var absUri = new UriAttempt(new Uri(uri.Uri,
                            new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute)));
                        _messages.Send(absUri);
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