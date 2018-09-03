namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using AngleSharp.Parser.Html;

    using Mega.Services.InfoParser;

    using Microsoft.Extensions.Logging;

    public class IthappensClient : WebClient
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<IthappensClient>();

        public Dictionary<string, ArticleInfo> Articles { get; set; }

        public string HrefPrevPage { get; set; }

        public IthappensClient()
        {
            this.Articles = new Dictionary<string, ArticleInfo>();
            base.UploadString()
        }

        protected override void OnDownloadStringCompleted(DownloadStringCompletedEventArgs args)
        {
            var message = args.Result;
            var parser = new HtmlParser();
            var document = parser.Parse(message);

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
                        this.Articles.Add(urlArticle.Value, artInfo);
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
                this.HrefPrevPage = document.QuerySelector("li.prev>a").Attributes["href"].Value;
                //var absUriPrevPage = new Uri(message.Uri, new Uri(hrefPrevPage, UriKind.RelativeOrAbsolute));

                //this.requests.Send(new UriRequest(absUriPrevPage));
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
            }
            base.OnDownloadStringCompleted(args);
        }
        
    }
}
