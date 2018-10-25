namespace Mega.Web.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Модель статьи
    /// </summary>
    public class ArticleModel
    {
        /// <value>
        /// ID статьи
        /// </value>
        public int ArticleId { get; set; }

        /// <value>
        /// Дата создания статьи
        /// </value>
        public DateTime DateCreate { get; set; }

        /// <value>
        /// Текст статьи
        /// </value>
        public string Text { get; set; }

        /// <value>
        /// Заголовок статьи
        /// </value>
        public string Head { get; set; }

        /// <value>
        /// Идентификатор статьи с сайта
        /// </value>
        public int OuterArticleId { get; set; }

        /// <value>
        /// Теги статьи
        /// </value>
        public IEnumerable<string> Tags { get; set; }

        /// <value>
        /// Идентификатор статьи с сайта
        /// </value>
        public int TagsCount => this.Tags.Count();
    }
}
