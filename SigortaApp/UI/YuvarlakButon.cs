using System.Drawing.Drawing2D;

namespace SigortaApp.UI;

public enum ButonStili { Birincil, Ikincil, Tehlike }

public class YuvarlakButon : Button
{
    private ButonStili _stil = ButonStili.Birincil;
    private bool _ustunde, _basili;

    public ButonStili Stil
    {
        get => _stil;
        set { _stil = value; Invalidate(); }
    }

    public YuvarlakButon()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        Cursor = Cursors.Hand;
        Font = Tema.Kalin;
        Height = 42;
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnMouseEnter(EventArgs e) { _ustunde = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _ustunde = false; _basili = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _basili = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _basili = false; Invalidate(); base.OnMouseUp(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Tema.Arkaplan);

        Color zemin, yazi;
        Color? kenar = null;

        switch (_stil)
        {
            case ButonStili.Ikincil:
                zemin = _basili ? Color.FromArgb(229, 231, 235)
                      : _ustunde ? Color.FromArgb(249, 250, 251) : Tema.Kart;
                yazi = Tema.Metin;
                kenar = Color.FromArgb(209, 213, 219);
                break;

            case ButonStili.Tehlike:
                zemin = _basili ? Color.FromArgb(254, 202, 202)
                      : _ustunde ? Color.FromArgb(254, 226, 226) : Color.FromArgb(254, 242, 242);
                yazi = Tema.Tehlike;
                break;

            default:
                zemin = _basili || _ustunde ? Tema.BirincilHover : Tema.Birincil;
                yazi = Color.White;
                break;
        }

        if (!Enabled)
        {
            zemin = Color.FromArgb(229, 231, 235);
            yazi = Color.FromArgb(156, 163, 175);
            kenar = null;
        }

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var yol = Tema.YuvarlakYol(r, 8);
        using (var firca = new SolidBrush(zemin)) g.FillPath(firca, yol);
        if (kenar != null)
        {
            using var kalem = new Pen(kenar.Value);
            g.DrawPath(kalem, yol);
        }

        TextRenderer.DrawText(g, Text, Font, ClientRectangle, yazi,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
    }
}