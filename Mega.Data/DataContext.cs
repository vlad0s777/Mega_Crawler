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
        private readonly string connectionString;

        public DataContext(string connectionString)
            : base(new DbContextOptionsBuilder<DataContext>().UseNpgsql(connectionString).Options)
        {
            this.connectionString = connectionString;
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<ArticleTag> ArticleTag { get; set; }

        public IEnumerable<Article> GetArticles(int limit = 0, int offset = 0)
        {
            return this.Articles.Skip(offset).Take(limit);
        }

        public async Task<int> CountArticles(int tagId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.MinValue;
            var end = endDate ?? DateTime.Now;
            return tagId == 0
                       ? await this.Articles.CountAsync(x => x.DateCreate >= start && x.DateCreate <= end)
                       : await this.ArticleTag.CountAsync(x => x.TagId == tagId && x.Article.DateCreate >= start && x.Article.DateCreate <= end);
        }

        public async Task<Article> GetArticle(int outerKey) => await this.Articles.FirstAsync(t => t.OuterArticleId == outerKey);

        public IEnumerable<Tag> GetTags(int limit = 0, int offset = 0)
        {
            return this.Tags.Skip(offset).Take(limit);
        }

        public async Task<Tag> GetTag(string outerKey) => await this.Tags.FirstAsync(t => t.TagKey == outerKey);
      
        public async Task<int> CountTags(int articleId = 0) => articleId == 0 ? await this.Tags.CountAsync() : await this.ArticleTag.CountAsync(t => t.ArticleId == articleId);

        public IEnumerable<Tag> GetPopularTags(int countTags = 1)
        {
            var counts = new Dictionary<int, int>();
            foreach (var tag in this.Tags)
            {
                counts.Add(tag.TagId, CountArticles(tag.TagId).Result);
            }

            var countsList = counts.ToList();
            countsList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            for (var i = 0; i < countTags; i++)
            {
                yield return this.Tags.Find(countsList[i].Key);
            }           
        }

        public void Migrate() => this.Database.Migrate();

        public new async Task AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken)) => await base.AddAsync(entity, cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasIndex(pc => pc.OuterArticleId).IsUnique();
            modelBuilder.Entity<ArticleTag>().HasKey(pc => new { pc.ArticleId, pc.TagId });
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Article).WithMany(p => p.ArticleTag).HasForeignKey(pc => pc.ArticleId);
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Tag).WithMany(c => c.ArticleTag).HasForeignKey(pc => pc.TagId);
        }
    }
}
