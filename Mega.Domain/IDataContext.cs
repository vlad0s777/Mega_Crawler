namespace Mega.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDataContext
    {
        IEnumerable<Article> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0);

        Task<Article> GetArticle(int id, bool outer = false);

        Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null);

        IEnumerable<Tag> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0);

        IEnumerable<TagDelete> GetDeleteTags(int limit = int.MaxValue, int offset = 0);

        Task<Tag> GetTag(string outerKey);

        Task<Tag> GetTag(int id);        

        int CountTags(int articleId = 0);

        IEnumerable<Tag> GetPopularTags(int countTags = 1);

        void Migrate();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}