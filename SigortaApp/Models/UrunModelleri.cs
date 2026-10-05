namespace SigortaApp.Models;

public class Urun
{
    public int Id { get; set; }
    public string Kod { get; set; } = "";
    public string Ad { get; set; } = "";
    public UrunTuru Tur { get; set; }
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; } = true;

    public List<UrunTeminati> Teminatlar { get; set; } = new();
    public List<TarifeSatiri> Tarifeler { get; set; } = new();
}

// Teminat kataloğu: ürünlerden bağımsız genel tanımlar (Yatarak tedavi, Doğum, Check-up...)
public class Teminat
{
    public int Id { get; set; }
    public string Kod { get; set; } = "";
    public string Ad { get; set; } = "";
    public string? Aciklama { get; set; }
}

// Bir ürünün sunduğu teminat ve koşulları
public class UrunTeminati
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public int TeminatId { get; set; }
    public Teminat Teminat { get; set; } = null!;

    public decimal? Limit { get; set; }              // null = limitsiz
    public decimal KatilimPayiOrani { get; set; }    // yüzde (0-100)
    public int BeklemeSuresiGun { get; set; }
    public string? Istisnalar { get; set; }
}

// Yaş aralığı ve gruba göre yıllık net prim
public class TarifeSatiri
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public TarifeGrubu Grup { get; set; }
    public int YasMin { get; set; }
    public int YasMax { get; set; }
    public decimal YillikPrim { get; set; }
    public DateTime GecerliBaslangic { get; set; } = DateTime.Today;
    public DateTime? GecerliBitis { get; set; }
}

// KDV, ÖİV gibi kalemler: ad ve oranı sen tanımlarsın, kodda sabit değer yok
public class VergiParametresi
{
    public int Id { get; set; }
    public string Ad { get; set; } = "";
    public decimal Oran { get; set; }                // yüzde
    public int? UrunId { get; set; }                 // null = tüm ürünlere uygulanır
    public bool Aktif { get; set; } = true;
    public DateTime GecerliBaslangic { get; set; } = DateTime.Today;
}

// Kısa dönem iptal tarifesi: poliçe MaxGun gün veya daha az sürdüyse prim oranı
public class KisaDonemOrani
{
    public int Id { get; set; }
    public int MaxGun { get; set; }
    public decimal PrimOrani { get; set; }           // yüzde
}

// Basit ayar tablosu (örnek: hatırlatma gün sayısı)
public class Ayar
{
    public string Anahtar { get; set; } = "";
    public string Deger { get; set; } = "";
}