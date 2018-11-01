namespace Mega.Web.Api.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Модель тега
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed. Suppression is OK here.")]
    public class TagModel
    {
        /// <summary>
        /// ID тега
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// строковый ключ статьи с сайта
        /// </summary>
        public string TagKey { get; set; }

        /// <summary>
        /// имя тега
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// количество статей с данным тегом
        /// </summary>
        public int ArticlesCount { get; set; }
    }
}
