using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class HasarService
{
    public async Task<List<HasarSatiri>> AraAsync(string? metin)
    {
        using var db = new AppDbContext();
        var q = db.Hasarlar.AsNoTracking()
            .Include(h => h.Police).ThenInclude(p => p.Musteri)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(metin))
        {
            foreach (var kelime in metin.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(h => h.Police.PoliceNo.Contains(k)
                              || h.Police.Musteri.Ad.Contains(k)
                              || h.Police.Musteri.Soyad.Contains(k)
                              || h.Police.Musteri.TcKimlikNo.Contains(k)
                              || h.Aciklama.Contains(k));
            }
        }

        var liste = await q.OrderByDescending(h => h.HasarTarihi).ToListAsync();

        return liste.Select(h => new HasarSatiri
        {
            Id = h.Id,
            PoliceNo = h.Police.PoliceNo,
            MusteriAdSoyad = h.Police.Musteri.AdSoyad,
            HasarTarihi = h.HasarTarihi,
            Aciklama = h.Aciklama,
            Tutar = h.Tutar,
            Durum = DurumMetni(h.Durum),
            DurumKodu = h.Durum
        }).ToList();
    }

    public async Task<Hasar?> GetirAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.Hasarlar.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Sonuc> EkleAsync(Hasar h)
    {
        h.Aciklama = h.Aciklama.Trim();
        using var db = new AppDbContext();

        var hata = await DogrulaAsync(db, h, yeni: true);
        if (hata != null) return Sonuc.Hata(hata);

        h.Durum = HasarDurumu.Beklemede;
        db.Hasarlar.Add(h);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Hasar kaydı eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(Hasar h)
    {
        h.Aciklama = h.Aciklama.Trim();
        using var db = new AppDbContext();

        var mevcut = await db.Hasarlar.FirstOrDefaultAsync(x => x.Id == h.Id);
        if (mevcut == null) return Sonuc.Hata("Hasar kaydı bulunamadı.");
        if (mevcut.Durum == HasarDurumu.Odendi)
            return Sonuc.Hata("Ödenmiş hasar kaydı değiştirilemez.");

        h.PoliceId = mevcut.PoliceId;   // poliçe değiştirilemez
        var hata = await DogrulaAsync(db, h, yeni: false);
        if (hata != null) return Sonuc.Hata(hata);

        mevcut.HasarTarihi = h.HasarTarihi;
        mevcut.Aciklama = h.Aciklama;
        mevcut.Tutar = h.Tutar;
        mevcut.Durum = h.Durum;

        await db.SaveChangesAsync();
        return Sonuc.Ok("Hasar kaydı güncellendi.");
    }

    public static string DurumMetni(HasarDurumu d) => d switch
    {
        HasarDurumu.Beklemede => "Beklemede",
        HasarDurumu.Onaylandi => "Onaylandı",
        HasarDurumu.Reddedildi => "Reddedildi",
        _ => "Ödendi"
    };

    private static async Task<string?> DogrulaAsync(AppDbContext db, Hasar h, bool yeni)
    {
        if (string.IsNullOrWhiteSpace(h.Aciklama)) return "Açıklama boş olamaz.";
        if (h.Tutar <= 0) return "Tutar 0'dan büyük olmalı.";
        if (h.HasarTarihi.Date > DateTime.Today) return "Hasar tarihi gelecekte olamaz.";

        var police = await db.Policeler.AsNoTracking().FirstOrDefaultAsync(p => p.Id == h.PoliceId);
        if (police == null) return "Poliçe bulunamadı.";

        // İptal kontrolü yalnızca yeni kayıtta: önceden açılmış hasar, poliçe iptal olsa da işlenebilir
        if (yeni && police.Durum == PoliceDurumu.Iptal)
            return "İptal edilmiş poliçeye hasar eklenemez.";

        if (h.HasarTarihi.Date < police.BaslangicTarihi.Date || h.HasarTarihi.Date > police.BitisTarihi.Date)
            return $"Hasar tarihi poliçe dönemi içinde olmalı ({police.BaslangicTarihi:dd.MM.yyyy} – {police.BitisTarihi:dd.MM.yyyy}).";

        return null;
    }
}