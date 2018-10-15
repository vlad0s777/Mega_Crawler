namespace Mega.Domain
{
    public class ArticleTag
    {
        public Article Article { get; set; }

        public int TagId { get; set; }

        public int ArticleId { get; set; }

        public Tag Tag { get; set; }
    }
}
