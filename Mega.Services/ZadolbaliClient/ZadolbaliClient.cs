namespace Mega.Services.ZadolbaliClient
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
        public const string RootUriString = "https://zadolba.li/";

        public const int CountAttempt = 3;

        private const int DelayBegin = 5000;

        private const int DelayEnd = 15000;

        private const int Timeout = 8000;

        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<ZadolbaliClient>();

        private static readonly Stopwatch Watch = new Stopwatch();

        private readonly Func<string, Task<string>> clientDelegate;

        private readonly ProxyWebClient client;

        private readonly Random random;

        private int DelayGenerate() => this.random.Next(DelayBegin, DelayEnd);

        public ZadolbaliClient(int seed)
        {
            this.random = new Random(seed);
            this.client = new ProxyWebClient(RootUriString, Timeout);
            this.clientDelegate = id => this.client.GetStringAsync(id);
        }

        public ZadolbaliClient(int seed, int timeout = 0, string proxy = "")
        {
            this.random = new Random(seed);
            this.client = new ProxyWebClient(RootUriString, timeout, proxy);
            this.clientDelegate = id => this.client.GetStringAsync(id);
        }

        public ZadolbaliClient(Func<string, Task<string>> clientDelegate)
        {
            this.clientDelegate = clientDelegate;
            this.random = new Random();
            this.client = new ProxyWebClient(RootUriString, Timeout);
        }

        public string Proxy
        {
            get => this.client.ProxyServer;
            set => this.client.ProxyServer = value;
        }

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

        public async Task<List<string>> GenerateIDs()
        {
            DateTime start;
            try
            {
                Watch.Start();
                this.client.Delay = DelayGenerate();
                var body = await this.clientDelegate.Invoke(string.Empty);
                var parser = new HtmlParser();
                
                using (var document = await parser.ParseAsync(body))
                {
                    var firsttagSelector = document.QuerySelector("ul>li.first>a");
                    start = DateTime.ParseExact(firsttagSelector.Attributes["href"].Value.Split("/").Last(), "yyyyMMdd", null);
                }
            }
            catch (Exception e)
            {
                Watch.Reset();
                throw new Exception(e.Message + e.Source);
            }

            var current = DateTime.Now;
            var ids = new List<string>();
            while (current >= start)
            {
                ids.Add(current.Date.ToString("yyyyMMdd"));
                current = current.AddDays(-1);
            }

            Logger.LogDebug($"Generate ids: {Watch.Elapsed.TotalMilliseconds} ms.");

            Watch.Reset();
            return ids;
        }

        public async Task<List<TagInfo>> GetTags()
        {
            var tags = new List<TagInfo>();
            try
            {
                Watch.Start();
                this.client.Delay = DelayGenerate();
                var body = await this.clientDelegate.Invoke("tags");
                var parser = new HtmlParser();
                using (var document = await parser.ParseAsync(body))
                {
                    var tagsSelector = document.QuerySelectorAll("#cloud li>a");
                    foreach (var selector in tagsSelector)
                    {
                        var key = selector.Attributes["href"].Value.Split("/").Last();
                        var text = selector.InnerHtml;
                        tags.Add(new TagInfo(key, text));
                    }
                }
            }
            catch (Exception e)
            {
                Watch.Reset();
                throw new Exception(e.Message);
            }

            Logger.LogDebug($"Parsing tags: {Watch.Elapsed.TotalMilliseconds} ms.");

            Watch.Reset();
            return tags;
        }

        public async Task<PageOf<ArticleInfo>> GetArticles(string idPage)
        {
            var articles = new PageOf<ArticleInfo>(idPage);
            try
            {
                Watch.Start();

                this.client.Delay = DelayGenerate();
                var body = await this.clientDelegate.Invoke(idPage);                
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
