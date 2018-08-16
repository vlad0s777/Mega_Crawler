using System;
using System.Collections.Generic;
using AngleSharp.Parser.Html;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class ServiceInfoParcer
    {
        private static ILogger Logger { get; } = ApplicationLogging.CreateLogger<ServiceInfoParcer>();

        private readonly MessageBroker<UriBody> reports;

        public readonly Dictionary<string, ArticleInfo> Info;

        private readonly int maxdepth;

        public ServiceInfoParcer(Dictionary<string, ArticleInfo> info, MessageBroker<UriBody> reports, int maxdepth = -1)
        {
            this.Info = info;

            this.reports = reports;

            this.maxdepth = maxdepth;
        }



        public bool Work()
        {
            if (this.reports.TryReceive(out var uri))
            {
                if (uri.Depth == this.maxdepth)
                {
                    Logger.LogDebug($"In {uri.Uri} max depth. Next report..");
                    return false;
                }

                var parser = new HtmlParser();
                var document = parser.Parse(uri.Body);
                try
                {
                    var head = document.QuerySelector("div.story>h1").InnerHtml;
                    var date = DateTime.Parse(document.QuerySelector("div.story>div.meta>div.date-time").InnerHtml);
                    var body = document.QuerySelector("div.text").InnerHtml;
                    var tagsSelector = document.QuerySelectorAll("div.story>div.meta>div.tags>ul>li>a");
                    var tagsDictionary = new Dictionary<string, string>();

                    foreach (var selector in tagsSelector)
                    {
                        var href = selector.Attributes["href"].Value;
                        var text = selector.InnerHtml;

                        tagsDictionary.Add(href, text);
                    }

                    var artInfo = new ArticleInfo(date, tagsDictionary, body, head);
                    this.Info.Add(uri.Uri.LocalPath, artInfo);
   
                    Logger.LogInformation($"Add '{head}' document!");
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e.Message);
                }
            }

            return true;
        }
    }
}