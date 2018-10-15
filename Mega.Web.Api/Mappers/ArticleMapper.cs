namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Domain;
    using Mega.Web.Api.Models;

    public class ArticleMapper : IMapper<Article, ArticleModel>
    {
        private readonly IDataContext context;

        public ArticleMapper(IDataContext context)
        {
            this.context = context;
        }

        public ArticleModel Map(Article article)
        {
            return new ArticleModel()
                       {
                           ArticleId = article.ArticleId,
                           DateCreate = article.DateCreate,
                           Head = article.Head,
                           OuterArticleId = article.OuterArticleId,
                           Text = article.Text,
                           Tags = this.context.GetTags(articleId: article.ArticleId).Select(x => x.TagKey)
                       };
        }

        public IEnumerable<ArticleModel> Map(IEnumerable<Article> articles)
        {
            return articles.Select(Map);
        }
    }
}
