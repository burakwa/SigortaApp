using Microsoft.EntityFrameworkCore;
using SigortaApp.Models;

namespace SigortaApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Police> Policeler => Set<Police>();
    public DbSet<AileBireyi> AileBireyleri => Set<AileBireyi>();
    public DbSet<Hasar> Hasarlar => Set<Hasar>();

    public DbSet<Urun> Urunler => Set<Urun>();
    public DbSet<Teminat> Teminatlar => Set<Teminat>();
    public DbSet<UrunTeminati> UrunTeminatlari => Set<UrunTeminati>();
    public DbSet<TarifeSatiri> Tarifeler => Set<TarifeSatiri>();
    public DbSet<VergiParametresi> Vergiler => Set<VergiParametresi>();
    public DbSet<KisaDonemOrani> KisaDonemOranlari => Set<KisaDonemOrani>();
    public DbSet<PoliceTeminati> PoliceTeminatlari => Set<PoliceTeminati>();
    public DbSet<Zeyil> Zeyiller => Set<Zeyil>();
    public DbSet<Ayar> Ayarlar => Set<Ayar>();

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
        // --- Müşteri ---
        mb.Entity<Musteri>().HasIndex(m => m.TcKimlikNo).IsUnique();
        mb.Entity<Musteri>().HasQueryFilter(m => !m.Silindi);

        // --- Poliçe ---
        mb.Entity<Police>(e =>
        {
            e.HasIndex(p => p.PoliceNo).IsUnique();
            e.Property(p => p.NetPrim).HasPrecision(18, 2);
            e.Property(p => p.VergiTutari).HasPrecision(18, 2);
            e.Property(p => p.Prim).HasPrecision(18, 2);
            e.Property(p => p.KazanilanPrim).HasPrecision(18, 2);
            e.Property(p => p.IadeTutari).HasPrecision(18, 2);
            e.HasQueryFilter(p => !p.Musteri.Silindi);
            e.HasOne(p => p.Urun).WithMany()
                .HasForeignKey(p => p.UrunId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<AileBireyi>().HasQueryFilter(a => !a.Police.Musteri.Silindi);

        mb.Entity<Hasar>().Property(h => h.Tutar).HasPrecision(18, 2);
        mb.Entity<Hasar>().HasQueryFilter(h => !h.Police.Musteri.Silindi);

        // --- Ürün, teminat, tarife, vergi ---
        mb.Entity<Urun>().HasIndex(u => u.Kod).IsUnique();
        mb.Entity<Teminat>().HasIndex(t => t.Kod).IsUnique();

        mb.Entity<UrunTeminati>(e =>
        {
            e.HasIndex(x => new { x.UrunId, x.TeminatId }).IsUnique();
            e.Property(x => x.Limit).HasPrecision(18, 2);
            e.Property(x => x.KatilimPayiOrani).HasPrecision(5, 2);
            e.HasOne(x => x.Teminat).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<TarifeSatiri>().Property(t => t.YillikPrim).HasPrecision(18, 2);
        mb.Entity<VergiParametresi>().Property(v => v.Oran).HasPrecision(9, 4);

        mb.Entity<KisaDonemOrani>(e =>
        {
            e.HasIndex(k => k.MaxGun).IsUnique();
            e.Property(k => k.PrimOrani).HasPrecision(5, 2);
        });

        mb.Entity<Ayar>().HasKey(a => a.Anahtar);

        // --- Poliçeye bağlı kayıtlar ---
        mb.Entity<PoliceTeminati>(e =>
        {
            e.Property(x => x.Limit).HasPrecision(18, 2);
            e.Property(x => x.KatilimPayiOrani).HasPrecision(5, 2);
            e.HasOne(x => x.Teminat).WithMany().OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.Police.Musteri.Silindi);
        });

        mb.Entity<Zeyil>(e =>
        {
            e.HasIndex(z => new { z.PoliceId, z.ZeyilNo }).IsUnique();
            e.Property(z => z.PrimFarki).HasPrecision(18, 2);
            e.HasQueryFilter(z => !z.Police.Musteri.Silindi);
        });
    }
}