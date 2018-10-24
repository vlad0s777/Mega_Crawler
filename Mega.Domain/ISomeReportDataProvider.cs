namespace Mega.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISomeReportDataProvider
    {
        Task<List<Article>> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0);

        Task<Article> GetArticle(int id, bool outer = false);

        Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null);

        Task<List<Tag>> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0);

        Task<List<TagDelete>> GetDeleteTags(int limit = int.MaxValue, int offset = 0);

        Task<Tag> GetTag(string outerKey);

        Task<Tag> GetTag(int id);

        Task<int> CountTags(int articleId = 0);

        Task<List<Tag>> GetPopularTags(int countTags = 1);

        void Migrate();

        Task<object> AddAsync(object entity);
    }
}
