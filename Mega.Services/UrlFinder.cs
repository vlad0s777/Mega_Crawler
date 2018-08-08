using System;
using System.Text.RegularExpressions;
using Mega.Messaging;

namespace Mega.Services
{
    public class UrlFinder
    {
        private const string HrefPattern = "href\\s*=\\s*(?:[\"'](?<uri>[^\"']*)[\"'])";

        private readonly MessageBroker<Uri> _messages;
        private readonly MessageBroker<UriBody> _reports;

        public UrlFinder(MessageBroker<Uri> messages, MessageBroker<UriBody> reports)
        {
            _messages = messages;
            _reports = reports;
        }

        public bool Work()
        {
            while (_reports.TryReceive(out var uri))
            {
                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    if (Console.KeyAvailable)
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                            return false;
                    try
                    {
                        var absUri = new Uri(uri.Uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
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