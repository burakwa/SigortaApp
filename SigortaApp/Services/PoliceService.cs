using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class PoliceService
{
    public async Task<List<PoliceSatiri>> AraAsync(string? metin)
    {
        using var db = new AppDbContext();
        var q = db.Policeler.AsNoTracking().Include(p => p.Musteri).AsQueryable();

        if (!string.IsNullOrWhiteSpace(metin))
        {
            foreach (var kelime in metin.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(p => p.PoliceNo.Contains(k) || p.Musteri.Ad.Contains(k)
                              || p.Musteri.Soyad.Contains(k) || p.Musteri.TcKimlikNo.Contains(k));
            }
        }

        var liste = await q.OrderByDescending(p => p.BaslangicTarihi).ToListAsync();

        return liste.Select(p => new PoliceSatiri
        {
            Id = p.Id,
            PoliceNo = p.PoliceNo,
            MusteriAdSoyad = p.Musteri.AdSoyad,
            BaslangicTarihi = p.BaslangicTarihi,
            BitisTarihi = p.BitisTarihi,
            Prim = p.Prim,
            Durum = DurumMetni(p)
        }).ToList();
    }

    public async Task<Police?> GetirAsync(int id)
    {
        using var db = new AppDbContext();
        return await db.Policeler.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<string> YeniPoliceNoAsync()
    {
        using var db = new AppDbContext();
        string onEk = $"PL-{DateTime.Now.Year}-";

        var mevcutlar = await db.Policeler.IgnoreQueryFilters()
            .Where(p => p.PoliceNo.StartsWith(onEk))
            .Select(p => p.PoliceNo)
            .ToListAsync();

        int enBuyuk = 0;
        foreach (var no in mevcutlar)
            if (int.TryParse(no[onEk.Length..], out int n) && n > enBuyuk) enBuyuk = n;

        return $"{onEk}{enBuyuk + 1:D5}";
    }

    public async Task<Sonuc> EkleAsync(Police p)
    {
        var hata = Dogrula(p);
        if (hata != null) return Sonuc.Hata(hata);

        p.PoliceNo = p.PoliceNo.Trim();

        using var db = new AppDbContext();

        if (!await db.Musteriler.AnyAsync(m => m.Id == p.MusteriId))
            return Sonuc.Hata("Müşteri bulunamadı.");

        if (await db.Policeler.IgnoreQueryFilters().AnyAsync(x => x.PoliceNo == p.PoliceNo))
            return Sonuc.Hata("Bu poliçe no zaten kullanılıyor.");

        p.Durum = PoliceDurumu.Bagli;
        db.Policeler.Add(p);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Poliçe eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(Police p)
    {
        var hata = Dogrula(p);
        if (hata != null) return Sonuc.Hata(hata);

        p.PoliceNo = p.PoliceNo.Trim();

        using var db = new AppDbContext();

        var mevcut = await db.Policeler.FirstOrDefaultAsync(x => x.Id == p.Id);
        if (mevcut == null) return Sonuc.Hata("Poliçe bulunamadı.");

        if (await db.Policeler.IgnoreQueryFilters().AnyAsync(x => x.PoliceNo == p.PoliceNo && x.Id != p.Id))
            return Sonuc.Hata("Bu poliçe no başka bir poliçede kullanılıyor.");

        mevcut.PoliceNo = p.PoliceNo;
        mevcut.BaslangicTarihi = p.BaslangicTarihi;
        mevcut.BitisTarihi = p.BitisTarihi;
        mevcut.Prim = p.Prim;
        mevcut.Durum = p.Durum;   // müşteri bilerek değiştirilmiyor

        await db.SaveChangesAsync();
        return Sonuc.Ok("Poliçe güncellendi.");
    }

    public async Task<Sonuc> IptalEtAsync(int id)
    {
        using var db = new AppDbContext();
        var p = await db.Policeler.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return Sonuc.Hata("Poliçe bulunamadı.");
        if (p.Durum == PoliceDurumu.Iptal) return Sonuc.Hata("Poliçe zaten iptal edilmiş.");

        p.Durum = PoliceDurumu.Iptal;
        await db.SaveChangesAsync();
        return Sonuc.Ok("Poliçe iptal edildi.");
    }

    public static string DurumMetni(Police p)
    {
        switch (p.Durum)
        {
            case PoliceDurumu.Teklif: return "Teklif";
            case PoliceDurumu.Iptal: return "İptal";
            case PoliceDurumu.Yenilendi: return "Yenilendi";
        }
        return p.BitisTarihi.Date < DateTime.Today ? "Süresi Dolmuş" : "Aktif";
    }

    private static string? Dogrula(Police p)
    {
        if (string.IsNullOrWhiteSpace(p.PoliceNo)) return "Poliçe no boş olamaz.";
        if (p.MusteriId <= 0) return "Müşteri seçmelisin.";
        if (p.BitisTarihi.Date <= p.BaslangicTarihi.Date) return "Bitiş tarihi başlangıç tarihinden sonra olmalı.";
        if (p.Prim <= 0) return "Prim 0'dan büyük olmalı.";
        return null;
    }

    public async Task<List<PoliceSecenegi>> HasarIcinSecenekleriGetirAsync()
    {
        using var db = new AppDbContext();
        var liste = await db.Policeler.AsNoTracking()
            .Include(p => p.Musteri)
            .Where(p => p.Durum != PoliceDurumu.Iptal && p.Durum != PoliceDurumu.Teklif)
            .OrderByDescending(p => p.BaslangicTarihi)
            .ToListAsync();

        return liste.Select(p => new PoliceSecenegi
        {
            Id = p.Id,
            Gorunum = $"{p.PoliceNo}  ·  {p.Musteri.AdSoyad}",
            Baslangic = p.BaslangicTarihi,
            Bitis = p.BitisTarihi
        }).ToList();
    }
}