using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class UrunTeminatlariPenceresi : Form
{
    private readonly UrunService _servis = new();
    private readonly int _urunId;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;

    public UrunTeminatlariPenceresi(int urunId, string urunAdi)
    {
        _urunId = urunId;

        Text = "Ürün Teminatları";
        ClientSize = new Size(1000, 600);
        MinimumSize = new Size(860, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Tema.Arkaplan;
        Font = Tema.Normal;
        Padding = new Padding(28);
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };

        // --- Başlık ---
        var header = new Panel { Dock = DockStyle.Top, Height = 84 };
        var lblAlt = new Label
        {
            Text = $"{urunAdi}  ·  Değişiklikler yalnızca yeni poliçeleri etkiler",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Ürün Teminatları",
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
        var toolbar = new Panel { Dock = DockStyle.Top, Height = 58 };

        _lblSayi = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Tema.MetinSoluk,
            Font = Tema.Kucuk,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var butonlar = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 380,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 6, 0, 0)
        };
        var btnEkle = new YuvarlakButon { Text = "+  Teminat Ekle", Width = 150, Margin = new Padding(10, 0, 0, 0) };
        var btnDuzenle = new YuvarlakButon { Text = "Düzenle", Width = 100, Stil = ButonStili.Ikincil, Margin = new Padding(10, 0, 0, 0) };
        var btnCikar = new YuvarlakButon { Text = "Çıkar", Width = 90, Stil = ButonStili.Tehlike, Margin = new Padding(10, 0, 0, 0) };
        butonlar.Controls.Add(btnEkle);
        butonlar.Controls.Add(btnDuzenle);
        butonlar.Controls.Add(btnCikar);

        toolbar.Controls.Add(_lblSayi);   // Fill önce
        toolbar.Controls.Add(butonlar);   // Right sonra

        // --- Kart + tablo ---
        var kart = new KartPanel { Dock = DockStyle.Fill };
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        var adKolon = Kolon("TeminatAdi", "TEMİNAT", 24);
        adKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(adKolon);
        _grid.Columns.Add(Kolon("Limit", "LİMİT", 16));
        _grid.Columns.Add(Kolon("KatilimPayi", "KATILIM PAYI", 12));
        _grid.Columns.Add(Kolon("BeklemeSuresi", "BEKLEME", 12));
        _grid.Columns.Add(Kolon("Istisnalar", "İSTİSNALAR", 36));
        kart.Controls.Add(_grid);

        Controls.Add(kart);
        Controls.Add(toolbar);
        Controls.Add(header);

        Tema.Uygula(this);

        btnEkle.Click += async (_, _) => await EkleAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
        btnCikar.Click += async (_, _) => await CikarAsync();
        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await DuzenleAsync();
        };
    }

    private static DataGridViewTextBoxColumn Kolon(string alan, string baslik, float agirlik)
        => new() { DataPropertyName = alan, HeaderText = baslik, FillWeight = agirlik };

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await YukleAsync();
    }

    private async Task YukleAsync()
    {
        var liste = await _servis.TeminatlariListeleAsync(_urunId);
        _grid.DataSource = liste;
        _lblSayi.Text = liste.Count == 0
            ? "Bu ürüne henüz teminat eklenmedi"
            : $"{liste.Count} teminat tanımlı";
    }

    private UrunTeminatSatiri? Secili() => _grid.CurrentRow?.DataBoundItem as UrunTeminatSatiri;

    private async Task EkleAsync()
    {
        using var f = new UrunTeminatiFormu(_urunId);
        if (f.ShowDialog(this) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var ut = await _servis.TeminatGetirAsync(satir.Id);
        if (ut == null) return;

        using var f = new UrunTeminatiFormu(_urunId, ut, satir.TeminatAdi);
        if (f.ShowDialog(this) == DialogResult.OK) await YukleAsync();
    }

    private async Task CikarAsync()
    {
        var satir = Secili();
        if (satir == null) return;

        var onay = MessageBox.Show(
            $"{satir.TeminatAdi} teminatı üründen çıkarılacak. Mevcut poliçeler etkilenmez. Emin misin?",
            "Çıkarma onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (onay != DialogResult.Yes) return;

        var sonuc = await _servis.TeminatCikarAsync(satir.Id);
        if (!sonuc.Basarili) MessageBox.Show(sonuc.Mesaj);
        await YukleAsync();
    }
}