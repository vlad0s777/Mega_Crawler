namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
    using Mega.Web.Api.Models;

    public class TagMapper : IMapper<Tags, TagModel>
    {
        private readonly IArticleRepository articleRepository;

        public TagMapper(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public async Task<TagModel> Map(Tags tag)
        {
            return new TagModel()
                       {
                           TagKey = tag.Tag_Key,
                           TagId = tag.Tag_Id,
                           Name = tag.Name,
                           ArticlesCount = await this.articleRepository.CountArticles(tag.Tag_Id)
                       };
        }

        public IEnumerable<TagModel> Map(IEnumerable<Tags> tags) => tags.Select(x => Map(x).Result);
    }
}
