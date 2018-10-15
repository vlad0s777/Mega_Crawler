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
        [HttpGet("{numPage=1}")]
        public ActionResult<IEnumerable<Article>> GetPage(int numPage)
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
        public async Task<Article> GetArticle(int id)
        {
            try
            {
                return await this.context.GetArticle(id);
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Article not found!");
            }
        }

        [HttpGet("article/{id}/tags")]
        public IEnumerable<Tag> GetTags(int id) => this.context.GetTags(articleId: id);

        [HttpGet("count/{startDate:datetime?}/{endDate:datetime?}")]
        public async Task<int> CountArticles(DateTime? startDate, DateTime? endDate)
        {
            return await this.context.CountArticles(startDate: startDate, endDate: endDate);
        }
    }
}
