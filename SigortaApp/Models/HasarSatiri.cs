namespace SigortaApp.Models;

public class HasarSatiri
{
    public int Id { get; init; }
    public string PoliceNo { get; init; } = "";
    public string MusteriAdSoyad { get; init; } = "";
    public DateTime HasarTarihi { get; init; }
    public string Aciklama { get; init; } = "";
    public decimal Tutar { get; init; }
    public string Durum { get; init; } = "";
    public HasarDurumu DurumKodu { get; init; }
}