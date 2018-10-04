namespace Mega.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Mega.Domain;

    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext, IDataContext
    {
        private readonly string connectionString;

        public DataContext(string connectionString)
            : base(new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options)
        {
            this.connectionString = connectionString;
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ArticleTag> ArticleTag { get; set; }

        public async Task<Article> GetArticle(int outerKey) => await this.Articles.FirstAsync(t => t.OuterArticleId == outerKey);

        public async Task<Tag> GetTag(string outerKey) => await this.Tags.FirstAsync(t => t.TagKey == outerKey);

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;
            return tagId == 0
                       ? await this.Articles.CountAsync(x => x.DateCreate >= start && x.DateCreate <= end)
                       : await this.ArticleTag.CountAsync(x => x.TagId == tagId && x.Article.DateCreate >= start && x.Article.DateCreate <= end);
        }

        public async Task<int> CountTags(int articleId = 0)
        {
            return articleId == 0 ? await this.Tags.CountAsync() : await this.ArticleTag.CountAsync(t => t.ArticleId == articleId);
        }

        public async Task<Tag> PopularTag()
        {
            var counts = new Dictionary<int, int>();
            foreach (var tag in this.Tags)
            {
                counts.Add(tag.TagId, await CountArticles(tag.TagId));
            }

            return this.Tags.Find(counts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key);
        }

        public IDataContext CreateNewContext() => new DataContext(this.connectionString);

        public void Migrate() => this.Database.Migrate();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasIndex(pc => pc.OuterArticleId).IsUnique();
            modelBuilder.Entity<ArticleTag>().HasKey(pc => new { pc.ArticleId, pc.TagId });
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Article).WithMany(p => p.ArticleTags).HasForeignKey(pc => pc.ArticleId);
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Tag).WithMany(c => c.ArticleTags).HasForeignKey(pc => pc.TagId);
        }
    }
}
