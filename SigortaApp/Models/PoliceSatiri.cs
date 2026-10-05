namespace SigortaApp.Models;

public class PoliceSatiri
{
    public int Id { get; init; }
    public string PoliceNo { get; init; } = "";
    public string MusteriAdSoyad { get; init; } = "";
    public DateTime BaslangicTarihi { get; init; }
    public DateTime BitisTarihi { get; init; }
    public decimal Prim { get; init; }
    public string Durum { get; init; } = "";
}