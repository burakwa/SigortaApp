using SigortaApp.Forms;
using SigortaApp.Models;
using SigortaApp.Services;

namespace SigortaApp.UI;

public class MusteriListeSayfasi : UserControl
{
    private readonly MusteriService _servis = new();
    private readonly AramaKutusu _ara;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;
    private readonly System.Windows.Forms.Timer _aramaZamanlayici = new() { Interval = 300 };

    public MusteriListeSayfasi()
    {
        BackColor = Tema.Arkaplan;

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 86 };
        var lblAlt = new Label
        {
            Text = "Müşteri ve sigortalı kayıtlarını görüntüle, ekle ve düzenle",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Müşteriler",
            Font = Tema.Baslik,
            ForeColor = Tema.Metin,
            Dock = DockStyle.Top,
            Height = 50,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(lblAlt);      // önce eklenen altta kalır
        header.Controls.Add(lblBaslik);

        // --- Araç çubuğu ---
        var toolbar = new Panel { Dock = DockStyle.Top, Height = 66 };

        _ara = new AramaKutusu("TC, ad soyad veya telefon ara...")
        {
            Location = new Point(0, 8),
            Size = new Size(300, 42)
        };

        var butonlar = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 520,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };
        var btnYeni = new YuvarlakButon
        {
            Text = "+  Yeni Müşteri",
            Width = 150,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnDuzenle = new YuvarlakButon
        {
            Text = "Düzenle",
            Width = 100,
            Stil = ButonStili.Ikincil,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnGecmis = new YuvarlakButon
        {
            Text = "Geçmiş",
            Width = 100,
            Stil = ButonStili.Ikincil,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnSil = new YuvarlakButon
        {
            Text = "Sil",
            Width = 80,
            Stil = ButonStili.Tehlike,
            Margin = new Padding(10, 0, 0, 0)
        };
        butonlar.Controls.Add(btnYeni);
        butonlar.Controls.Add(btnDuzenle);
        butonlar.Controls.Add(btnGecmis);
        butonlar.Controls.Add(btnSil);

        toolbar.Controls.Add(butonlar);
        toolbar.Controls.Add(_ara);

        // --- Alt bilgi ---
        _lblSayi = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 32,
            AutoSize = false,
            ForeColor = Tema.MetinSoluk,
            Font = Tema.Kucuk,
            TextAlign = ContentAlignment.BottomLeft
        };

        // --- Kart + tablo ---
        var kart = new KartPanel { Dock = DockStyle.Fill };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        _grid.Columns.Add(Kolon("TcKimlikNo", "T.C. KİMLİK NO", 18));

        var adKolon = Kolon("AdSoyad", "AD SOYAD", 28);
        adKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(adKolon);

        _grid.Columns.Add(Kolon("Telefon", "TELEFON", 18));
        _grid.Columns.Add(Kolon("DogumTarihi", "DOĞUM TARİHİ", 16, "dd.MM.yyyy"));
        _grid.Columns.Add(Kolon("KayitTarihi", "KAYIT TARİHİ", 20, "dd.MM.yyyy HH:mm"));
        kart.Controls.Add(_grid);

        // Eklenme sırası önemli: Fill en önce, sonra Bottom, sonra Top'lar
        Controls.Add(kart);
        Controls.Add(_lblSayi);
        Controls.Add(toolbar);
        Controls.Add(header);

        Tema.Uygula(this);

        // --- Olaylar ---
        btnYeni.Click += async (_, _) => await YeniAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
        btnGecmis.Click += (_, _) => GecmisAc();
        btnSil.Click += async (_, _) => await SilAsync();
        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await DuzenleAsync();
        };

        // Arama: yazmayı bıraktıktan 300 ms sonra çalışır
        _ara.Kutu.TextChanged += (_, _) =>
        {
            _aramaZamanlayici.Stop();
            _aramaZamanlayici.Start();
        };
        _aramaZamanlayici.Tick += async (_, _) =>
        {
            _aramaZamanlayici.Stop();
            await YukleAsync();
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
        await YukleAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _aramaZamanlayici.Dispose();
        base.Dispose(disposing);
    }

    private async Task YukleAsync()
    {
        var liste = await _servis.AraAsync(_ara.Kutu.Text);
        _grid.DataSource = liste;
        _lblSayi.Text = $"{liste.Count} müşteri listeleniyor";
    }

    private Musteri? Secili() => _grid.CurrentRow?.DataBoundItem as Musteri;

    private async Task YeniAsync()
    {
        using var f = new MusteriFormu();
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var m = Secili();
        if (m == null) return;

        using var f = new MusteriFormu(m);
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private void GecmisAc()
    {
        var m = Secili();
        if (m == null) return;

        using var f = new MusteriGecmisiPenceresi(m.Id);
        f.ShowDialog(FindForm());
    }

    private async Task SilAsync()
    {
        var m = Secili();
        if (m == null) return;

        var onay = MessageBox.Show(
            $"{m.AdSoyad} adlı müşteri silinecek. Emin misin?\n(Geçmiş poliçe ve hasar kayıtları korunur.)",
            "Silme onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (onay != DialogResult.Yes) return;

        var sonuc = await _servis.SilAsync(m.Id);
        if (!sonuc.Basarili) MessageBox.Show(sonuc.Mesaj);
        await YukleAsync();
    }
}