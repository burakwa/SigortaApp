using SigortaApp.Forms;
using SigortaApp.Models;
using SigortaApp.Services;

namespace SigortaApp.UI;

public class PoliceListeSayfasi : UserControl
{
    private readonly PoliceService _servis = new();
    private readonly AramaKutusu _ara;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;
    private readonly System.Windows.Forms.Timer _aramaZamanlayici = new() { Interval = 300 };

    public PoliceListeSayfasi()
    {
        BackColor = Tema.Arkaplan;

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 86 };
        var lblAlt = new Label
        {
            Text = "Poliçeleri görüntüle, yeni poliçe oluştur veya iptal et",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Poliçeler",
            Font = Tema.Baslik,
            ForeColor = Tema.Metin,
            Dock = DockStyle.Top,
            Height = 50,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(lblAlt);
        header.Controls.Add(lblBaslik);

        // --- Araç çubuğu ---
        var toolbar = new Panel { Dock = DockStyle.Top, Height = 66 };

        _ara = new AramaKutusu("Poliçe no, müşteri adı veya TC ara...")
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
            Text = "+  Yeni Poliçe",
            Width = 130,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnDuzenle = new YuvarlakButon
        {
            Text = "Düzenle",
            Width = 90,
            Stil = ButonStili.Ikincil,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnAile = new YuvarlakButon
        {
            Text = "Aile Bireyleri",
            Width = 140,
            Stil = ButonStili.Ikincil,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnIptalEt = new YuvarlakButon
        {
            Text = "İptal Et",
            Width = 90,
            Stil = ButonStili.Tehlike,
            Margin = new Padding(10, 0, 0, 0)
        };
        butonlar.Controls.Add(btnYeni);
        butonlar.Controls.Add(btnDuzenle);
        butonlar.Controls.Add(btnAile);
        butonlar.Controls.Add(btnIptalEt);

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

        var noKolon = Kolon("PoliceNo", "POLİÇE NO", 16);
        noKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(noKolon);
        _grid.Columns.Add(Kolon("MusteriAdSoyad", "MÜŞTERİ", 24));
        _grid.Columns.Add(Kolon("BaslangicTarihi", "BAŞLANGIÇ", 13, "dd.MM.yyyy"));
        _grid.Columns.Add(Kolon("BitisTarihi", "BİTİŞ", 13, "dd.MM.yyyy"));
        _grid.Columns.Add(Kolon("Prim", "PRİM (₺)", 14, "N2"));
        _grid.Columns.Add(Kolon("Durum", "DURUM", 14));
        kart.Controls.Add(_grid);

        Controls.Add(kart);
        Controls.Add(_lblSayi);
        Controls.Add(toolbar);
        Controls.Add(header);

        Tema.Uygula(this);

        // Durum sütununu renklendir
        _grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _grid.Columns[e.ColumnIndex].DataPropertyName != "Durum") return;
            var durum = e.Value as string;
            e.CellStyle.Font = Tema.Kalin;
            e.CellStyle.ForeColor = durum switch
            {
                "Aktif" => Tema.Basari,
                "İptal" => Tema.Tehlike,
                _ => Color.FromArgb(217, 119, 6)   // Süresi Dolmuş: amber
            };
        };

        // --- Olaylar ---
        btnYeni.Click += async (_, _) => await YeniAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
        btnAile.Click += (_, _) => AileBireyleriAc();
        btnIptalEt.Click += async (_, _) => await IptalEtAsync();
        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await DuzenleAsync();
        };

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
        _lblSayi.Text = $"{liste.Count} poliçe listeleniyor";
    }

    private PoliceSatiri? Secili() => _grid.CurrentRow?.DataBoundItem as PoliceSatiri;

    private void AileBireyleriAc()
    {
        var satir = Secili();
        if (satir == null) return;

        using var f = new AileBireyleriPenceresi(satir.Id, satir.PoliceNo, satir.MusteriAdSoyad);
        f.ShowDialog(FindForm());
    }

    private async Task YeniAsync()
    {
        using var f = new PoliceFormu();
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var police = await _servis.GetirAsync(satir.Id);
        if (police == null) return;

        using var f = new PoliceFormu(police);
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task IptalEtAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var onay = MessageBox.Show(
            $"{satir.PoliceNo} numaralı poliçe iptal edilecek. Emin misin?",
            "İptal onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (onay != DialogResult.Yes) return;

        var sonuc = await _servis.IptalEtAsync(satir.Id);
        if (!sonuc.Basarili) MessageBox.Show(sonuc.Mesaj);
        await YukleAsync();
    }
}