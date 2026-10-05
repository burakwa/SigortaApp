namespace SigortaApp.Models;

public class Musteri
{
    public int Id { get; set; }
    public string TcKimlikNo { get; set; } = "";
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public string Gorunum => $"{AdSoyad}  ·  {TcKimlikNo}";
    public DateTime DogumTarihi { get; set; }
    public string Telefon { get; set; } = "";
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.Now;
    public bool Silindi { get; set; }   // soft delete

    public string AdSoyad => $"{Ad} {Soyad}";
    public List<Police> Policeler { get; set; } = new();
}