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
    public class ArticlesController : ControllerBase
    {
        private readonly DataContext context;

        public ArticlesController(DataContext context) => this.context = context;

        [HttpGet]
        public ActionResult<IEnumerable<Article>> Get() => this.context.Articles;

        [HttpGet("{numPage}")]
        public ActionResult<IEnumerable<Article>> GetPage(int numPage)
        {
            var articles = this.context.GetArticles(10, 10 * (numPage - 1));
            if (articles.Count() != 0)
            {
                return new ActionResult<IEnumerable<Article>>(this.context.GetArticles(10, 10 * (numPage - 1)));
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!"); 
        }

        [HttpGet("article/{id}")]
        public ActionResult<Article> Get(int id)
        {
            try
            {
                var article = this.context.Articles.Find(id);
                var _ = article.ArticleId;
                return article;
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Article not found!");
            }
        }

        [HttpGet("article/{id}/tags")]
        public ActionResult<IEnumerable<Tag>> GetTags(int id) => new ActionResult<IEnumerable<Tag>>(this.context.ArticleTag.Where(x => x.ArticleId == id).Select(y => y.Tag));
    }
}
