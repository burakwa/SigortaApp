namespace SigortaApp.Models;

public class Police
{
    public int Id { get; set; }
    public string PoliceNo { get; set; } = "";
    public int MusteriId { get; set; }
    public Musteri Musteri { get; set; } = null!;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public decimal Prim { get; set; }
    public PoliceDurumu Durum { get; set; } = PoliceDurumu.Aktif;

    public List<AileBireyi> AileBireyleri { get; set; } = new();
    public List<Hasar> Hasarlar { get; set; } = new();
}