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
    public class ArticlesController : ControllerBase
    {
        private readonly IDataContext context;

        private readonly IMapper <Article, Models.Article> articleMapper;

        public ArticlesController(IDataContext context, IMapper<Article, Models.Article> articleMapper)
        {
            this.context = context;
            this.articleMapper = articleMapper;
        }

        [HttpGet]
        public IEnumerable<Models.Article> Get()
        {
            return this.articleMapper.Map(this.context.Articles);
        }

        [HttpGet("{numPage}")]
        public IEnumerable<Models.Article> GetPage(int numPage)
        {
            var articles = this.articleMapper.Map(this.context.GetArticles(10, 10 * (numPage - 1)));

            var enumerable = articles as Models.Article[] ?? articles.ToArray();
            if (enumerable.Count() != 0)
            {
                return enumerable;
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!"); 
        }

        [HttpGet("article/{id}")]
        public ActionResult<Models.Article> Get(int id)
        {
            try
            {
                var article = this.context.Articles.Find(id);
                var _ = article.ArticleId;
                return this.articleMapper.Map(article);
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Article not found!");
            }
        }
    }
}
