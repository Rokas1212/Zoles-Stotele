using Microsoft.EntityFrameworkCore;

namespace Stotele.Server.Models.ApplicationDbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Naudotojas> Naudotojai { get; set; }
        public DbSet<Uzsakymas> Uzsakymai { get; set; }
        public DbSet<Apmokejimas> Apmokejimai { get; set; }
        public DbSet<Klientas> Klientai { get; set; }
        public DbSet<Adresas> Adresai { get; set; }
        public DbSet<Parduotuve> Parduotuve { get; set; }
        public DbSet<Taskai> Taskai { get; set; }
        public DbSet<Vadybininkas> Vadybininkai { get; set; }
        public DbSet<Kategorija> Kategorijos { get; set; }
        public DbSet<Preke> Prekes { get; set; }
        public DbSet<Atsiliepimas> Atsiliepimai { get; set; }
        public DbSet<MegstamaKategorija> MegstamosKategorijos { get; set; }
        public DbSet<Nuolaida> Nuolaidos { get; set; }
        public DbSet<PrekesPerziura> PrekesPerziuros { get; set; }
        public DbSet<PrekesKategorija> PrekesKategorijos { get; set; }
        public DbSet<PrekesParduotuve> PrekesParduotuve { get; set; }
        public DbSet<PrekesUzsakymas> PrekesUzsakymai { get; set; }
        public DbSet<UzblokuotaRekomendacija> UzblokuotosRekomendacijos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure PrekesUzsakymas -> Uzsakymas relationship
            modelBuilder.Entity<PrekesUzsakymas>()
                .HasOne(pu => pu.Uzsakymas)
                .WithMany(u => u.PrekesUzsakymai)
                .HasForeignKey(pu => pu.UzsakymasId);
        }
    }
}