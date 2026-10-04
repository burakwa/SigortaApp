using System.Drawing.Drawing2D;

namespace SigortaApp.UI;

// Yuvarlatılmış köşeli, ince çerçeveli beyaz kart
public class KartPanel : Panel
{
    public int Yaricap { get; set; } = 12;

    public KartPanel()
    {
        DoubleBuffered = true;
        BackColor = Tema.Kart;
        Padding = new Padding(6);
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Köşelerde arkadaki sayfa rengi görünsün
        e.Graphics.Clear(Parent?.BackColor ?? Tema.Arkaplan);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using var yol = Tema.YuvarlakYol(r, Yaricap);
        using var firca = new SolidBrush(Tema.Kart);
        using var kalem = new Pen(Tema.Cizgi);
        g.FillPath(firca, yol);
        g.DrawPath(kalem, yol);
    }
}