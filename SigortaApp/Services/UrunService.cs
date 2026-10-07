using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class UrunService
{
    // ---------- Ürün ----------

    public async Task<List<UrunSatiri>> AraAsync(string? metin)
    {
        using var db = new AppDbContext();
        var q = db.Urunler.AsNoTracking().Include(u => u.Teminatlar).AsQueryable();

        if (!string.IsNullOrWhiteSpace(metin))
        {
            foreach (var kelime in metin.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(u => u.Kod.Contains(k) || u.Ad.Contains(k));
            }
        }

        var liste = await q.OrderBy(u => u.Ad).ToListAsync();

        return liste.Select(u => new UrunSatiri
        {
            Id = u.Id,
            Kod = u.Kod,
            Ad = u.Ad,
            Tur = TurMetni(u.Tur),
            TeminatSayisi = u.Teminatlar.Count,
            Aktif = u.Aktif,
            Durum = u.Aktif ? "Aktif" : "Pasif"
        }).ToList();
    }

    public async Task<Urun?> GetirAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.Urunler.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Sonuc> EkleAsync(Urun u)
    {
        Temizle(u);
        var hata = Dogrula(u);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();
        if (await db.Urunler.AnyAsync(x => x.Kod == u.Kod))
            return Sonuc.Hata("Bu kod başka bir üründe kullanılıyor.");

        u.Aktif = true;
        db.Urunler.Add(u);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Ürün eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(Urun u)
    {
        Temizle(u);
        var hata = Dogrula(u);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();
        var mevcut = await db.Urunler.FirstOrDefaultAsync(x => x.Id == u.Id);
        if (mevcut == null) return Sonuc.Hata("Ürün bulunamadı.");

        if (await db.Urunler.AnyAsync(x => x.Kod == u.Kod && x.Id != u.Id))
            return Sonuc.Hata("Bu kod başka bir üründe kullanılıyor.");

        mevcut.Kod = u.Kod;
        mevcut.Ad = u.Ad;
        mevcut.Tur = u.Tur;
        mevcut.Aciklama = u.Aciklama;

        await db.SaveChangesAsync();
        return Sonuc.Ok("Ürün güncellendi.");
    }

    public async Task<Sonuc> DurumDegistirAsync(int id)
    {
        using var db = new AppDbContext();
        var u = await db.Urunler.FirstOrDefaultAsync(x => x.Id == id);
        if (u == null) return Sonuc.Hata("Ürün bulunamadı.");

        u.Aktif = !u.Aktif;
        await db.SaveChangesAsync();
        return Sonuc.Ok(u.Aktif ? "Ürün aktif edildi." : "Ürün pasife alındı.");
    }

    public static string TurMetni(UrunTuru t) => t switch
    {
        UrunTuru.TamamlayiciSaglik => "Tamamlayıcı Sağlık (TSS)",
        UrunTuru.OzelSaglik => "Özel Sağlık",
        UrunTuru.YatarakTedavi => "Yatarak Tedavi",
        _ => "Ayakta Tedavi"
    };

    private static void Temizle(Urun u)
    {
        u.Kod = u.Kod.Trim().ToUpperInvariant();
        u.Ad = u.Ad.Trim();
        u.Aciklama = string.IsNullOrWhiteSpace(u.Aciklama) ? null : u.Aciklama.Trim();
    }

    private static string? Dogrula(Urun u)
    {
        if (string.IsNullOrWhiteSpace(u.Kod)) return "Ürün kodu boş olamaz.";
        if (string.IsNullOrWhiteSpace(u.Ad)) return "Ürün adı boş olamaz.";
        return null;
    }

    // ---------- Ürün teminatları ----------

    public async Task<List<UrunTeminatSatiri>> TeminatlariListeleAsync(int urunId)
    {
        using var db = new AppDbContext();
        var liste = await db.UrunTeminatlari.AsNoTracking()
            .Include(x => x.Teminat)
            .Where(x => x.UrunId == urunId)
            .ToListAsync();

        return liste
            .OrderBy(x => x.Teminat.Ad)
            .Select(x => new UrunTeminatSatiri
            {
                Id = x.Id,
                TeminatKod = x.Teminat.Kod,
                TeminatAdi = x.Teminat.Ad,
                Limit = x.Limit == null ? "Limitsiz" : $"{x.Limit:N2} ₺",
                KatilimPayi = $"%{x.KatilimPayiOrani:0.##}",
                BeklemeSuresi = x.BeklemeSuresiGun == 0 ? "Yok" : $"{x.BeklemeSuresiGun} gün",
                Istisnalar = x.Istisnalar ?? ""
            }).ToList();
    }

    public async Task<UrunTeminati?> TeminatGetirAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.UrunTeminatlari.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    // Ürüne henüz eklenmemiş teminatlar
    public async Task<List<Teminat>> EklenebilirTeminatlarAsync(int urunId)
    {
        using var db = new AppDbContext();
        var ekli = await db.UrunTeminatlari
            .Where(x => x.UrunId == urunId)
            .Select(x => x.TeminatId)
            .ToListAsync();

        return await db.Teminatlar.AsNoTracking()
            .Where(t => !ekli.Contains(t.Id))
            .OrderBy(t => t.Ad)
            .ToListAsync();
    }

    public async Task<Sonuc> TeminatKaydetAsync(UrunTeminati ut)
    {
        ut.Istisnalar = string.IsNullOrWhiteSpace(ut.Istisnalar) ? null : ut.Istisnalar.Trim();

        if (ut.KatilimPayiOrani < 0 || ut.KatilimPayiOrani > 100)
            return Sonuc.Hata("Katılım payı 0 ile 100 arasında olmalı.");
        if (ut.Limit.HasValue && ut.Limit.Value <= 0)
            return Sonuc.Hata("Limit 0'dan büyük olmalı (limitsiz için 'Limitsiz'i işaretle).");
        if (ut.BeklemeSuresiGun < 0)
            return Sonuc.Hata("Bekleme süresi negatif olamaz.");

        using var db = new AppDbContext();

        if (!await db.Urunler.AnyAsync(u => u.Id == ut.UrunId))
            return Sonuc.Hata("Ürün bulunamadı.");

        if (ut.Id == 0)
        {
            if (ut.TeminatId <= 0) return Sonuc.Hata("Teminat seçmelisin.");
            if (!await db.Teminatlar.AnyAsync(t => t.Id == ut.TeminatId))
                return Sonuc.Hata("Teminat bulunamadı.");
            if (await db.UrunTeminatlari.AnyAsync(x => x.UrunId == ut.UrunId && x.TeminatId == ut.TeminatId))
                return Sonuc.Hata("Bu teminat üründe zaten tanımlı.");

            db.UrunTeminatlari.Add(ut);
            await db.SaveChangesAsync();
            return Sonuc.Ok("Teminat eklendi.");
        }

        var mevcut = await db.UrunTeminatlari.FirstOrDefaultAsync(x => x.Id == ut.Id);
        if (mevcut == null) return Sonuc.Hata("Kayıt bulunamadı.");

        mevcut.Limit = ut.Limit;
        mevcut.KatilimPayiOrani = ut.KatilimPayiOrani;
        mevcut.BeklemeSuresiGun = ut.BeklemeSuresiGun;
        mevcut.Istisnalar = ut.Istisnalar;

        await db.SaveChangesAsync();
        return Sonuc.Ok("Teminat güncellendi.");
    }

    // Poliçelerde teminatlar kopya olduğu için ürün teminatını çıkarmak mevcut poliçeleri etkilemez
    public async Task<Sonuc> TeminatCikarAsync(int id)
    {
        using var db = new AppDbContext();
        var x = await db.UrunTeminatlari.FirstOrDefaultAsync(t => t.Id == id);
        if (x == null) return Sonuc.Hata("Kayıt bulunamadı.");

        db.UrunTeminatlari.Remove(x);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Teminat üründen çıkarıldı.");
    }
}