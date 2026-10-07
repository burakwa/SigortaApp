namespace SigortaApp.Models;

public class UrunSatiri
{
    public int Id { get; init; }
    public string Kod { get; init; } = "";
    public string Ad { get; init; } = "";
    public string Tur { get; init; } = "";
    public int TeminatSayisi { get; init; }
    public string Durum { get; init; } = "";
    public bool Aktif { get; init; }
}

public class UrunTeminatSatiri
{
    public int Id { get; init; }
    public string TeminatKod { get; init; } = "";
    public string TeminatAdi { get; init; } = "";
    public string Limit { get; init; } = "";
    public string KatilimPayi { get; init; } = "";
    public string BeklemeSuresi { get; init; } = "";
    public string Istisnalar { get; init; } = "";
}