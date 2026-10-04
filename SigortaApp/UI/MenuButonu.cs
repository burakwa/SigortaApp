namespace SigortaApp.UI;

public class MenuButonu : Button
{
    private bool _aktif;

    public bool Aktif
    {
        get => _aktif;
        set { _aktif = value; Yenile(); }
    }

    public MenuButonu()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Height = 46;
        TextAlign = ContentAlignment.MiddleLeft;
        Padding = new Padding(22, 0, 0, 0);
        Font = Tema.Normal;
        ForeColor = Color.White;
        Cursor = Cursors.Hand;
        Margin = Padding.Empty;
        Yenile();
    }

    private void Yenile()
    {
        BackColor = _aktif ? Tema.Birincil : Tema.Sidebar;
        FlatAppearance.MouseOverBackColor = _aktif ? Tema.Birincil : Tema.SidebarHover;
        FlatAppearance.MouseDownBackColor = _aktif ? Tema.BirincilHover : Tema.SidebarHover;
    }
}