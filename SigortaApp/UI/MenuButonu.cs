using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SigortaApp.UI;

public class MenuButonu : Button
{
    private bool _aktif, _ustunde;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Ikon { get; set; } = "";

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Aktif
    {
        get => _aktif;
        set { _aktif = value; Invalidate(); }
    }

    public MenuButonu()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Height = 48;
        Font = Tema.Normal;
        Cursor = Cursors.Hand;
        Margin = new Padding(12, 2, 12, 2);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnMouseEnter(EventArgs e) { _ustunde = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _ustunde = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Tema.Sidebar);

        Color zemin = _aktif ? Tema.SidebarAktif : _ustunde ? Tema.SidebarHover : Tema.Sidebar;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var yol = Tema.YuvarlakYol(r, 10))
        using (var firca = new SolidBrush(zemin))
            g.FillPath(firca, yol);

        // Aktif sayfa: sol vurgu çizgisi
        if (_aktif)
        {
            using var cizgi = new SolidBrush(Tema.BirincilAcik);
            g.FillRectangle(cizgi, 0, 13, 4, Height - 26);
        }

        var ikonRenk = _aktif ? Tema.BirincilAcik : Tema.SidebarSoluk;
        var yaziRenk = _aktif ? Color.White : Tema.SidebarMetin;

        TextRenderer.DrawText(g, Ikon, Tema.Ikon, new Rectangle(16, 0, 32, Height), ikonRenk,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        TextRenderer.DrawText(g, Text, Font, new Rectangle(56, 0, Width - 60, Height), yaziRenk,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
    }
}