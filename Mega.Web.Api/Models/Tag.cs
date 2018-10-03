﻿namespace Mega.Web.Api.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Модель тега
    /// </summary>
    public class Tag
    {
        /// <value>
        /// ID тега
        /// </value>
        public int TagId { get; set; }

        /// <value>
        /// строковый ключ статьи с сайта
        /// </value>
        public string TagKey { get; set; }

        /// <value>
        /// имя тега
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// статьи с данным тегом
        /// </value>
        public IEnumerable<string> Articles { get; set; }

        /// <value>
        /// количество статей с данным тегом
        /// </value>
        public int CountArticles => this.Articles.Count();
    }
}
