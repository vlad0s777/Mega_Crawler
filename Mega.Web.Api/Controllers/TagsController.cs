namespace Mega.Web.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Data;
    using Mega.Domain;
    using Mega.Web.Api.Exceptions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly DataContext context;

        public TagsController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Tag>> Get()
        {
            return this.context.Tags;
        }

        [HttpGet("{numPage}")]
        public ActionResult<IEnumerable<Tag>> GetPage(int numPage)
        {
            var tags = this.context.GetTags(10, 10 * (numPage - 1));
            if (tags.Count() != 0)
            {
                return new ActionResult<IEnumerable<Tag>>(this.context.GetTags(10, 10 * (numPage - 1)));
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!");
        }

        [HttpGet("tag/{id}")]
        public ActionResult<Tag> Get(int id)
        {
            try
            {
                var tag = this.context.Tags.Find(id);
                var _ = tag.TagId;
                return tag;
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Tag not found!");
            }
        }

        [HttpGet("tag/{id}/articles")]
        public ActionResult<IEnumerable<Article>> GetArticles(int id)
        {
            return new ActionResult<IEnumerable<Article>>(this.context.ArticlesTags.Where(x => x.TagId == id).Select(y => y.Article));
        }
    }
}