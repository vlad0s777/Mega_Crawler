namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
    using Mega.Web.Api.Models;

    public class ArticleMapper : IMapper<Article, ArticleModel>
    {
        private readonly ITagRepository tagRepository;

        public ArticleMapper(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<ArticleModel> Map(Article article)
        {
            return new ArticleModel()
                       {
                           ArticleId = article.ArticleId,
                           DateCreate = article.DateCreate,
                           Head = article.Head,
                           OuterArticleId = article.OuterArticleId,
                           Text = article.Text,
                           Tags = (await this.tagRepository.GetTags(articleId: article.ArticleId)).Select(x => x.TagKey)
                       };
        }

        public IEnumerable<ArticleModel> Map(IEnumerable<Article> articles) => articles.Select(x => Map(x).Result);
    }
}
