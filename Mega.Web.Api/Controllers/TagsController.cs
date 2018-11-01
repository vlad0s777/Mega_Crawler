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

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контроллер тегов
    /// </summary>
    /// <remarks>
    /// В данном контроллере можно получить теги, статьи у определенных тегов, количество статей у определенного тега в опредеделенный промежуток времени,
    /// получить определенное количество популярных тегов
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ISomeReportDataProvider someReportDataProvider;

        private readonly IMapper<Article, ArticleModel> articleMapper;

        private readonly IMapper<Tag, TagModel> tagMapper;

        /// <param name="someReportDataProvider">Контекст данных</param>
        /// <param name="articleMapper">Конвертер домена статьи в модель статьи</param>
        /// <param name="tagMapper">Конвертер домена тега в модель тега</param>
        public TagsController(ISomeReportDataProvider someReportDataProvider, IMapper<Tag, TagModel> tagMapper, IMapper<Article, ArticleModel> articleMapper)
        {
            this.someReportDataProvider = someReportDataProvider;
            this.tagMapper = tagMapper;
            this.articleMapper = articleMapper;
        }

        /// <summary>
        /// Получение списка тегов
        /// </summary>
        /// <returns>
        /// Модели тегов
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если страница не найдена
        /// </exception>
        /// <param name="numPage">Номер страницы</param>
        [HttpGet("{numPage=1}")]
        public async Task<List<TagModel>> GetPage(int numPage)
        {
            var tags = this.tagMapper.Map(await this.someReportDataProvider.GetTags(10, 10 * (numPage - 1))).ToList();
            if (tags.Count() != 0)
            {
                return tags;
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!");
        }

        /// <summary>
        /// Получение одного тега по идентификатору
        /// </summary>
        /// <returns>
        /// Модель тега
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если тег не найден
        /// </exception>
        /// <param name="id">Идентификатор тега</param>
        [HttpGet("tag/{id}")]
        public async Task<TagModel> GetTag(int id)
        {
            try
            {
                return await this.tagMapper.Map(await this.someReportDataProvider.GetTag(id));
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, "Tag not found!");
            }
        }

        /// <summary>
        /// Получение списка статей у определенного тега
        /// </summary>
        /// <returns>
        /// Модели статей
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если страница не найдена
        /// </exception>
        /// <param name="numPage">Номер страницы</param>
        /// <param name="id">Идентификатор тега</param>
        [HttpGet("tag/{id}/articles/{numPage=1}")]
        public async Task<List<ArticleModel>> GetArticles(int id, int numPage)
        {
            var articles = this.articleMapper.Map(await this.someReportDataProvider.GetArticles(10, 10 * (numPage - 1), id)).ToList();
            if (articles.Count() != 0)
            {
                return articles;
            }

            throw new HttpResponseException(StatusCodes.Status404NotFound, "Page not found!");
        }

        /// <summary>
        /// Получение количества статей в определенный промежуток времени у определенного тега
        /// </summary>
        /// <returns>
        /// Количество статей
        /// </returns>
        /// <param name="startDate">Начальная дата, необязательная, если без неё, то будет подсчитано количество всех статей</param>
        /// <param name="endDate">Конечная дата, необяательная, если без неё, то будет подсчитано количество статей от начальной даты до последней статьи</param>
        /// <param name="id">Идентификатор тега</param>
        [HttpGet("tag/{id}/articles/count/{startDate:datetime?}/{endDate:datetime?}")]
        public async Task<int> CountArticles(int id, DateTime? startDate, DateTime? endDate) => await this.someReportDataProvider.CountArticles(tagId: id, startDate: startDate, endDate: endDate);

        /// <summary>
        /// Получение определенного количества самых популярных тегов
        /// </summary>
        /// <returns>
        /// Модели статей
        /// </returns>
        /// <param name="count">Количество возвращаемых самых популярных тегов</param>
        [HttpGet("popular/{count=1}")]
        public IEnumerable<TagModel> GetPopularTags(int count) => this.tagMapper.Map(this.context.GetPopularTags(count));

        /// <summary>
        /// Удаление одного тега
        /// </summary>
        /// <remarks>
        /// Добавляем тег в блеклист (таблица TagsDelete)
        /// </remarks>
        /// <returns>
        /// Результат  удаления
        /// </returns>
        /// <param name="id">Идентификатор тега</param>
        [HttpDelete("tag/{id}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task DeleteTag(int id)
        {
            object entity;
            try
            {
                entity = new RemovedTag { DeletionDate = DateTime.Now, Tag = await this.context.GetTag(id) };
            }
            catch
            {
                throw new HttpResponseException(404, $"Tag {id} not found");
            }

            await this.context.AddAsync(entity);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление списка тегов
        /// </summary>
        /// <remarks>
        /// Добавляем теги в блеклист (таблица TagsDelete)
        /// </remarks>
        /// <returns>
        /// Результат  удаления
        /// </returns>
        /// <param name="ids">Список идентификаторов тегов</param>
        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete]
        public async Task DeleteTags(List<int> ids)
        {
            foreach (var id in ids)
            {
                await DeleteTag(id);
            }
        }
    }
}