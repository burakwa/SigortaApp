namespace SigortaApp.Services;

public class Sonuc
{
    public bool Basarili { get; init; }
    public string Mesaj { get; init; } = "";

    public static Sonuc Ok(string mesaj = "") => new() { Basarili = true, Mesaj = mesaj };
    public static Sonuc Hata(string mesaj) => new() { Basarili = false, Mesaj = mesaj };
}