namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
    using Mega.Web.Api.Models;

    public class TagMapper : IMapper<Tag, TagModel>
    {
        private readonly IArticleRepository articleRepository;

        public TagMapper(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public async Task<TagModel> Map(Tag tag)
        {
            return new TagModel()
                       {
                           TagKey = tag.TagKey,
                           TagId = tag.TagId,
                           Name = tag.Name,
                           ArticlesCount = await this.articleRepository.CountArticles(tag.TagId)
                       };
        }

        public IEnumerable<TagModel> Map(IEnumerable<Tag> tags) => tags.Select(x => Map(x).Result);
    }
}
