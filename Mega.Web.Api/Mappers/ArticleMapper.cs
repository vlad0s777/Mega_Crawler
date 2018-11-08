namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
    using Mega.Web.Api.Models;

    public class ArticleMapper : IMapper<Articles, ArticleModel>
    {
        private readonly ITagRepository tagRepository;

        public ArticleMapper(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<ArticleModel> Map(Articles article)
        {
            return new ArticleModel()
                       {
                           ArticleId = article.Article_Id,
                           DateCreate = article.Date_Create,
                           Head = article.Head,
                           OuterArticleId = article.Outer_Article_Id,
                           Text = article.Text,
                           Tags = (await this.tagRepository.GetTags(articleId: article.Article_Id)).Select(x => x.Tag_Key)
                       };
        }

        public IEnumerable<ArticleModel> Map(IEnumerable<Articles> articles) => articles.Select(x => Map(x).Result);
    }
}
