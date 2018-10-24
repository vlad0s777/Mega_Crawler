namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Web.Api.Exceptions;
    using Mega.Web.Api.Mappers;
    using Mega.Web.Api.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контроллер статей
    /// </summary>
    /// <remarks>
    /// В данном контроллере можно просмотреть скрауленные статьи, получить количество статей в опредеделенный промежуток времени
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ISomeReportDataProvider someReportDataProvider;

        private readonly IMapper<Article, ArticleModel> articleMapper;

        /// <param name="someReportDataProvider">Контекст данных</param>
        /// <param name="articleMapper">Конвертер домена статьи в модель статьи</param>
        public ArticlesController(ISomeReportDataProvider someReportDataProvider, IMapper<Article, ArticleModel> articleMapper)
        {
            this.someReportDataProvider = someReportDataProvider;
            this.articleMapper = articleMapper;
        }

        /// <summary>
        /// Получение списка статей
        /// </summary>
        /// <returns>
        /// Модели статей
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если страница не найдена
        /// </exception>
        /// <param name="numPage">Номер страницы</param>
        [HttpGet("{numPage=1}")]
        public async Task<List<ArticleModel>> GetPage(int numPage)
        {
            var articles = this.articleMapper.Map(await this.someReportDataProvider.GetArticles(10, 10 * (numPage - 1))).ToList();
            if (articles.Count() != 0)
            {
                return articles;
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!");
        }

        /// <summary>
        /// Получение одной статьи по идентификатору
        /// </summary>
        /// <returns>
        /// Модель статьи
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если статья не найдена
        /// </exception>
        /// <param name="id">Идентификатор статьи</param>
        [HttpGet("article/{id}")]
        public async Task<ArticleModel> GetArticle(int id)
        {
            try
            {
                return await this.articleMapper.Map(await this.someReportDataProvider.GetArticle(id));
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Article not found!");
            }
        }

        /// <summary>
        /// Получение количества статей в определенный промежуток времени
        /// </summary>
        /// <returns>
        /// Количество статей
        /// </returns>
        /// <param name="startDate">Начальная дата, необязательная, если без неё, то будет подсчитано количество всех статей</param>
        /// <param name="endDate">Конечная дата, необяательная, если без неё, то будет подсчитано количество статей от начальной даты до последней статьи</param>
        [HttpGet("count/{startDate:datetime?}/{endDate:datetime?}")]
        public async Task<int> CountArticles(DateTime? startDate, DateTime? endDate)
        {
            return await this.someReportDataProvider.CountArticles(startDate: startDate, endDate: endDate);
        }
    }
}
