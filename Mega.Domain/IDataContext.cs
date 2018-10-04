namespace Mega.Domain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IDataContext
    {
        Task<Article> GetArticle(int outerKey);

        Task<Tag> GetTag(string outerKey);

        Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null);

        Task<int> CountTags(int articleId = 0);

        Task<Tag> PopularTag();

        IDataContext CreateNewContext();

        void Migrate();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
