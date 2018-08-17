namespace Mega.Services
{
    using System;
    using System.Collections.Generic;

    using AngleSharp.Parser.Html;

    using Mega.Messaging;

    using Microsoft.Extensions.Logging;

    public class ServiceInfoParcer
    {
        private static ILogger Logger { get; } = ApplicationLogging.CreateLogger<ServiceInfoParcer>();

        private readonly MessageBroker<UriLimits> messages;

        private readonly MessageBroker<UriBody> reports;

        public readonly Dictionary<string, ArticleInfo> Info;

        private readonly int maxdepth;

        public ServiceInfoParcer(MessageBroker<UriLimits> messages, MessageBroker<UriBody> reports, Dictionary<string, ArticleInfo> info, int maxdepth = -1)
        {
            this.Info = info;

            this.reports = reports;

            this.maxdepth = maxdepth;

            this.messages = messages;
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
                    var articleBody = document.QuerySelectorAll("div.story");
                    foreach (var article in articleBody)
                    {
                        try
                        {
                            var articleDoc = parser.Parse(article.InnerHtml);
                            var head = articleDoc.QuerySelector("h2").TextContent;
                            var urlArticle = articleDoc.QuerySelector("h2>a").Attributes["href"];
                            var date = DateTime.Parse(articleDoc.QuerySelector("div.meta>div.date-time").InnerHtml);
                            var body = articleDoc.QuerySelector("div.text").InnerHtml;
                            var tagsSelector = articleDoc.QuerySelectorAll("div.meta>div.tags>ul>li>a");
                            var tagsDictionary = new Dictionary<string, string>();
                            foreach (var selector in tagsSelector)
                            {
                                var href = selector.Attributes["href"].Value;
                                var text = selector.InnerHtml;

                                tagsDictionary.Add(href, text);
                            }

                            var artInfo = new ArticleInfo(date, tagsDictionary, body, head);
                            this.Info.Add(urlArticle.Value, artInfo);
                            Logger.LogInformation($"Add '{head}' document!");
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e.Message);
                }

                try
                {
                    var hrefPrevPage = document.QuerySelector("li.prev>a").Attributes["href"].Value;
                    var absUriPrevPage = new Uri(uri.Uri, new Uri(hrefPrevPage, UriKind.RelativeOrAbsolute));

                    this.messages.Send(new UriLimits(absUriPrevPage));
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