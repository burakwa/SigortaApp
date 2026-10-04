using System.Drawing.Drawing2D;

namespace SigortaApp.UI;

public static class Tema
{
    // Renkler
    public static readonly Color Arkaplan = Color.FromArgb(243, 244, 246);
    public static readonly Color Kart = Color.White;
    public static readonly Color Sidebar = Color.FromArgb(15, 23, 42);
    public static readonly Color SidebarHover = Color.FromArgb(24, 34, 56);
    public static readonly Color SidebarAktif = Color.FromArgb(30, 41, 59);
    public static readonly Color SidebarMetin = Color.FromArgb(203, 213, 225);
    public static readonly Color SidebarSoluk = Color.FromArgb(148, 163, 184);
    public static readonly Color Birincil = Color.FromArgb(13, 148, 136);
    public static readonly Color BirincilHover = Color.FromArgb(15, 118, 110);
    public static readonly Color BirincilAcik = Color.FromArgb(94, 234, 212);
    public static readonly Color Secim = Color.FromArgb(240, 253, 250);
    public static readonly Color Tehlike = Color.FromArgb(220, 38, 38);
    public static readonly Color Basari = Color.FromArgb(22, 163, 74);
    public static readonly Color Metin = Color.FromArgb(17, 24, 39);
    public static readonly Color MetinSoluk = Color.FromArgb(107, 114, 128);
    public static readonly Color Cizgi = Color.FromArgb(229, 231, 235);

    // Fontlar
    public static readonly Font Normal = new("Segoe UI", 10f);
    public static readonly Font Kalin = new("Segoe UI Semibold", 10f);
    public static readonly Font Kucuk = new("Segoe UI", 9f);
    public static readonly Font KucukKalin = new("Segoe UI Semibold", 9f);
    public static readonly Font Baslik = new("Segoe UI Semibold", 20f);
    public static readonly Font Ikon = new("Segoe MDL2 Assets", 12f);

    // Yuvarlatılmış dikdörtgen yolu (kart, buton, menü için)
    public static GraphicsPath YuvarlakYol(Rectangle r, int yaricap)
    {
        int d = yaricap * 2;
        var yol = new GraphicsPath();
        yol.AddArc(r.X, r.Y, d, d, 180, 90);
        yol.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        yol.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        yol.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        yol.CloseFigure();
        return yol;
    }

    // Kontrolleri temaya uydurur. Kontroller eklendikten sonra çağır.
    public static void Uygula(Control kok)
    {
        foreach (Control c in kok.Controls)
        {
            if (c is AramaKutusu) continue;   // kendi stilini kendisi yönetir

            switch (c)
            {
                case MenuButonu or YuvarlakButon:
                    break;

                case Button b:
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.Cursor = Cursors.Hand;
                    b.Font = Kalin;
                    b.ForeColor = Color.White;
                    b.BackColor = (b.Tag as string) == "tehlike" ? Tehlike : Birincil;
                    break;

                case TextBox t:
                    t.BorderStyle = BorderStyle.FixedSingle;
                    if (!FontAyarlandiMi(t)) t.Font = Normal;
                    break;

                case DataGridView g:
                    g.BorderStyle = BorderStyle.None;
                    g.BackgroundColor = Kart;
                    g.EnableHeadersVisualStyles = false;
                    g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                    g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    g.ColumnHeadersHeight = 44;
                    g.ColumnHeadersDefaultCellStyle.BackColor = Kart;
                    g.ColumnHeadersDefaultCellStyle.ForeColor = MetinSoluk;
                    g.ColumnHeadersDefaultCellStyle.Font = KucukKalin;
                    g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Kart;
                    g.ColumnHeadersDefaultCellStyle.SelectionForeColor = MetinSoluk;
                    g.ColumnHeadersDefaultCellStyle.Padding = new Padding(14, 0, 0, 0);
                    g.DefaultCellStyle.Font = Normal;
                    g.DefaultCellStyle.ForeColor = Metin;
                    g.DefaultCellStyle.Padding = new Padding(14, 0, 0, 0);
                    g.DefaultCellStyle.SelectionBackColor = Secim;
                    g.DefaultCellStyle.SelectionForeColor = Metin;
                    g.RowTemplate.Height = 48;
                    g.RowHeadersVisible = false;
                    g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    g.GridColor = Color.FromArgb(243, 244, 246);
                    g.AllowUserToAddRows = false;
                    g.AllowUserToResizeRows = false;
                    g.ReadOnly = true;
                    g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    break;

                case Label l:
                    if (!FontAyarlandiMi(l)) l.Font = Normal;
                    break;
            }

            if (c.HasChildren) Uygula(c);
        }
    }

    private static bool FontAyarlandiMi(Control c) =>
        c.Font.Name.StartsWith("Segoe UI");
}