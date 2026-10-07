using SigortaApp.Forms;
using SigortaApp.Models;
using SigortaApp.Services;

namespace SigortaApp.UI;

public class UrunListeSayfasi : UserControl
{
    private readonly UrunService _servis = new();
    private readonly AramaKutusu _ara;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;
    private readonly System.Windows.Forms.Timer _aramaZamanlayici = new() { Interval = 300 };

    public UrunListeSayfasi()
    {
        BackColor = Tema.Arkaplan;

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 86 };
        var lblAlt = new Label
        {
            Text = "Sigorta ürünlerini ve teminat koşullarını yönet",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Ürünler",
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

        _ara = new AramaKutusu("Kod veya ürün adı ara...")
        {
            Location = new Point(0, 8),
            Size = new Size(280, 42)
        };

        var butonlar = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 540,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };
        var btnYeni = new YuvarlakButon { Text = "+  Yeni Ürün", Width = 120, Margin = new Padding(10, 0, 0, 0) };
        var btnDuzenle = new YuvarlakButon { Text = "Düzenle", Width = 90, Stil = ButonStili.Ikincil, Margin = new Padding(10, 0, 0, 0) };
        var btnTeminat = new YuvarlakButon { Text = "Teminatlar", Width = 120, Stil = ButonStili.Ikincil, Margin = new Padding(10, 0, 0, 0) };
        var btnDurum = new YuvarlakButon { Text = "Aktif / Pasif", Width = 130, Stil = ButonStili.Ikincil, Margin = new Padding(10, 0, 0, 0) };
        butonlar.Controls.Add(btnYeni);
        butonlar.Controls.Add(btnDuzenle);
        butonlar.Controls.Add(btnTeminat);
        butonlar.Controls.Add(btnDurum);

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
        _grid.Columns.Add(Kolon("Kod", "KOD", 14));

        var adKolon = Kolon("Ad", "ÜRÜN ADI", 30);
        adKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(adKolon);

        _grid.Columns.Add(Kolon("Tur", "TÜR", 24));
        _grid.Columns.Add(Kolon("TeminatSayisi", "TEMİNAT SAYISI", 16));
        _grid.Columns.Add(Kolon("Durum", "DURUM", 16));
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
            e.CellStyle.Font = Tema.Kalin;
            e.CellStyle.ForeColor = (e.Value as string) == "Aktif" ? Tema.Basari : Tema.MetinSoluk;
        };

        btnYeni.Click += async (_, _) => await YeniAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
        btnTeminat.Click += async (_, _) => await TeminatlariAcAsync();
        btnDurum.Click += async (_, _) => await DurumDegistirAsync();
        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await TeminatlariAcAsync();
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

    private static DataGridViewTextBoxColumn Kolon(string alan, string baslik, float agirlik)
        => new() { DataPropertyName = alan, HeaderText = baslik, FillWeight = agirlik };

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
        _lblSayi.Text = $"{liste.Count} ürün listeleniyor";
    }

    private UrunSatiri? Secili() => _grid.CurrentRow?.DataBoundItem as UrunSatiri;

    private async Task YeniAsync()
    {
        using var f = new UrunFormu();
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var urun = await _servis.GetirAsync(satir.Id);
        if (urun == null) return;

        using var f = new UrunFormu(urun);
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task TeminatlariAcAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        using var f = new UrunTeminatlariPenceresi(satir.Id, satir.Ad);
        f.ShowDialog(FindForm());
        await YukleAsync();   // teminat sayısı değişmiş olabilir
    }

    private async Task DurumDegistirAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var sonuc = await _servis.DurumDegistirAsync(satir.Id);
        if (!sonuc.Basarili) MessageBox.Show(sonuc.Mesaj);
        await YukleAsync();
    }
}