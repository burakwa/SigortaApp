namespace SigortaApp.Models;

// Ürün teminatının poliçeye kopyalanmış hali: ürün sonradan değişse de poliçe etkilenmez
public class PoliceTeminati
{
    public int Id { get; set; }
    public int PoliceId { get; set; }
    public Police Police { get; set; } = null!;
    public int TeminatId { get; set; }
    public Teminat Teminat { get; set; } = null!;

    public string TeminatAdi { get; set; } = "";
    public decimal? Limit { get; set; }
    public decimal KatilimPayiOrani { get; set; }
    public int BeklemeSuresiGun { get; set; }
    public string? Istisnalar { get; set; }

    public bool Aktif { get; set; } = true;
    public DateTime EklenmeTarihi { get; set; } = DateTime.Today;
    public DateTime? CikarilmaTarihi { get; set; }
}

// Poliçe ara dönem değişikliği
public class Zeyil
{
    public int Id { get; set; }
    public int PoliceId { get; set; }
    public Police Police { get; set; } = null!;
    public int ZeyilNo { get; set; }                 // poliçe içinde 1, 2, 3...
    public ZeyilTuru Tur { get; set; }
    public DateTime YururlukTarihi { get; set; }
    public string Aciklama { get; set; } = "";
    public decimal PrimFarki { get; set; }           // + ek prim, - iade
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}