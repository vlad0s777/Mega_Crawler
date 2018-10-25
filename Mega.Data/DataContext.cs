namespace Mega.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Mega.Domain;

    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext, IDataContext
    {
        public DataContext(string connectionString)
            : base(new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<RemovedTag> RemovedTags { get; set; }

        public DbSet<ArticleTag> ArticleTag { get; set; }

        public IEnumerable<Article> GetArticles(int limit = int.MaxValue, int offset = 0, int tagId = 0)
        {
            var articles = tagId != 0 ? this.ArticleTag.Where(x => x.TagId == tagId).Select(y => y.Article) : this.Articles;
            return articles.Skip(offset).Take(limit);
        }

        public async Task<Article> GetArticle(int id, bool outer = false)
        {
            if (outer)
            {
                return await this.Articles.FirstAsync(t => t.OuterArticleId == id);
            }
            else
            {
                return await this.Articles.FirstAsync(t => t.ArticleId == id);
            }
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;
            return tagId == 0
                       ? await this.Articles.CountAsync(x => x.DateCreate >= start && x.DateCreate <= end)
                       : await this.ArticleTag.CountAsync(x => x.TagId == tagId && x.Article.DateCreate >= start && x.Article.DateCreate <= end);
        }

        public IEnumerable<Tag> GetTags(int limit = int.MaxValue, int offset = 0, int articleId = 0)
        {
            var tags = articleId == 0
                           ? from u in this.Tags
                             join p in GetRemovedTags() on u equals p.Tag into gj
                             from x in gj.DefaultIfEmpty()
                             where x == null
                             select u
                           : from u in this.ArticleTag.Where(x => x.ArticleId == articleId)
                             join p in GetRemovedTags() on u.Tag equals p.Tag into gj
                             from x in gj.DefaultIfEmpty()
                             where x == null
                             select u.Tag;
            return tags.Skip(offset).Take(limit);
        }

        public IEnumerable<RemovedTag> GetRemovedTags(int limit = int.MaxValue, int offset = 0) => this.RemovedTags.Skip(offset).Take(limit);

        public async Task<Tag> GetTag(int id) => await this.Tags.FirstAsync(t => t.TagId == id);       

        public async Task<Tag> GetTag(string outerKey) => await this.Tags.FirstAsync(t => t.TagKey == outerKey);

        public int CountTags(int articleId = 0) => GetTags(articleId).Count();

        public IEnumerable<Tag> GetPopularTags(int countTags = 1) =>
            (from artags in this.ArticleTag
             group artags by artags.TagId into grouped
             join tags in this.Tags on grouped.Key equals tags.TagId
             join removedtags in this.RemovedTags on grouped.Key equals removedtags.TagId into gj
             from x in gj.DefaultIfEmpty()
             where x == null
             orderby grouped.Count() descending
             select tags).Take(countTags);

        public void Migrate() => this.Database.Migrate();

        public new async Task AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken)) => await base.AddAsync(entity, cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasIndex(pc => pc.OuterArticleId).IsUnique();

            modelBuilder.Entity<ArticleTag>().HasKey(pc => new { pc.ArticleId, pc.TagId });
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Article).WithMany(p => p.ArticleTag).HasForeignKey(pc => pc.ArticleId);
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Tag).WithMany(c => c.ArticleTag).HasForeignKey(pc => pc.TagId);

            modelBuilder.Entity<RemovedTag>().HasOne(x => x.Tag).WithOne(x => x.RemovedTag).HasForeignKey<RemovedTag>(x => x.TagId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
