namespace Mega.Domain
{
    using System.Collections.Generic;

    public class Tag
    {
        public int TagId { get; set; }

        public string Uri { get; set; }

        public string Name { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; } = new List<ArticleTag>();
    }
}
