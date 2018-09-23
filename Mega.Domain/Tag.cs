namespace Mega.Domain
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Tag
    {
        public int TagId { get; set; }

        public string TagKey { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; } = new List<ArticleTag>();

        [NotMapped]
        public IEnumerable<Article> Articles => this.ArticleTags.Select(e => e.Article);


        [NotMapped]
        public int CountArticles => this.ArticleTags.Count;
    }
}
