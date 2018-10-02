namespace Mega.Web.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Article
    {
        public int ArticleId { get; set; }

        public DateTime DateCreate { get; set; }

        public string Text { get; set; }

        public string Head { get; set; }

        public int OuterArticleId { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public int CountTags => this.Tags.Count();
    }
}
