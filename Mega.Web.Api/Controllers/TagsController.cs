namespace Mega.Web.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

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

        [HttpGet]
        public IEnumerable<Models.Tag> Get()
        {
            return this.tagMapper.Map(this.context.Tags);
        }

        [HttpGet("{numPage}")]
        public IEnumerable<Models.Tag> GetPage(int numPage)
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
        public ActionResult<Models.Tag> Get(int id)
        {
            try
            {
                var tag = this.context.Tags.Find(id);
                var _ = tag.TagId;
                return this.tagMapper.Map(tag);
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Tag not found!");
            }
        }
    }
}