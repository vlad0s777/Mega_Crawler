namespace Mega.Data
{
    using Mega.Domain;

    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleTag>()
                .HasKey(pc => new { pc.ArticleId, pc.TagId });

            modelBuilder.Entity<ArticleTag>()
                .HasOne(pc => pc.Article)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(pc => pc.ArticleId);

            modelBuilder.Entity<ArticleTag>()
                .HasOne(pc => pc.Tag)
                .WithMany(c => c.ArticleTags)
                .HasForeignKey(pc => pc.TagId);
        }
    }
}
