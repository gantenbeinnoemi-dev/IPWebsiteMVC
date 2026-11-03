using IPWebsiteMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Data;

public class MyBooksDbContext(DbContextOptions<MyBooksDbContext> options) : DbContext(options)
{
    public DbSet<Buch> Buecher => Set<Buch>();
    public DbSet<Autor> Autoren => Set<Autor>();
    public DbSet<Land> Laender => Set<Land>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Altersklassifikation> Altersklassifikationen => Set<Altersklassifikation>();
    public DbSet<Benutzer> Benutzer => Set<Benutzer>();
    public DbSet<Kategorie> Kategorien => Set<Kategorie>();
    public DbSet<KategorieBuch> KategorieBuecher => Set<KategorieBuch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buch>().ToTable("Buch");
        modelBuilder.Entity<Autor>().ToTable("Autor");
        modelBuilder.Entity<Land>().ToTable("Land");
        modelBuilder.Entity<Genre>().ToTable("Genre");
        modelBuilder.Entity<Altersklassifikation>().ToTable("Altersklassifikation");
        modelBuilder.Entity<Benutzer>().ToTable("Benutzer");

        modelBuilder.Entity<Kategorie>(e =>
        {
            e.ToTable("Kategorie");
            e.HasOne<Benutzer>().WithMany().HasForeignKey(k => k.BenutzerID);
        });

        modelBuilder.Entity<KategorieBuch>(e =>
        {
            e.ToTable("Kategorie_Buch");
            e.HasKey(x => new { x.KategorieID, x.BuchID });
            e.HasOne(x => x.Kategorie).WithMany(k => k.Eintraege).HasForeignKey(x => x.KategorieID);
            e.HasOne(x => x.Buch).WithMany().HasForeignKey(x => x.BuchID);
        });

        modelBuilder.Entity<Buch>()
            .HasMany(b => b.Genres).WithMany(g => g.Buecher)
            .UsingEntity<Dictionary<string, object>>("Buch_Genre",
                r => r.HasOne<Genre>().WithMany().HasForeignKey("GenreID"),
                l => l.HasOne<Buch>().WithMany().HasForeignKey("BuchID"));

        modelBuilder.Entity<Buch>()
            .HasMany(b => b.Altersklassifikationen).WithMany(a => a.Buecher)
            .UsingEntity<Dictionary<string, object>>("Buch_Altersklassifikation",
                r => r.HasOne<Altersklassifikation>().WithMany().HasForeignKey("AltersklassifikationID"),
                l => l.HasOne<Buch>().WithMany().HasForeignKey("BuchID"));
    }
}
