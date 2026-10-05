using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class MusteriGecmisService
{
    public async Task<MusteriGecmisi?> GetirAsync(int musteriId)
    {
        using var db = new AppDbContext();

        var musteri = await db.Musteriler.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == musteriId);
        if (musteri == null) return null;

        var policeler = await db.Policeler.AsNoTracking()
            .Where(p => p.MusteriId == musteriId)
            .OrderByDescending(p => p.BaslangicTarihi)
            .ToListAsync();

        var hasarlar = await db.Hasarlar.AsNoTracking()
            .Include(h => h.Police)
            .Where(h => h.Police.MusteriId == musteriId)
            .OrderByDescending(h => h.HasarTarihi)
            .ToListAsync();

        return new MusteriGecmisi
        {
            Musteri = musteri,
            Policeler = policeler.Select(p => new PoliceSatiri
            {
                Id = p.Id,
                PoliceNo = p.PoliceNo,
                MusteriAdSoyad = musteri.AdSoyad,
                BaslangicTarihi = p.BaslangicTarihi,
                BitisTarihi = p.BitisTarihi,
                Prim = p.Prim,
                Durum = PoliceService.DurumMetni(p)
            }).ToList(),
            Hasarlar = hasarlar.Select(h => new HasarSatiri
            {
                Id = h.Id,
                PoliceNo = h.Police.PoliceNo,
                MusteriAdSoyad = musteri.AdSoyad,
                HasarTarihi = h.HasarTarihi,
                Aciklama = h.Aciklama,
                Tutar = h.Tutar,
                Durum = HasarService.DurumMetni(h.Durum),
                DurumKodu = h.Durum
            }).ToList()
        };
    }
}