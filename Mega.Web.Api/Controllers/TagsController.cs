namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public TagsController(DataContext context) => this.context = context;

        [HttpGet("{numPage=1}")]
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
        public IEnumerable<Article> GetArticles(int id) => this.context.ArticleTag.Where(x => x.TagId == id).Select(y => y.Article);

        [HttpGet("tag/{id}/articles/count/{startDate:datetime?}/{endDate:datetime?}")]
        public async Task<int> CountArticles(int id, DateTime? startDate, DateTime? endDate) => await this.context.CountArticles(tagId: id, startDate: startDate, endDate: endDate);

        [HttpGet("popular/{count=1}")]
        public IEnumerable<Tag> GetPopularTags(int count) => this.context.GetPopularTags(count);
    }
}