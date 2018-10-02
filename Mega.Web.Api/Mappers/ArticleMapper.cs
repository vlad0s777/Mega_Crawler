namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    public class ArticleMapper : IMapper<Domain.Article, Models.Article>
    {
        private readonly Domain.IDataContext context;

        public ArticleMapper(Domain.IDataContext context)
        {
            this.context = context;
        }

        public Models.Article Map(Domain.Article article)
        {
            return new Models.Article()
                       {
                           ArticleId = article.ArticleId,
                           DateCreate = article.DateCreate,
                           Head = article.Head,
                           OuterArticleId = article.OuterArticleId,
                           Text = article.Text,
                           Tags = this.context.ArticlesTags.Where(x => x.ArticleId == article.ArticleId).Select(y => y.Tag.Name)
                       };
        }

        public IEnumerable<Models.Article> Map(IEnumerable<Domain.Article> articles)
        {
            return articles.Select(Map);
        }
    }
}
