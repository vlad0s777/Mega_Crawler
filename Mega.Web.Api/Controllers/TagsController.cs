namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Web.Api.Exceptions;
    using Mega.Web.Api.Mappers;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IDataContext context;

        private readonly IMapper<Tag, Models.Tag> tagMapper;

        public TagsController(IDataContext context, IMapper<Tag, Models.Tag> tagMapper)
        {
            this.context = context;
            this.tagMapper = tagMapper;
        }

        [HttpGet("{numPage=1}")]
        public ActionResult<IEnumerable<Tag>> GetPage(int numPage)
        {
            var tags = this.context.GetTags(10, 10 * (numPage - 1));
            var enumerable = tags as Tag[] ?? tags.ToArray();
            if (enumerable.Count() != 0)
            {
                return this.tagMapper.Map(enumerable);
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!");
        }

        [HttpGet("tag/{id}")]
        public async Task<Tag> GetTag(int id)
        {
            try
            {
                return await this.context.GetTag(id);
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Tag not found!");
            }
        }

        [HttpGet("tag/{id}/articles")]
        public IEnumerable<Article> GetArticles(int id) => this.context.GetArticles(tagId: id);

        [HttpGet("tag/{id}/articles/count/{startDate:datetime?}/{endDate:datetime?}")]
        public async Task<int> CountArticles(int id, DateTime? startDate, DateTime? endDate) => await this.context.CountArticles(tagId: id, startDate: startDate, endDate: endDate);

        [HttpGet("popular/{count=1}")]
        public IEnumerable<Tag> GetPopularTags(int count) => this.context.GetPopularTags(count);
    }
}