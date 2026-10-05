using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Helpers;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class AileBireyiService
{
    public async Task<List<AileBireyi>> ListeleAsync(int policeId)
    {
        using var db = new AppDbContext();
        return await db.AileBireyleri.AsNoTracking()
            .Where(a => a.PoliceId == policeId)
            .OrderBy(a => a.Yakinlik).ThenBy(a => a.Ad)
            .ToListAsync();
    }

    public async Task<Sonuc> EkleAsync(AileBireyi a)
    {
        Temizle(a);
        using var db = new AppDbContext();

        var hata = await DogrulaAsync(db, a);
        if (hata != null) return Sonuc.Hata(hata);

        db.AileBireyleri.Add(a);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Aile bireyi eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(AileBireyi a)
    {
        Temizle(a);
        using var db = new AppDbContext();

        var mevcut = await db.AileBireyleri.FirstOrDefaultAsync(x => x.Id == a.Id);
        if (mevcut == null) return Sonuc.Hata("Aile bireyi bulunamadı.");

        a.PoliceId = mevcut.PoliceId;   // poliçe değiştirilemez
        var hata = await DogrulaAsync(db, a);
        if (hata != null) return Sonuc.Hata(hata);

        mevcut.TcKimlikNo = a.TcKimlikNo;
        mevcut.Ad = a.Ad;
        mevcut.Soyad = a.Soyad;
        mevcut.DogumTarihi = a.DogumTarihi;
        mevcut.Yakinlik = a.Yakinlik;

        await db.SaveChangesAsync();
        return Sonuc.Ok("Aile bireyi güncellendi.");
    }

    public async Task<Sonuc> SilAsync(int id)
    {
        using var db = new AppDbContext();
        var a = await db.AileBireyleri.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return Sonuc.Hata("Aile bireyi bulunamadı.");

        db.AileBireyleri.Remove(a);   // bağımlı yanlış girildiyse düzeltmek için gerçek silme
        await db.SaveChangesAsync();
        return Sonuc.Ok("Aile bireyi silindi.");
    }

    private static void Temizle(AileBireyi a)
    {
        a.TcKimlikNo = a.TcKimlikNo.Trim();
        a.Ad = a.Ad.Trim();
        a.Soyad = a.Soyad.Trim();
    }

    private static async Task<string?> DogrulaAsync(AppDbContext db, AileBireyi a)
    {
        if (!TcKimlikDogrulayici.Gecerlimi(a.TcKimlikNo)) return "Geçersiz T.C. Kimlik No.";
        if (string.IsNullOrWhiteSpace(a.Ad)) return "Ad boş olamaz.";
        if (string.IsNullOrWhiteSpace(a.Soyad)) return "Soyad boş olamaz.";
        if (a.DogumTarihi.Date > DateTime.Today) return "Doğum tarihi gelecekte olamaz.";

        var police = await db.Policeler.Include(p => p.Musteri)
            .FirstOrDefaultAsync(p => p.Id == a.PoliceId);
        if (police == null) return "Poliçe bulunamadı.";
        if (police.Durum == PoliceDurumu.Iptal)
            return "İptal edilmiş poliçede aile bireyi değişikliği yapılamaz.";

        if (police.Musteri.TcKimlikNo == a.TcKimlikNo)
            return "Poliçe sahibi zaten sigortalı, kendisini aile bireyi olarak ekleyemezsin.";

        if (await db.AileBireyleri.AnyAsync(x =>
                x.PoliceId == a.PoliceId && x.TcKimlikNo == a.TcKimlikNo && x.Id != a.Id))
            return "Bu T.C. Kimlik No bu poliçede zaten kayıtlı.";

        if (a.Yakinlik == Yakinlik.Es && await db.AileBireyleri.AnyAsync(x =>
                x.PoliceId == a.PoliceId && x.Yakinlik == Yakinlik.Es && x.Id != a.Id))
            return "Bu poliçede zaten bir eş tanımlı.";

        return null;
    }
}