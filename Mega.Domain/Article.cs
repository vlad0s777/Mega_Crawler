namespace Mega.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Article
    {
        public int ArticleId { get; set; }

        public DateTime DateCreate { get; set; }

        public string Text { get; set; }

        public string Head { get; set; }

        public int OuterArticleId { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; } = new List<ArticleTag>();

        [NotMapped]
        public IEnumerable<Tag> Tags => this.ArticleTags.Select(e => e.Tag);


        [NotMapped]
        public int CountTags => this.ArticleTags.Count;
    }
}
