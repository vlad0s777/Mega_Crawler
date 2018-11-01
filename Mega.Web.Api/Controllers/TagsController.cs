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
        private readonly IDataContext context;

        private readonly IMapper<Article, ArticleModel> articleMapper;

        private readonly IMapper<Tag, TagModel> tagMapper;

        /// <param name="context">Контекст данных</param>
        /// <param name="articleMapper">Конвертер домена статьи в модель статьи</param>
        /// <param name="tagMapper">Конвертер домена тега в модель тега</param>
        public TagsController(IDataContext context, IMapper<Tag, TagModel> tagMapper, IMapper<Article, ArticleModel> articleMapper)
        {
            this.context = context;
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
        public IEnumerable<TagModel> GetPage(int numPage)
        {
            var tags = this.tagMapper.Map(this.context.GetTags());
            var tagModels = tags as TagModel[] ?? tags.ToArray();
            if (tagModels.Count() != 0)
            {
                return tagModels;
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
                return this.tagMapper.Map(await this.context.GetTag(id));
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
        public IEnumerable<ArticleModel> GetArticles(int id, int numPage)
        {
            var articles = this.articleMapper.Map(this.context.GetArticles(10, 10 * (numPage - 1), id));
            var articleModels = articles as ArticleModel[] ?? articles.ToArray();
            if (articleModels.Count() != 0)
            {
                return articleModels;
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
        public async Task<int> CountArticles(int id, DateTime? startDate, DateTime? endDate) => await this.context.CountArticles(tagId: id, startDate: startDate, endDate: endDate);

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