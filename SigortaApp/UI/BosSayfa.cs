namespace SigortaApp.UI;

public class BosSayfa : UserControl
{
    public BosSayfa(string baslik, string aciklama)
    {
        BackColor = Tema.Arkaplan;

        var lblAciklama = new Label
        {
            Text = aciklama, Font = Tema.Normal, ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top, Height = 30
        };
        var lblBaslik = new Label
        {
            Text = baslik, Font = Tema.Baslik, ForeColor = Tema.Metin,
            Dock = DockStyle.Top, Height = 52
        };

        Controls.Add(lblAciklama);   // önce eklenen altta kalır
        Controls.Add(lblBaslik);
    }
}