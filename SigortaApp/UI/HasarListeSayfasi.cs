using SigortaApp.Forms;
using SigortaApp.Models;
using SigortaApp.Services;

namespace SigortaApp.UI;

public class HasarListeSayfasi : UserControl
{
    private readonly HasarService _servis = new();
    private readonly AramaKutusu _ara;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;
    private readonly System.Windows.Forms.Timer _aramaZamanlayici = new() { Interval = 300 };

    public HasarListeSayfasi()
    {
        BackColor = Tema.Arkaplan;

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 86 };
        var lblAlt = new Label
        {
            Text = "Hasar kayıtlarını görüntüle, yeni hasar gir ve durumunu güncelle",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Hasarlar",
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

        _ara = new AramaKutusu("Poliçe no, müşteri veya açıklama ara...")
        {
            Location = new Point(0, 8),
            Size = new Size(380, 42)
        };

        var butonlar = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 320,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };
        var btnYeni = new YuvarlakButon
        {
            Text = "+  Yeni Hasar",
            Width = 140,
            Margin = new Padding(10, 0, 0, 0)
        };
        var btnDuzenle = new YuvarlakButon
        {
            Text = "Düzenle",
            Width = 100,
            Stil = ButonStili.Ikincil,
            Margin = new Padding(10, 0, 0, 0)
        };
        butonlar.Controls.Add(btnYeni);
        butonlar.Controls.Add(btnDuzenle);

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
        _grid.Columns.Add(Kolon("HasarTarihi", "HASAR TARİHİ", 13, "dd.MM.yyyy"));

        var noKolon = Kolon("PoliceNo", "POLİÇE NO", 15);
        noKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(noKolon);

        _grid.Columns.Add(Kolon("MusteriAdSoyad", "MÜŞTERİ", 20));
        _grid.Columns.Add(Kolon("Aciklama", "AÇIKLAMA", 27));
        _grid.Columns.Add(Kolon("Tutar", "TUTAR (₺)", 12, "N2"));
        _grid.Columns.Add(Kolon("Durum", "DURUM", 13));
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
            e.CellStyle.ForeColor = (e.Value as string) switch
            {
                "Ödendi" => Tema.Basari,
                "Onaylandı" => Tema.Birincil,
                "Reddedildi" => Tema.Tehlike,
                _ => Color.FromArgb(217, 119, 6)   // Beklemede: amber
            };
        };

        // --- Olaylar ---
        btnYeni.Click += async (_, _) => await YeniAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
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
        _lblSayi.Text = $"{liste.Count} hasar kaydı listeleniyor";
    }

    private HasarSatiri? Secili() => _grid.CurrentRow?.DataBoundItem as HasarSatiri;

    private async Task YeniAsync()
    {
        using var f = new HasarFormu();
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        if (satir.DurumKodu == HasarDurumu.Odendi)
        {
            MessageBox.Show("Ödenmiş hasar kaydı değiştirilemez.");
            return;
        }

        var hasar = await _servis.GetirAsync(satir.Id);
        if (hasar == null) return;

        using var f = new HasarFormu(hasar, $"{satir.PoliceNo}  ·  {satir.MusteriAdSoyad}");
        if (f.ShowDialog(FindForm()) == DialogResult.OK) await YukleAsync();
    }
}