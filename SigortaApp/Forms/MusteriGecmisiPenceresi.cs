using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class MusteriGecmisiPenceresi : Form
{
    private readonly MusteriGecmisService _servis = new();
    private readonly int _musteriId;

    private readonly Label _lblBaslik = new()
    {
        Text = "Müşteri Geçmişi",
        Font = Tema.Baslik,
        ForeColor = Tema.Metin,
        Dock = DockStyle.Top,
        Height = 50,
        AutoSize = false,
        TextAlign = ContentAlignment.MiddleLeft
    };
    private readonly Label _lblAlt = new()
    {
        Font = Tema.Normal,
        ForeColor = Tema.MetinSoluk,
        Dock = DockStyle.Top,
        Height = 28,
        AutoSize = false
    };

    private readonly OzetKarti _kartPolice = new("TOPLAM POLİÇE");
    private readonly OzetKarti _kartPrim = new("TOPLAM PRİM");
    private readonly OzetKarti _kartHasar = new("HASAR KAYDI");
    private readonly OzetKarti _kartOdenen = new("ÖDENEN HASAR");

    private readonly YuvarlakButon _btnPolice = new() { Text = "Poliçeler", Width = 160, Margin = new Padding(0, 0, 10, 0) };
    private readonly YuvarlakButon _btnHasar = new() { Text = "Hasarlar", Width = 160, Margin = new Padding(0, 0, 10, 0) };

    private readonly DataGridView _gridPolice;
    private readonly DataGridView _gridHasar;
    private readonly Label _lblBilgi = new()
    {
        Dock = DockStyle.Bottom,
        Height = 32,
        AutoSize = false,
        ForeColor = Tema.MetinSoluk,
        Font = Tema.Kucuk,
        TextAlign = ContentAlignment.BottomLeft
    };

    public MusteriGecmisiPenceresi(int musteriId)
    {
        _musteriId = musteriId;

        Text = "Müşteri Geçmişi";
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Tema.Arkaplan;
        Font = Tema.Normal;
        Padding = new Padding(28);
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 84 };
        header.Controls.Add(_lblAlt);      // önce eklenen altta kalır
        header.Controls.Add(_lblBaslik);

        // --- Özet kartları ---
        var kartlar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 118,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(0, 0, 0, 14)
        };
        for (int i = 0; i < 4; i++)
            kartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        kartlar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        foreach (var k in new[] { _kartPolice, _kartPrim, _kartHasar, _kartOdenen })
        {
            k.Dock = DockStyle.Fill;
            k.Margin = new Padding(0, 0, 12, 0);
            kartlar.Controls.Add(k);
        }
        _kartOdenen.Margin = Padding.Empty;

        // --- Sekme düğmeleri ---
        var sekmeler = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 56,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 4, 0, 0)
        };
        sekmeler.Controls.Add(_btnPolice);
        sekmeler.Controls.Add(_btnHasar);

        // --- Kart + iki tablo (aynı yerde, biri görünür) ---
        var kart = new KartPanel { Dock = DockStyle.Fill };

        _gridPolice = GridOlustur();
        var noKolon = Kolon("PoliceNo", "POLİÇE NO", 20);
        noKolon.DefaultCellStyle.Font = Tema.Kalin;
        _gridPolice.Columns.Add(noKolon);
        _gridPolice.Columns.Add(Kolon("BaslangicTarihi", "BAŞLANGIÇ", 18, "dd.MM.yyyy"));
        _gridPolice.Columns.Add(Kolon("BitisTarihi", "BİTİŞ", 18, "dd.MM.yyyy"));
        _gridPolice.Columns.Add(Kolon("Prim", "PRİM (₺)", 22, "N2"));
        _gridPolice.Columns.Add(Kolon("Durum", "DURUM", 22));

        _gridHasar = GridOlustur();
        _gridHasar.Columns.Add(Kolon("HasarTarihi", "HASAR TARİHİ", 16, "dd.MM.yyyy"));
        var hasarNo = Kolon("PoliceNo", "POLİÇE NO", 18);
        hasarNo.DefaultCellStyle.Font = Tema.Kalin;
        _gridHasar.Columns.Add(hasarNo);
        _gridHasar.Columns.Add(Kolon("Aciklama", "AÇIKLAMA", 34));
        _gridHasar.Columns.Add(Kolon("Tutar", "TUTAR (₺)", 16, "N2"));
        _gridHasar.Columns.Add(Kolon("Durum", "DURUM", 16));

        kart.Controls.Add(_gridPolice);
        kart.Controls.Add(_gridHasar);

        // Eklenme sırası önemli: Fill en önce, sonra Bottom, sonra Top'lar
        Controls.Add(kart);
        Controls.Add(_lblBilgi);
        Controls.Add(sekmeler);
        Controls.Add(kartlar);
        Controls.Add(header);

        Tema.Uygula(this);

        // Durum sütunlarını renklendir
        _gridPolice.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _gridPolice.Columns[e.ColumnIndex].DataPropertyName != "Durum") return;
            e.CellStyle.Font = Tema.Kalin;
            e.CellStyle.ForeColor = (e.Value as string) switch
            {
                "Aktif" => Tema.Basari,
                "İptal" => Tema.Tehlike,
                _ => Color.FromArgb(217, 119, 6)   // Süresi Dolmuş
            };
        };
        _gridHasar.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _gridHasar.Columns[e.ColumnIndex].DataPropertyName != "Durum") return;
            e.CellStyle.Font = Tema.Kalin;
            e.CellStyle.ForeColor = (e.Value as string) switch
            {
                "Ödendi" => Tema.Basari,
                "Onaylandı" => Tema.Birincil,
                "Reddedildi" => Tema.Tehlike,
                _ => Color.FromArgb(217, 119, 6)   // Beklemede
            };
        };

        _btnPolice.Click += (_, _) => Sekme(0);
        _btnHasar.Click += (_, _) => Sekme(1);
        Sekme(0);
    }

    private static DataGridView GridOlustur() => new()
    {
        Dock = DockStyle.Fill,
        AutoGenerateColumns = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

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

    private MusteriGecmisi? _gecmis;

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _gecmis = await _servis.GetirAsync(_musteriId);
        if (_gecmis == null)
        {
            MessageBox.Show("Müşteri bulunamadı.");
            Close();
            return;
        }

        var m = _gecmis.Musteri;
        _lblBaslik.Text = m.AdSoyad;
        _lblAlt.Text = $"Müşteri geçmişi  ·  T.C. {m.TcKimlikNo}  ·  {m.Telefon}  ·  Kayıt: {m.KayitTarihi:dd.MM.yyyy}";

        _kartPolice.Deger = _gecmis.Policeler.Count.ToString();
        _kartPolice.Alt = $"{_gecmis.AktifPoliceSayisi} aktif";

        _kartPrim.Deger = $"{_gecmis.ToplamPrim:N2} ₺";
        _kartPrim.Alt = "iptal edilenler hariç";

        _kartHasar.Deger = _gecmis.Hasarlar.Count.ToString();
        _kartHasar.Alt = $"{_gecmis.BekleyenHasarSayisi} beklemede";

        _kartOdenen.Deger = $"{_gecmis.OdenenHasarTutari:N2} ₺";
        _kartOdenen.Alt = $"{_gecmis.OdenenHasarSayisi} ödenen kayıt";

        _btnPolice.Text = $"Poliçeler ({_gecmis.Policeler.Count})";
        _btnHasar.Text = $"Hasarlar ({_gecmis.Hasarlar.Count})";

        _gridPolice.DataSource = _gecmis.Policeler;
        _gridHasar.DataSource = _gecmis.Hasarlar;
        Sekme(0);
    }

    private void Sekme(int sira)
    {
        _gridPolice.Visible = sira == 0;
        _gridHasar.Visible = sira == 1;
        _btnPolice.Stil = sira == 0 ? ButonStili.Birincil : ButonStili.Ikincil;
        _btnHasar.Stil = sira == 1 ? ButonStili.Birincil : ButonStili.Ikincil;

        if (_gecmis == null) { _lblBilgi.Text = ""; return; }

        _lblBilgi.Text = sira == 0
            ? (_gecmis.Policeler.Count == 0 ? "Bu müşterinin poliçesi bulunmuyor" : $"{_gecmis.Policeler.Count} poliçe")
            : (_gecmis.Hasarlar.Count == 0 ? "Bu müşterinin hasar kaydı bulunmuyor" : $"{_gecmis.Hasarlar.Count} hasar kaydı");
    }
}