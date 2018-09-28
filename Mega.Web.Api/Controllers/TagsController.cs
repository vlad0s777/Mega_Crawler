namespace Mega.Web.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Data;
    using Mega.Domain;

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
            return new ActionResult<IEnumerable<Tag>>(this.context.GetTags(10, 10 * (numPage - 1)));
        }

        [HttpGet("tag/{id}")]
        public ActionResult<Tag> Get(int id)
        {
            return this.context.Tags.Find(id);
        }

        [HttpGet("tag/{id}/articles")]
        public ActionResult<IEnumerable<Article>> GetArticles(int id)
        {
            return new ActionResult<IEnumerable<Article>>(this.context.ArticlesTags.Where(x => x.TagId == id).Select(y => y.Article));
        }
    }
}