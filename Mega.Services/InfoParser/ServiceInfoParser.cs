namespace Mega.Services.InfoParser
{
    using System;
    using System.Collections.Generic;

    using AngleSharp.Parser.Html;

    using Mega.Messaging;
    using Mega.Services.ContentCollector;

    using Microsoft.Extensions.Logging;

    public class ServiceInfoParser : IMessageProcessor
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ServiceInfoParser>();

        private readonly MessageBroker<UriRequest> requests;

        private readonly MessageBroker<UriBody> bodies;

        private readonly Dictionary<string, ArticleInfo> articles;

        private readonly int maxdepth;

        public ServiceInfoParser(IMessageBroker<UriRequest> requests, IMessageBroker<UriBody> bodies, Dictionary<string, ArticleInfo> articles, Settings settings = null)
        {
            this.articles = articles;

            this.bodies = (MessageBroker<UriBody>)bodies;

            if (settings != null)
            {
                this.maxdepth = settings.DepthLimit;
            }
            else
            {
                this.maxdepth = -1;
            }

            this.requests = (MessageBroker<UriRequest>)requests;
        }

        public bool Run()
        {
            if (this.bodies.TryReceive(out var body))
            {
                if (body.Depth == this.maxdepth)
                {
                    Logger.LogDebug($"In {body.Uri} max depth. Next report..");
                    return false;
                }

                var parser = new HtmlParser();
                var document = parser.Parse(body.Body);

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
                            var content = articleDoc.QuerySelector("div.text").InnerHtml;
                            var tagsSelector = articleDoc.QuerySelectorAll("div.meta>div.tags>ul>li>a");
                            var tagsDictionary = new Dictionary<string, string>();
                            foreach (var selector in tagsSelector)
                            {
                                var href = selector.Attributes["href"].Value;
                                var text = selector.InnerHtml;

                                tagsDictionary.Add(href, text);
                            }

                            var artInfo = new ArticleInfo(date, tagsDictionary, content, head);
                            this.articles.Add(urlArticle.Value, artInfo);
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
                    var absUriPrevPage = new Uri(body.Uri, new Uri(hrefPrevPage, UriKind.RelativeOrAbsolute));

                    this.requests.Send(new UriRequest(absUriPrevPage));
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