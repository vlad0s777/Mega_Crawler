namespace Mega.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IDataContext : IDisposable
    {
        DbSet<Article> Articles { get; set; }

        DbSet<Tag> Tags { get; set; }

        IEnumerable<Article> GetArticles(int limit = 0, int offset = 0);

        IEnumerable<Tag> GetTags(int limit = 0, int offset = 0);

        DbSet<ArticlesTags> ArticlesTags { get; set; }

        IDataContext CreateNewContext();

        void Migrate();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));

        int SaveChanges();

        EntityEntry Add(object entity); 
    }
}
