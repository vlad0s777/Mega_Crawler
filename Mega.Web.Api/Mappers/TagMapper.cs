namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Domain;

    public class TagMapper : IMapper<Tag, Models.Tag>
    {
        private readonly Domain.IDataContext context;

        public TagMapper(Domain.IDataContext context)
        {
            this.context = context;
        }

        public Models.Tag Map(Domain.Tag tag)
        {
            return new Models.Tag()
                       {
                           TagKey = tag.TagKey,
                           TagId = tag.TagId,
                           Articles = this.context.ArticlesTags.Where(x => x.TagId == tag.TagId).Select(y => y.Article.Head),
                           Name = tag.Name
            };
        }

        public IEnumerable<Models.Tag> Map(IEnumerable<Domain.Tag> tags)
        {
            return tags.Select(Map);
        }
    }
}
