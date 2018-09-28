namespace Mega.Web.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Mega.Data;
    using Mega.Domain;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly DataContext context;

        public ArticlesController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Article>> Get()
        {
            return this.context.Articles;
        }

        [HttpGet("{numPage}")]
        public ActionResult<IEnumerable<Article>> GetPage(int numPage)
        {
            return new ActionResult<IEnumerable<Article>>(this.context.GetArticles(10, 10 * (numPage - 1)));
        }

        [HttpGet("article/{id}")]
        public ActionResult<Article> Get(int id)
        {
            return this.context.Articles.Find(id);
        }

        [HttpGet("article/{id}/tags")]
        public ActionResult<IEnumerable<Tag>> GetTags(int id)
        {
            return new ActionResult<IEnumerable<Tag>>(this.context.ArticlesTags.Where(x => x.ArticleId == id).Select(y => y.Tag));
        }

        /*        // POST api/values
                [HttpPost]
                public void Post([FromBody] string value)
                {
                }

                // PUT api/values/5
                [HttpPut("{id}")]
                public void Put(int id, [FromBody] string value)
                {
                }

                // DELETE api/values/5
                [HttpDelete("{id}")]
                public void Delete(int id)
                {
                }*/
    }
}
