namespace SigortaApp.Models;

public class AileBireyi
{
    public int Id { get; set; }
    public int PoliceId { get; set; }
    public Police Police { get; set; } = null!;
    public string TcKimlikNo { get; set; } = "";
    public string Ad { get; set; } = "";
    public string Soyad { get; set; } = "";
    public DateTime DogumTarihi { get; set; }
    public Yakinlik Yakinlik { get; set; }
}