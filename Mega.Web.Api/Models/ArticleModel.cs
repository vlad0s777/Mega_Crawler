namespace Mega.Web.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Модель статьи
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed. Suppression is OK here.")]
    public class ArticleModel
    {
        /// <summary>
        /// ID статьи
        /// </summary>        
        public int ArticleId { get; set; }

        /// <summary>
        /// Дата создания статьи
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Текст статьи
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Заголовок статьи
        /// </summary>
        public string Head { get; set; }

        /// <summary>
        /// Идентификатор статьи с сайта
        /// </summary>
        public int OuterArticleId { get; set; }

        /// <summary>
        /// Теги статьи
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Идентификатор статьи с сайта
        /// </summary>
        public int TagsCount => this.Tags.Count();
    }
}