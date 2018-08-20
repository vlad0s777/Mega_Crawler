namespace Mega.Services.InfoParser
{
    using System.Collections.Generic;

    public class WrapperArticles
    {
        public readonly Dictionary<string, ArticleInfo> Articles;

        public WrapperArticles()
        {
            this.Articles = new Dictionary<string, ArticleInfo>();
        }
    }
}
