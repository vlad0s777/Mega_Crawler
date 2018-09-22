namespace Mega.Domain
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Tag
    {
        public int TagId { get; set; }

        public string Uri { get; set; }

        public string Name { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();

        [NotMapped]
        public int CountArticles => this.ArticleTags.Count;
    }
}
