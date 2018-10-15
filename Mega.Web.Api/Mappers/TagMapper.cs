namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Domain;
    using Mega.Web.Api.Models;

    public class TagMapper : IMapper<Tag, TagModel>
    {
        private readonly IDataContext context;

        public TagMapper(IDataContext context)
        {
            this.context = context;
        }

        public TagModel Map(Tag tag)
        {
            return new TagModel()
                       {
                           TagKey = tag.TagKey,
                           TagId = tag.TagId,
                           Name = tag.Name,
                           CountArticles = this.context.CountArticles(tag.TagId).Result
            };
        }

        public IEnumerable<TagModel> Map(IEnumerable<Tag> tags) => tags.Select(Map);
    }
}
