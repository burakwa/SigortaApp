namespace SigortaApp.Models;

public class MusteriGecmisi
{
    public Musteri Musteri { get; init; } = null!;
    public List<PoliceSatiri> Policeler { get; init; } = new();
    public List<HasarSatiri> Hasarlar { get; init; } = new();

    public int AktifPoliceSayisi => Policeler.Count(p => p.Durum == "Aktif");
    public decimal ToplamPrim => Policeler.Where(p => p.Durum != "İptal").Sum(p => p.Prim);
    public int BekleyenHasarSayisi => Hasarlar.Count(h => h.DurumKodu == HasarDurumu.Beklemede);
    public int OdenenHasarSayisi => Hasarlar.Count(h => h.DurumKodu == HasarDurumu.Odendi);
    public decimal OdenenHasarTutari => Hasarlar
        .Where(h => h.DurumKodu == HasarDurumu.Odendi).Sum(h => h.Tutar);
}