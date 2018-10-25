namespace Mega.Web.Api.Models
{
    /// <summary>
    /// Модель тега
    /// </summary>
    public class TagModel
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
        /// количество статей с данным тегом
        /// </value>
        public int ArticlesCount { get; set; }
    }
}
