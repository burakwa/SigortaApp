using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class DashboardService
{
    // Adım 16'da ayarlardan okunacak
    public const int HatirlatmaGunu = 30;

    public async Task<DashboardOzeti> GetirAsync()
    {
        using var db = new AppDbContext();
        var bugun = DateTime.Today;
        var sinir = bugun.AddDays(HatirlatmaGunu);

        int musteri = await db.Musteriler.CountAsync();

        int bagli = await db.Policeler.CountAsync(p =>
            p.Durum == PoliceDurumu.Bagli && p.BitisTarihi >= bugun);

        int bekleyen = await db.Hasarlar.CountAsync(h => h.Durum == HasarDurumu.Beklemede);

        int yaklasan = await db.Policeler.CountAsync(p =>
            p.Durum == PoliceDurumu.Bagli && p.BitisTarihi >= bugun && p.BitisTarihi <= sinir);

        int vadesiGecen = await db.Policeler.CountAsync(p =>
            p.Durum == PoliceDurumu.Bagli && p.BitisTarihi < bugun);

        // Yakında bitenler önde (en yakın tarih ilk), vadesi geçenler arkada (en yeni ilk)
        var yaklasanlar = await db.Policeler.AsNoTracking().Include(p => p.Musteri)
            .Where(p => p.Durum == PoliceDurumu.Bagli && p.BitisTarihi >= bugun && p.BitisTarihi <= sinir)
            .OrderBy(p => p.BitisTarihi)
            .Take(50).ToListAsync();

        var gecenler = await db.Policeler.AsNoTracking().Include(p => p.Musteri)
            .Where(p => p.Durum == PoliceDurumu.Bagli && p.BitisTarihi < bugun)
            .OrderByDescending(p => p.BitisTarihi)
            .Take(50).ToListAsync();

        var satirlar = yaklasanlar.Concat(gecenler)
            .Take(50)
            .Select(p => SatirOlustur(p, bugun))
            .ToList();

        return new DashboardOzeti
        {
            ToplamMusteri = musteri,
            BagliPolice = bagli,
            BekleyenHasar = bekleyen,
            YaklasanYenileme = yaklasan,
            VadesiGecen = vadesiGecen,
            HatirlatmaGunu = HatirlatmaGunu,
            Yenilemeler = satirlar
        };
    }

    private static YenilemeSatiri SatirOlustur(Police p, DateTime bugun)
    {
        int kalan = (p.BitisTarihi.Date - bugun).Days;

        return new YenilemeSatiri
        {
            PoliceId = p.Id,
            PoliceNo = p.PoliceNo,
            MusteriAdSoyad = p.Musteri.AdSoyad,
            Telefon = p.Musteri.Telefon,
            BitisTarihi = p.BitisTarihi,
            KalanGun = kalan,
            Kalan = kalan < 0 ? $"{-kalan} gün geçti" : kalan == 0 ? "Bugün" : $"{kalan} gün",
            Prim = p.Prim
        };
    }
}