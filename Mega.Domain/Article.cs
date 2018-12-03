namespace Mega.Domain
{
    using System;

    public class Article
    {
        public int ArticleId { get; set; }

        public DateTime DateCreate { get; set; }

        public string Text { get; set; }

        public string Head { get; set; }

        public int OuterArticleId { get; set; }
    }
}
