namespace Mega.Domain
{
    using System.Collections.Generic;

    public class Tag
    {
        public int TagId { get; set; }

        public string TagKey { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ArticleTag> ArticleTag { get; } = new List<ArticleTag>();

        public RemovedTag RemovedTag { get; set; }
    }
}
