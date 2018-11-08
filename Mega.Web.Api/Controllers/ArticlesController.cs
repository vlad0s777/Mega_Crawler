namespace Mega.Web.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;
    using Mega.Domain.Repositories;
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
        private readonly IArticleRepository articleRepository;

        private readonly IMapper<Articles, ArticleModel> articleMapper;

        /// <param name="articleRepository">Репозиторий статьи</param>
        /// <param name="articleMapper">Конвертер домена статьи в модель статьи</param>
        public ArticlesController(IMapper<Articles, ArticleModel> articleMapper, IArticleRepository articleRepository)
        {
            this.articleMapper = articleMapper;
            this.articleRepository = articleRepository;
        }

        /// <summary>
        /// Получение списка статей
        /// </summary>
        /// <returns>
        /// Модели статей
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если страница не найдена
        /// </exception>
        /// <param name="page">Номер страницы</param>
        [HttpGet]
        public async Task<List<ArticleModel>> GetPage(int page = 1)
        {
            var articles = this.articleMapper.Map(await this.articleRepository.GetArticles(10, 10 * (page - 1))).ToList();
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
        [HttpGet("{id}")]
        public async Task<ArticleModel> GetArticle(int id)
        {
            try
            {
                return await this.articleMapper.Map(await this.articleRepository.Get(id));
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
        [HttpGet("count")]
        public async Task<int> CountArticles(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await this.articleRepository.CountArticles(startDate: startDate, endDate: endDate);
        }
    }
}
