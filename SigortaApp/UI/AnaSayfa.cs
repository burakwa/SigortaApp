using System.Globalization;
using SigortaApp.Models;
using SigortaApp.Services;

namespace SigortaApp.UI;

public class AnaSayfa : UserControl
{
    private readonly DashboardService _servis = new();

    private readonly OzetKarti _kartMusteri = new("MÜŞTERİ");
    private readonly OzetKarti _kartPolice = new("AKTİF POLİÇE");
    private readonly OzetKarti _kartHasar = new("BEKLEYEN HASAR");
    private readonly OzetKarti _kartYenileme = new("YAKLAŞAN YENİLEME");

    private readonly Label _lblTablo = new()
    {
        Text = "Yenileme Takibi",
        Font = new Font("Segoe UI Semibold", 13f),
        ForeColor = Tema.Metin,
        Dock = DockStyle.Top,
        Height = 40,
        AutoSize = false,
        TextAlign = ContentAlignment.MiddleLeft
    };
    private readonly Label _lblBilgi = new()
    {
        Dock = DockStyle.Bottom,
        Height = 32,
        AutoSize = false,
        ForeColor = Tema.MetinSoluk,
        Font = Tema.Kucuk,
        TextAlign = ContentAlignment.BottomLeft
    };
    private readonly DataGridView _grid;

    public AnaSayfa()
    {
        BackColor = Tema.Arkaplan;

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 86 };
        var lblAlt = new Label
        {
            Text = DateTime.Today.ToString("d MMMM yyyy, dddd", new CultureInfo("tr-TR")),
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Ana Sayfa",
            Font = Tema.Baslik,
            ForeColor = Tema.Metin,
            Dock = DockStyle.Top,
            Height = 50,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(lblAlt);      // önce eklenen altta kalır
        header.Controls.Add(lblBaslik);

        // --- Özet kartları ---
        var kartlar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 132,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(0, 0, 0, 16)
        };
        for (int i = 0; i < 4; i++)
            kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        kartlar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        foreach (var k in new[] { _kartMusteri, _kartPolice, _kartHasar, _kartYenileme })
        {
            k.Dock = DockStyle.Fill;
            k.Margin = new Padding(0, 0, 12, 0);
            kartlar.Controls.Add(k);
        }
        _kartYenileme.Margin = Padding.Empty;

        // --- Tablo ---
        var kart = new KartPanel { Dock = DockStyle.Fill };
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        _grid.Columns.Add(Kolon("Kalan", "KALAN", 14));

        var noKolon = Kolon("PoliceNo", "POLİÇE NO", 18);
        noKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(noKolon);

        _grid.Columns.Add(Kolon("MusteriAdSoyad", "MÜŞTERİ", 26));
        _grid.Columns.Add(Kolon("Telefon", "TELEFON", 16));
        _grid.Columns.Add(Kolon("BitisTarihi", "BİTİŞ", 12, "dd.MM.yyyy"));
        _grid.Columns.Add(Kolon("Prim", "PRİM (₺)", 14, "N2"));
        kart.Controls.Add(_grid);

        // Eklenme sırası önemli: Fill en önce, sonra Bottom, sonra Top'lar
        Controls.Add(kart);
        Controls.Add(_lblBilgi);
        Controls.Add(_lblTablo);
        Controls.Add(kartlar);
        Controls.Add(header);

        Tema.Uygula(this);

        // Kalan gün sütununu renklendir
        _grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _grid.Columns[e.ColumnIndex].DataPropertyName != "Kalan") return;
            if (_grid.Rows[e.RowIndex].DataBoundItem is not YenilemeSatiri s) return;

            e.CellStyle.Font = Tema.Kalin;
            e.CellStyle.ForeColor = s.KalanGun < 0 ? Tema.Tehlike
                                  : s.KalanGun <= 7 ? Color.FromArgb(217, 119, 6)
                                  : Tema.Metin;
        };
    }

    private static DataGridViewTextBoxColumn Kolon(string alan, string baslik, float agirlik, string? bicim = null)
    {
        var k = new DataGridViewTextBoxColumn
        {
            DataPropertyName = alan,
            HeaderText = baslik,
            FillWeight = agirlik
        };
        if (bicim != null) k.DefaultCellStyle.Format = bicim;
        return k;
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var o = await _servis.GetirAsync();

        _kartMusteri.Deger = o.ToplamMusteri.ToString();
        _kartMusteri.Alt = "kayıtlı müşteri";

        _kartPolice.Deger = o.BagliPolice.ToString();
        _kartPolice.Alt = "bağlı, vadesi gelmemiş";

        _kartHasar.Deger = o.BekleyenHasar.ToString();
        _kartHasar.Alt = "onay bekliyor";

        _kartYenileme.Deger = o.YaklasanYenileme.ToString();
        _kartYenileme.Alt = $"{o.VadesiGecen} poliçenin vadesi geçmiş";

        _grid.DataSource = o.Yenilemeler;
        _lblBilgi.Text = o.Yenilemeler.Count == 0
            ? $"{o.HatirlatmaGunu} gün içinde biten ya da vadesi geçmiş poliçe yok"
            : $"Önümüzdeki {o.HatirlatmaGunu} gün içinde biten ve vadesi geçmiş poliçeler listeleniyor";
    }
}