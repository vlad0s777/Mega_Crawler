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

        public ArticlesController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IQueryable<Models.Article> Get()
        {
            var articles = from b in this.context.Articles
                        select new Models.Article()
                                   {
                                       ArticleId = b.ArticleId,
                                       DateCreate = b.DateCreate,
                                       Head = b.Head,
                                       OuterArticleId = b.OuterArticleId,
                                       Text = b.Text,
                                       Tags = this.context.ArticlesTags.Where(x => x.ArticleId == b.ArticleId).Select(y => y.Tag.Name)
                        };

            return articles;
        }

        [HttpGet("{numPage}")]
        public IEnumerable<Models.Article> GetPage(int numPage)
        {
            var articles = from b in this.context.GetArticles(10, 10 * (numPage - 1))
                           select new Models.Article()
                                      {
                                          ArticleId = b.ArticleId,
                                          DateCreate = b.DateCreate,
                                          Head = b.Head,
                                          OuterArticleId = b.OuterArticleId,
                                          Text = b.Text,
                                          Tags = this.context.ArticlesTags.Where(x => x.ArticleId == b.ArticleId).Select(y => y.Tag.Name)
                                      };
            var enumerable = articles.ToList();
            if (enumerable.ToArray().Count() != 0)
            {
                return enumerable;
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
        public ActionResult<IEnumerable<Tag>> GetTags(int id)
        {
            return new ActionResult<IEnumerable<Tag>>(this.context.ArticlesTags.Where(x => x.ArticleId == id).Select(y => y.Tag));
        }
    }
}
