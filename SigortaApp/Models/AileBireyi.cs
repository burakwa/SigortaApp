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
    public string AdSoyad => $"{Ad} {Soyad}";

    public string YakinlikMetni => Yakinlik switch
    {
        Yakinlik.Es => "Eş",
        Yakinlik.Cocuk => "Çocuk",
        Yakinlik.Anne => "Anne",
        Yakinlik.Baba => "Baba",
        _ => "Diğer"
    };

    public int Yas
    {
        get
        {
            var bugun = DateTime.Today;
            int yas = bugun.Year - DogumTarihi.Year;
            if (DogumTarihi.Date > bugun.AddYears(-yas)) yas--;
            return yas;
        }
    }
}