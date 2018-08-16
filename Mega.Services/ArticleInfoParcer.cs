using System;
using System.Collections.Generic;
using AngleSharp.Parser.Html;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class ArticleInfoParcer : IMessageProcessor
    {
        public readonly Dictionary<string, ArticleInfo> Info;
        private readonly int maxdepth;
        private readonly MessageBroker<UriBody> reports;

        public ArticleInfoParcer(Dictionary<string, ArticleInfo> info, MessageBroker<UriBody> reports, int maxdepth = -1)
        {
            this.Info = info;
            this.reports = reports;
            this.maxdepth = maxdepth;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ArticleInfoParcer>();

        public bool Run()
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

                    this.Info.Add(uri.Uri.LocalPath, new ArticleInfo(date, tagsDictionary, body, head));
                    Logger.LogDebug($"Add {head} document!");
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