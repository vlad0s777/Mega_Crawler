namespace Mega.Data
{
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

        public IDataContext CreateNewContext()
        {
            return new DataContext(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>().HasKey(pc => new { pc.ArticleId, pc.TagId });
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Article).WithMany(p => p.ArticleTags).HasForeignKey(pc => pc.ArticleId);
            modelBuilder.Entity<ArticleTag>().HasOne(pc => pc.Tag).WithMany(c => c.ArticleTags);
        }
    }
}
