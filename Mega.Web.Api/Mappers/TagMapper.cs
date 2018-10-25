namespace Mega.Web.Api.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Web.Api.Models;

    public class TagMapper : IMapper<Tag, TagModel>
    {
        private readonly ISomeReportDataProvider someReportDataProvider;

        public TagMapper(ISomeReportDataProvider someReportDataProvider)
        {
            this.someReportDataProvider = someReportDataProvider;
        }

        public async Task<TagModel> Map(Tag tag)
        {
            return new TagModel()
                       {
                           TagKey = tag.TagKey,
                           TagId = tag.TagId,
                           Name = tag.Name,
                           ArticlesCount = await this.someReportDataProvider.CountArticles(tag.TagId)
                       };
        }

        public IEnumerable<TagModel> Map(IEnumerable<Tag> tags) => tags.Select(x => Map(x).Result);
    }
}
