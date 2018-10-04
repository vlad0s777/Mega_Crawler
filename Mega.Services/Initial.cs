namespace Mega.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp.Parser.Html;

    using Mega.Domain;
    using Mega.Services.WebClient;

    public class Initial : IDisposable
    {
        private readonly Settings settings;

        private readonly IDataContext dataContext;

        public Initial(Settings settings, IDataContext dataContext)
        {
            this.settings = settings;
            this.dataContext = dataContext;
        }

        public IEnumerable<string> GenerateIDs(DateTime start)
        {
            var current = DateTime.Now;
            while (current >= start)
            {
                yield return current.Date.ToString("yyyyMMdd");
                current = current.AddDays(-1);
            }
        }

        public List<TagInfo> GetTags(Func<Uri, string> clientDelegate)
        {
            var tags = new List<TagInfo>();
            try
            {
                var body = clientDelegate(new Uri(this.settings.RootUriString + "tags", UriKind.Absolute));
                var parser = new HtmlParser();
                using (var document = parser.Parse(body))
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
                throw new Exception(e.Message);
            }

            return tags;
        }

        public async Task AddTagInBase()
        {
            if (await this.dataContext.CountTags() != 0)
            {
                return;
            }

            using (var client = new System.Net.WebClient())
            {
                foreach (var tag in GetTags(client.DownloadString))
                {
                    await this.dataContext.AddAsync(new Tag { Name = tag.Name, TagKey = tag.TagKey });
                }
            }

            await this.dataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}
