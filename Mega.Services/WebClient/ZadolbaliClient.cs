namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp.Parser.Html;

    using Microsoft.Extensions.Logging;

    public class ZadolbaliClient : IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ZadolbaliClient>();

        public static DateTime GetDate(string specificDate)
        {
            var parts = specificDate.Split(new char[] { ',', ':' });
            switch (parts.First().ToLower())
            {
                case "сегодня":
                    return DateTime.Today.AddHours(Convert.ToDouble(parts[1])).AddMinutes(Convert.ToDouble(parts[2]));
                case "вчера":
                    return DateTime.Today.AddDays(-1).AddHours(Convert.ToDouble(parts[1])).AddMinutes(Convert.ToDouble(parts[2]));
                default:
                    return DateTime.Parse(parts[0]).AddHours(Convert.ToDouble(parts[1])).AddMinutes(Convert.ToDouble(parts[2]));
            }
        }

        private readonly Func<Uri, Task<string>> clientDelegate;

        private readonly ProxyWebClient client;

        public ZadolbaliClient(Settings settings)
        {
            this.client = new ProxyWebClient(settings);
            this.clientDelegate = uri => this.client.GetStringAsync(uri);
        }

        public ZadolbaliClient(Func<Uri, Task<string>> clientDelegate) => this.clientDelegate = clientDelegate;

        public async Task<string> DownloadUrl(Uri uri) => await this.clientDelegate.Invoke(uri);

        public async Task<HashSet<ArticleInfo>> GetArticle(string body)
        {
            var articles = new HashSet<ArticleInfo>();
            var parser = new HtmlParser();
            var articleBody = (await parser.ParseAsync(body)).QuerySelectorAll("div.story");
            foreach (var article in articleBody)
            {
                var articleDoc = await parser.ParseAsync(article.InnerHtml);
                var head = articleDoc.QuerySelector("h2").TextContent;
                var urlArticle = articleDoc.QuerySelector("h2>a").Attributes["href"];
                var date = GetDate(articleDoc.QuerySelector("div.meta>div.date-time").InnerHtml);

                var content = articleDoc.QuerySelector("div.text").InnerHtml;
                var tagsSelector = articleDoc.QuerySelectorAll("div.meta>div.tags>ul>li>a");
                var tagsDictionary = new Dictionary<string, string>();
                foreach (var selector in tagsSelector)
                {
                    var href = selector.Attributes["href"].Value;
                    var text = selector.InnerHtml;

                    tagsDictionary.Add(href, text);
                }

                articles.Add(new ArticleInfo(date, tagsDictionary, content, head, urlArticle.Value));
                Logger.LogInformation($"Add '{head}' document!");                             
            }

            return articles;
        }

        public async Task<Uri> GetPrevPage(string body)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(body);
            try
            {
                var hrefPrevPage = document.QuerySelector("li.prev>a").Attributes["href"].Value;
                var absUriPrevPage = new Uri(hrefPrevPage, UriKind.RelativeOrAbsolute);

                return absUriPrevPage;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
                return null;
            }
        }

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}
