using Microsoft.EntityFrameworkCore;
using SigortaApp.Models;

namespace SigortaApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Police> Policeler => Set<Police>();
    public DbSet<AileBireyi> AileBireyleri => Set<AileBireyi>();
    public DbSet<Hasar> Hasarlar => Set<Hasar>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var yol = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SigortaApp", "sigorta.db");
        Directory.CreateDirectory(Path.GetDirectoryName(yol)!);
        options.UseSqlite($"Data Source={yol}");
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Musteri>().HasIndex(m => m.TcKimlikNo).IsUnique();
        mb.Entity<Musteri>().HasQueryFilter(m => !m.Silindi);
        mb.Entity<Police>().HasIndex(p => p.PoliceNo).IsUnique();
        mb.Entity<Police>().Property(p => p.Prim).HasPrecision(18, 2);
        mb.Entity<Hasar>().Property(h => h.Tutar).HasPrecision(18, 2);
    }
}