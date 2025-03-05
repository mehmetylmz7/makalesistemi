using Microsoft.EntityFrameworkCore;

namespace makalesistemi.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=DIDIM\\SQLEXPRESS; database=makalesistemi; integrated security=true;TrustServerCertificate = True;");
            }
        }
        // DbSet'ler (Tablolar)
        public DbSet<Author> Authors { get; set; }
        public DbSet<Editor> Editors { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Anonymization> Anonymizations { get; set; }
        public DbSet<Log> Logs { get; set; }

        // Fluent API konfigürasyonları (isteğe bağlı, ilişkileri özelleştirebilirsiniz)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Editor sadece bir tane olacağı için, bunu veritabanında tekil yapmak için:
            modelBuilder.Entity<Editor>()
                .HasData(new Editor { Id = 1, Email = "editor@domain.com", CreatedAt = DateTime.UtcNow });

            // İlişkiler ve diğer konfigürasyonlar
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author)
                .WithMany(a => a.Articles)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Reviewer)
                .WithMany(r => r.Reviews)
                .HasForeignKey(a => a.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Article)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Anonymization>()
                .HasOne(a => a.Article)
                .WithMany()
                .HasForeignKey(a => a.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.Author)
                .WithMany()
                .HasForeignKey(l => l.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
