namespace Mega.Services.WebClient
{
    using System.Collections.Generic;

    public class ClientPair
    {
        public string IdPrev { get; }

        public HashSet<ArticleInfo> Articles { get; }

        public ClientPair(string idPrev, HashSet<ArticleInfo> articles)
        {
            this.IdPrev = idPrev;
            this.Articles = articles;
        }
    }
}