namespace Mega.Web.Api.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Tag
    {
        public int TagId { get; set; }

        public string TagKey { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> Articles { get; set; }

        public int CountArticles => this.Articles.Count();
    }
}
