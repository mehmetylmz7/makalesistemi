using makalesistemi.Models;
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
                optionsBuilder.UseSqlServer("server=DIDIM\\SQLEXPRESS; database=makalesistemi; integrated security=true;TrustServerCertificate=True;");
            }
        }

        public DbSet<Yazar> Yazarlar { get; set; }
        public DbSet<Makale> Makaleler { get; set; }
        public DbSet<Editor> Editorler { get; set; }
        public DbSet<Hakem> Hakemler { get; set; }
        public DbSet<Log> Loglar { get; set; }
        public DbSet<Anonimlestirme> Anonimlestirmeler { get; set; }
        
        public DbSet<Sohbet> Sohbetler { get; set; }    

        //public DbSet<Mesaj> Mesajlar { get; set; }

        // public DbSet<Degerlendirme> Degerlendirmeler { get; set; }   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Birleşik anahtarları belirtin (Eğer Gerekirse)
          //  modelBuilder.Entity<Degerlendirme>()
             //   .HasKey(d => new { d.Id, d.MakaleId, d.HakemId });

            // Makale - Yazar ilişkisi (1-N)
            modelBuilder.Entity<Makale>()
                .HasOne(m => m.Yazar)
                .WithMany(y => y.Makaleler)
                .HasForeignKey(m => m.YazarId)
                .OnDelete(DeleteBehavior.Restrict);

            // Makale - Hakem ilişkisi (1-1 opsiyonel)
            modelBuilder.Entity<Makale>()
                .HasOne(m => m.Hakem)
                .WithMany(h => h.Makaleler)
                .HasForeignKey(m => m.HakemId)
                .OnDelete(DeleteBehavior.SetNull);

            // Makale - Log ilişkisi (1-N)
            modelBuilder.Entity<Log>()
                .HasOne(l => l.Makale)
                .WithMany(m => m.Loglar)
                .HasForeignKey(l => l.MakaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Makale - Anonimleştirme ilişkisi (1-N)
            modelBuilder.Entity<Anonimlestirme>()
                .HasOne(a => a.Makale)
                .WithMany(m => m.Anonimlestirmeler)
                .HasForeignKey(a => a.MakaleId)
                .OnDelete(DeleteBehavior.Cascade);
            /*
                        // Makale - Değerlendirme ilişkisi (1-N)
                        modelBuilder.Entity<Degerlendirme>()
                            .HasOne(d => d.Makale)
                            .WithMany(m => m.Degerlendirmeler)
                            .HasForeignKey(d => d.MakaleId)
                            .OnDelete(DeleteBehavior.Cascade); */

            // Hakem - Değerlendirme ilişkisi (1-N)
            // modelBuilder.Entity<Degerlendirme>()
            //   .HasOne(d => d.Hakem)
            //  .WithMany(h => h.Degerlendirmeler)
            // .HasForeignKey(d => d.HakemId)
            //   .OnDelete(DeleteBehavior.Cascade);
            // Sohbet - Yazar ilişkisi (1-N)

            modelBuilder.Entity<Sohbet>()
                .HasOne(s => s.Gonderici) // Sohbet'in bir Göndericisi (Yazar) var
                .WithMany(y => y.Sohbetler) // Yazar'ın birden fazla Sohbet'i olabilir
                .HasForeignKey(s => s.GondericiId) // Sohbet'in GondericiId'si Yazar'ın Id'si ile ilişkili
                .OnDelete(DeleteBehavior.Restrict); // Yazar silinirse, Sohbet'ler silinmesin

            // Sohbet - Makale ilişkisi (1-N)
            modelBuilder.Entity<Sohbet>()
                .HasOne(s => s.Makale) // Sohbet'in bir Makalesi var
                .WithMany(m => m.Sohbetler) // Makale'nin birden fazla Sohbet'i olabilir
                .HasForeignKey(s => s.MakaleId) // Sohbet'in MakaleId'si Makale'nin Id'si ile ilişkili
                .OnDelete(DeleteBehavior.Cascade); // Makale silinirse, ilişkili Sohbet'ler de silinsin
        }
    }
}
