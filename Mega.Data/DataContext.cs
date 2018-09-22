namespace Mega.Data
{
    using Mega.Domain;

    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext, IDataContext
    {
//        public DataContext()
//        {          
//        }
//
//        public DataContext(DbContextOptions options)
//            : base(options)
//        {
//        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=11111");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

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

           // base.OnModelCreating(modelBuilder);
        }
    }
}
