using System;
using AngleSharp.Parser.Html;
using Mega.Messaging;
using Microsoft.Extensions.Logging;

namespace Mega.Services
{
    public class ArticleUrlParcer
    {
        private readonly MessageBroker<UriLimits> articles;
        private readonly int maxdepth;
        private readonly MessageBroker<UriLimits> messages;
        private readonly MessageBroker<UriBody> reports;

        public ArticleUrlParcer(MessageBroker<UriLimits> messages, MessageBroker<UriBody> reports,
            MessageBroker<UriLimits> articles, int checkDepth = -1)
        {
            this.messages = messages;
            this.articles = articles;
            this.reports = reports;
            this.maxdepth = checkDepth;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ArticleUrlParcer>();

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
                    var articleHrefCssSelector = document.QuerySelectorAll("div.story>h2>a");
                    foreach (var article in articleHrefCssSelector)
                    {
                        var hrefArticle = article.Attributes["href"].Value;
                        var depth = uri.Depth + 1;
                        var absUriArticle = new Uri(uri.Uri, new Uri(hrefArticle, UriKind.RelativeOrAbsolute));
                        this.articles.Send(new UriLimits(absUriArticle, 0, depth));
                        Logger.LogDebug($"Send in analiz {absUriArticle}");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e.Message);
                }

                try
                {
                    var prevPageCssSelector = document.QuerySelector("li.prev>a");
                    var hrefPrevPage = prevPageCssSelector.Attributes["href"].Value;
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