namespace Mega.Data
{
    using System.Collections.Generic;
    using System.Linq;

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

        public IEnumerable<Article> GetArticles(int limit = 0, int offset = 0)
        {
            return this.Articles.Skip(offset).Take(limit);
        }

        public DbSet<Tag> Tags { get; set; }

        public IEnumerable<Tag> GetTags(int limit = 0, int offset = 0)
        {
            return this.Tags.Skip(offset).Take(limit);
        }

        public DbSet<ArticlesTags> ArticlesTags { get; set; }

        public IDataContext CreateNewContext()
        {
            return new DataContext(this.connectionString);
        }

        public void Migrate()
        {
            this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticlesTags>().HasKey(pc => new { pc.ArticleId, pc.TagId });
            modelBuilder.Entity<ArticlesTags>().HasOne(pc => pc.Article).WithMany(p => p.ArticlesTags).HasForeignKey(pc => pc.ArticleId);
            modelBuilder.Entity<ArticlesTags>().HasOne(pc => pc.Tag).WithMany(c => c.ArticlesTags).HasForeignKey(pc => pc.TagId);
        }
    }
}
