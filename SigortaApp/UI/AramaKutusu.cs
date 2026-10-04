namespace SigortaApp.UI;

// İçinde büyüteç simgesi olan yuvarlatılmış arama kutusu
public class AramaKutusu : KartPanel
{
    public TextBox Kutu { get; }

    public AramaKutusu(string ipucu)
    {
        Height = 42;
        Yaricap = 10;

        Kutu = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 11f),
            PlaceholderText = ipucu,
            BackColor = Tema.Kart,
            ForeColor = Tema.Metin
        };

        var ikon = new Label
        {
            Text = "\uE721",
            Font = Tema.Ikon,
            ForeColor = Tema.MetinSoluk,
            AutoSize = false,
            Bounds = new Rectangle(8, 0, 30, 42),
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.Add(Kutu);
        Controls.Add(ikon);

        Resize += (_, _) => Yerlestir();
        Yerlestir();
    }

    private void Yerlestir()
    {
        Kutu.Location = new Point(42, Math.Max(0, (Height - Kutu.Height) / 2));
        Kutu.Width = Math.Max(50, Width - 42 - 14);
    }
}