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

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
