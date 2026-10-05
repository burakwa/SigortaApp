using System.ComponentModel;

namespace SigortaApp.UI;

// Başlık, büyük değer ve alt not içeren küçük özet kartı
public class OzetKarti : KartPanel
{
    private readonly Label _lblDeger;
    private readonly Label _lblAlt;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Deger { get => _lblDeger.Text; set => _lblDeger.Text = value; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Alt { get => _lblAlt.Text; set => _lblAlt.Text = value; }

    public OzetKarti(string baslik)
    {
        Padding = new Padding(20, 14, 16, 8);

        var lblBaslik = new Label
        {
            Text = baslik,
            Font = Tema.KucukKalin,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 22,
            AutoSize = false
        };
        _lblDeger = new Label
        {
            Text = "–",
            Font = new Font("Segoe UI Semibold", 16f),
            ForeColor = Tema.Metin,
            Dock = DockStyle.Top,
            Height = 44,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };
        _lblAlt = new Label
        {
            Font = Tema.Kucuk,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 22,
            AutoSize = false
        };

        // Dock.Top: son eklenen en üstte durur
        Controls.Add(_lblAlt);
        Controls.Add(_lblDeger);
        Controls.Add(lblBaslik);
    }
}