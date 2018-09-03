namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;

    using AngleSharp.Parser.Html;

    using Mega.Services.ContentCollector;

    using Microsoft.Extensions.Logging;

    public class IthappensClient
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<IthappensClient>();

        private Func<Uri, string> ClientDelegate { get; }

        private readonly Dictionary<string, ArticleInfo> articles;

        public IthappensClient(Dictionary<string, ArticleInfo> articles, Func<Uri, string> clientDelegate)
        {
            this.ClientDelegate = clientDelegate;
            this.articles = articles;
        }

        public UriRequest Handle(Uri uri)
        {
            var documentBody = this.ClientDelegate.Invoke(uri);
            Logger.LogInformation($"OK {uri}");
            GetArticles(documentBody);
            return GetPrevPage(documentBody);
        }

        public void GetArticles(string documentBody)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(documentBody);

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
        }

        public UriRequest GetPrevPage(string body)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(body);
            try
            {
                var hrefPrevPage = document.QuerySelector("li.prev>a").Attributes["href"].Value;
                var absUriPrevPage = new Uri(hrefPrevPage, UriKind.RelativeOrAbsolute);

                return new UriRequest(absUriPrevPage);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
                return null;
            }
        }
    }
}
