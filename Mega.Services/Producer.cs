using System;
using System.Text.RegularExpressions;
using Mega.Messaging;

namespace Mega.Services
{
    public class Producer
    {
        public Producer(MessageBroker<Uri> messages, MessageBroker<UriBody> reports, string hrefPattern)
        {
            Messages = messages;
            Reports = reports;
            HrefPattern = hrefPattern;
        }

        public string HrefPattern { get; set; }

        public MessageBroker<Uri> Messages { get; set; }
        public MessageBroker<UriBody> Reports { get; set; }

        public void Work()
        {
            while (Reports.TryReceive(out var uri))
            {
                var m = Regex.Match(uri.Body, HrefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                while (m.Success)
                {
                    try
                    {
                        var absUri = new Uri(uri.Uri, new Uri(m.Groups["uri"].Value, UriKind.RelativeOrAbsolute));
                        Messages.Send(absUri);
                    }
                    catch (Exception)
                    {
                        Console.ResetColor();
                        Console.WriteLine($"Ignoring {m.Value}");
                    }

                    m = m.NextMatch();
                }
            }
        }
    }
}