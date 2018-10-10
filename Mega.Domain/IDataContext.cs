namespace Mega.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDataContext
    {
        Task<Article> GetArticle(int outerKey);

        Task<Tag> GetTag(string outerKey);

        Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null);

        Task<int> CountTags(int articleId = 0);

        IEnumerable<Tag> GetPopularTags(int countTags = 1);

        IDataContext CreateNewContext();

        void Migrate();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
