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
        private readonly ITagRepository tagRepository;

        private readonly IArticleRepository articleRepository;

        private readonly IRepository<RemovedTag> removedTagRepository;

        private readonly IMapper<Article, ArticleModel> articleMapper;

        private readonly IMapper<Tag, TagModel> tagMapper;

        /// <param name="tagRepository">Репозиторий тегов</param>
        /// <param name="removedTagRepository">Репозиторий удаленных тегов</param>
        /// <param name="articleRepository">Репозиторий статей</param>
        /// <param name="articleMapper">Конвертер домена статьи в модель статьи</param>
        /// <param name="tagMapper">Конвертер домена тега в модель тега</param>
        public TagsController(IMapper<Tag, TagModel> tagMapper, IMapper<Article, ArticleModel> articleMapper, ITagRepository tagRepository, IRepository<RemovedTag> removedTagRepository, IArticleRepository articleRepository)
        {
            this.tagMapper = tagMapper;
            this.articleMapper = articleMapper;
            this.tagRepository = tagRepository;
            this.removedTagRepository = removedTagRepository;
            this.articleRepository = articleRepository;
        }

        /// <summary>
        /// Получение списка тегов
        /// </summary>
        /// <returns>
        /// Модели тегов
        /// </returns>
        /// <exception cref="HttpResponseException">Возникает если страница не найдена
        /// </exception>
        /// <param name="page">Номер страницы</param>
        [HttpGet]
        public async Task<List<TagModel>> GetPage(int page = 1)
        {
            var tags = this.tagMapper.Map(await this.tagRepository.GetTags(10, 10 * (page - 1))).ToList();
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
        [HttpGet("{id}")]
        public async Task<TagModel> GetTag(int id)
        {
            try
            {
                return await this.tagMapper.Map(await this.tagRepository.Get(id));
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
        /// <param name="page">Номер страницы</param>
        /// <param name="id">Идентификатор тега</param>
        [HttpGet("{id}/articles")]
        public async Task<List<ArticleModel>> GetArticles(int id, int page = 1)
        {
            var articles = this.articleMapper.Map(await this.articleRepository.GetArticles(10, 10 * (page - 1), id)).ToList();
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
        [HttpGet("tag/{id}/articles/count")]
        public async Task<int> CountArticles(int id, DateTime? startDate, DateTime? endDate) => await this.articleRepository.CountArticles(tagId: id, startDate: startDate, endDate: endDate);

        /// <summary>
        /// Получение определенного количества самых популярных тегов
        /// </summary>
        /// <returns>
        /// Модели статей
        /// </returns>
        /// <param name="count">Количество возвращаемых самых популярных тегов</param>
        [HttpGet("popular")]
        public async Task<List<TagModel>> GetPopularTags(int count = 1) => this.tagMapper.Map(await this.tagRepository.GetPopularTags(count)).ToList();

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
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task DeleteTag(int id)
        {
            try
            {
                var entity = new RemovedTag { DeletionDate = DateTime.Now, TagId = id };
                await this.removedTagRepository.Create(entity);
            }
            catch
            {
                throw new HttpResponseException(StatusCodes.Status404NotFound, $"Tag {id} not found");
            }
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