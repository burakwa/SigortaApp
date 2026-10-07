namespace SigortaApp.Models;

public class YenilemeSatiri
{
    public int PoliceId { get; init; }
    public string PoliceNo { get; init; } = "";
    public string MusteriAdSoyad { get; init; } = "";
    public string Telefon { get; init; } = "";
    public DateTime BitisTarihi { get; init; }
    public int KalanGun { get; init; }
    public string Kalan { get; init; } = "";
    public decimal Prim { get; init; }
}

public class DashboardOzeti
{
    public int ToplamMusteri { get; init; }
    public int BagliPolice { get; init; }
    public int BekleyenHasar { get; init; }
    public int YaklasanYenileme { get; init; }
    public int VadesiGecen { get; init; }
    public int HatirlatmaGunu { get; init; }
    public List<YenilemeSatiri> Yenilemeler { get; init; } = new();
}