namespace SigortaApp.Models;

public class PoliceSecenegi
{
    public int Id { get; init; }
    public string Gorunum { get; init; } = "";
    public DateTime Baslangic { get; init; }
    public DateTime Bitis { get; init; }
}