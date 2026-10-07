using Microsoft.EntityFrameworkCore;
using SigortaApp.Data;
using SigortaApp.Models;

namespace SigortaApp.Services;

public class TeminatService
{
    public async Task<List<Teminat>> AraAsync(string? metin)
    {
        using var db = new AppDbContext();
        var q = db.Teminatlar.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(metin))
        {
            foreach (var kelime in metin.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(t => t.Kod.Contains(k) || t.Ad.Contains(k));
            }
        }

        return await q.OrderBy(t => t.Ad).ToListAsync();
    }

    public async Task<Sonuc> EkleAsync(Teminat t)
    {
        Temizle(t);
        var hata = Dogrula(t);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();
        if (await db.Teminatlar.AnyAsync(x => x.Kod == t.Kod))
            return Sonuc.Hata("Bu kod başka bir teminatta kullanılıyor.");

        db.Teminatlar.Add(t);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Teminat eklendi.");
    }

    public async Task<Sonuc> GuncelleAsync(Teminat t)
    {
        Temizle(t);
        var hata = Dogrula(t);
        if (hata != null) return Sonuc.Hata(hata);

        using var db = new AppDbContext();
        var mevcut = await db.Teminatlar.FirstOrDefaultAsync(x => x.Id == t.Id);
        if (mevcut == null) return Sonuc.Hata("Teminat bulunamadı.");

        if (await db.Teminatlar.AnyAsync(x => x.Kod == t.Kod && x.Id != t.Id))
            return Sonuc.Hata("Bu kod başka bir teminatta kullanılıyor.");

        mevcut.Kod = t.Kod;
        mevcut.Ad = t.Ad;
        mevcut.Aciklama = t.Aciklama;

        await db.SaveChangesAsync();
        return Sonuc.Ok("Teminat güncellendi.");
    }

    public async Task<Sonuc> SilAsync(int id)
    {
        using var db = new AppDbContext();
        var t = await db.Teminatlar.FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return Sonuc.Hata("Teminat bulunamadı.");

        int urunSayisi = await db.UrunTeminatlari.CountAsync(x => x.TeminatId == id);
        int policeSayisi = await db.PoliceTeminatlari.IgnoreQueryFilters().CountAsync(x => x.TeminatId == id);
        if (urunSayisi > 0 || policeSayisi > 0)
            return Sonuc.Hata($"Bu teminat kullanımda ({urunSayisi} üründe, {policeSayisi} poliçede), silinemez.");

        db.Teminatlar.Remove(t);
        await db.SaveChangesAsync();
        return Sonuc.Ok("Teminat silindi.");
    }

    private static void Temizle(Teminat t)
    {
        t.Kod = t.Kod.Trim().ToUpperInvariant();
        t.Ad = t.Ad.Trim();
        t.Aciklama = string.IsNullOrWhiteSpace(t.Aciklama) ? null : t.Aciklama.Trim();
    }

    private static string? Dogrula(Teminat t)
    {
        if (string.IsNullOrWhiteSpace(t.Kod)) return "Kod boş olamaz.";
        if (string.IsNullOrWhiteSpace(t.Ad)) return "Teminat adı boş olamaz.";
        return null;
    }
}