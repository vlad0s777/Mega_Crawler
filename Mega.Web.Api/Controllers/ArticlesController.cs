namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Web.Api.Exceptions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IDataContext context;

        public ArticlesController(IDataContext context) => this.context = context;

        [HttpGet("{numPage=1}")]
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
