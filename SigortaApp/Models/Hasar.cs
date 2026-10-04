namespace SigortaApp.Models;

public class Hasar
{
    public int Id { get; set; }
    public int PoliceId { get; set; }
    public Police Police { get; set; } = null!;
    public DateTime HasarTarihi { get; set; }
    public string Aciklama { get; set; } = "";
    public decimal Tutar { get; set; }
    public HasarDurumu Durum { get; set; } = HasarDurumu.Beklemede;
}