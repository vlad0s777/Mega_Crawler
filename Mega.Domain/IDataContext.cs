namespace Mega.Domain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public interface IDataContext : IDisposable
    {
        DbSet<Article> Articles { get; set; }

        DbSet<Tag> Tags { get; set; }

        DbSet<ArticlesTags> ArticlesTags { get; set; }

        IDataContext CreateNewContext();

        void Migrate();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));

        int SaveChanges();

        EntityEntry Add(object entity); 
    }
}
