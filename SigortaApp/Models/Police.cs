namespace SigortaApp.Models;

public class Police
{
    public int Id { get; set; }

    // Teklifte TK-2026-00001, bağlandığında PL-2026-00001
    public string PoliceNo { get; set; } = "";

    public int MusteriId { get; set; }
    public Musteri Musteri { get; set; } = null!;

    public int? UrunId { get; set; }
    public Urun? Urun { get; set; }

    public DateTime TanzimTarihi { get; set; } = DateTime.Now;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }

    public decimal NetPrim { get; set; }
    public decimal VergiTutari { get; set; }
    public decimal Prim { get; set; }   // brüt prim (vergiler dahil)

    public PoliceDurumu Durum { get; set; } = PoliceDurumu.Bagli;

    // Yenileme zinciri: bu poliçe hangi poliçenin yenilemesi
    public int? OncekiPoliceId { get; set; }

    // İptal bilgileri
    public DateTime? IptalTarihi { get; set; }
    public string? IptalNedeni { get; set; }
    public IptalYontemi? IptalHesapYontemi { get; set; }
    public decimal? KazanilanPrim { get; set; }
    public decimal? IadeTutari { get; set; }

    public List<AileBireyi> AileBireyleri { get; set; } = new();
    public List<Hasar> Hasarlar { get; set; } = new();
    public List<PoliceTeminati> Teminatlar { get; set; } = new();
    public List<Zeyil> Zeyiller { get; set; } = new();
}