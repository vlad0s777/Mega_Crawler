﻿namespace Mega.Web.Api.Controllers
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
    /// Контроллер тегов
    /// </summary>
    /// <remarks>
    /// В данном контроллере можно получить тэги, статьи у определенных тегов, количество статей у определенного тега в опредеделенный промежуток времени,
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
            var tags = this.tagMapper.Map(this.context.GetTags(10, 10 * (numPage - 1)));
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
    }
}