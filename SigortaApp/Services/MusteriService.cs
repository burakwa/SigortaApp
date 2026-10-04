using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Helpers;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class MusteriService
{
    public async Task<List<Musteri>> ListeleAsync()
    {
        using var db = new AppDbContext();
        return await db.Musteriler
            .OrderBy(m => m.Ad).ThenBy(m => m.Soyad)
            .ToListAsync();
    }

    public async Task<Musteri?> GetirAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.Musteriler.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Sonuc> EkleAsync(Musteri m)
    {
        var hata = Dogrula(m);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();

        // Silinmiş kayıtlar da dahil: unique index bunlara da takılır
        bool varMi = await db.Musteriler.IgnoreQueryFilters()
            .AnyAsync(x => x.TcKimlikNo == m.TcKimlikNo);
        if (varMi) return Sonuc.Hata("Bu T.C. Kimlik No ile kayıtlı bir müşteri zaten var.");

        m.KayitTarihi = DateTime.Now;
        db.Musteriler.Add(m);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Müşteri eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(Musteri m)
    {
        var hata = Dogrula(m);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();

        bool baskasiVar = await db.Musteriler.IgnoreQueryFilters()
            .AnyAsync(x => x.TcKimlikNo == m.TcKimlikNo && x.Id != m.Id);
        if (baskasiVar) return Sonuc.Hata("Bu T.C. Kimlik No başka bir müşteriye ait.");

        var mevcut = await db.Musteriler.FirstOrDefaultAsync(x => x.Id == m.Id);
        if (mevcut == null) return Sonuc.Hata("Müşteri bulunamadı.");

        mevcut.TcKimlikNo  = m.TcKimlikNo;
        mevcut.Ad          = m.Ad.Trim();
        mevcut.Soyad       = m.Soyad.Trim();
        mevcut.DogumTarihi = m.DogumTarihi;
        mevcut.Telefon     = m.Telefon.Trim();
        mevcut.Email       = m.Email?.Trim();
        mevcut.Adres       = m.Adres?.Trim();

        await db.SaveChangesAsync();
        return Sonuc.Ok("Müşteri güncellendi.");
    }

    public async Task<Sonuc> SilAsync(int id)
    {
        using var db = new AppDbContext();
        var m = await db.Musteriler.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return Sonuc.Hata("Müşteri bulunamadı.");

        m.Silindi = true;   // soft delete
        await db.SaveChangesAsync();
        return Sonuc.Ok("Müşteri silindi.");
    }

    private static string? Dogrula(Musteri m)
    {
        if (!TcKimlikDogrulayici.Gecerlimi(m.TcKimlikNo)) return "Geçersiz T.C. Kimlik No.";
        if (string.IsNullOrWhiteSpace(m.Ad))      return "Ad boş olamaz.";
        if (string.IsNullOrWhiteSpace(m.Soyad))   return "Soyad boş olamaz.";
        if (string.IsNullOrWhiteSpace(m.Telefon)) return "Telefon boş olamaz.";
        if (m.DogumTarihi > DateTime.Today)       return "Doğum tarihi gelecekte olamaz.";
        return null;
    }

    public async Task<List<Musteri>> AraAsync(string? metin)
    {
        using var db = new AppDbContext();
        var q = db.Musteriler.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(metin))
        {
            // Her kelime TC, ad, soyad veya telefonda geçmeli ("ahmet yıl" gibi aramalar çalışır)
            foreach (var kelime in metin.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(m => m.TcKimlikNo.Contains(k) || m.Ad.Contains(k)
                              || m.Soyad.Contains(k) || m.Telefon.Contains(k));
            }
        }

        return await q.OrderBy(m => m.Ad).ThenBy(m => m.Soyad).ToListAsync();
    }
}