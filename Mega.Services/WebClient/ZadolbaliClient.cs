namespace Mega.Services.WebClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp.Parser.Html;

    using Microsoft.Extensions.Logging;

    public class ZadolbaliClient : IDisposable
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ZadolbaliClient>();

        private static readonly Stopwatch Watch = new Stopwatch();

        public static DateTime GetDate(string specificDate)
        {
            var parts = specificDate.Split(',', ':');
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

        private readonly Func<string, Task<string>> clientDelegate;

        private readonly ProxyWebClient client;

        public ZadolbaliClient(Settings settings)
        {
            this.client = new ProxyWebClient(settings);
            this.clientDelegate = id => this.client.GetStringAsync(id);
        }

        public ZadolbaliClient(Func<string, Task<string>> clientDelegate) => this.clientDelegate = clientDelegate;

        public async Task<PageOf<ArticleInfo>> GetArticles(string idPage)
        {
            var articles = new PageOf<ArticleInfo>(idPage);
            try
            {
                var body = await this.clientDelegate.Invoke(idPage);

                Watch.Start();

                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(body))
                {
                    var articleBody = document.QuerySelectorAll("div.story");

                    foreach (var article in articleBody)
                    {
                        try
                        {
                            var articleDoc = await parser.ParseAsync(article.InnerHtml);

                            var head = articleDoc.QuerySelector("h2").TextContent;
                            var urlArticle = articleDoc.QuerySelector("h2>a").Attributes["href"];
                            var date = GetDate(articleDoc.QuerySelector("div.meta>div.date-time").InnerHtml);
                            var content = articleDoc.QuerySelector("div.text").TextContent;
                            var tagsSelector = articleDoc.QuerySelectorAll("div.meta>div.tags>ul>li>a");
                            var tags = new List<TagInfo>();
                            foreach (var selector in tagsSelector)
                            {
                                var key = selector.Attributes["href"].Value.Split("/").Last();
                                var text = selector.InnerHtml;

                                tags.Add(new TagInfo(key, text));
                            }

                            articles.Add(new ArticleInfo(date, tags, content, head, Convert.ToInt32(urlArticle.Value.Split("/").Last())));
                            Logger.LogInformation($"Add '{head}' document! Speed: {DownloadStatistic.Speed()}");
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Watch.Reset();
                throw new Exception(e.Message);
            }

            Logger.LogDebug($"Parsing pages: {Watch.Elapsed.TotalMilliseconds} ms.");
            Watch.Reset();
            return articles;
        }

        public void Dispose()
        {
            this.client?.Dispose();
        }
    }
}
