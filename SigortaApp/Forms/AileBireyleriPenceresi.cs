using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class AileBireyleriPenceresi : Form
{
    private readonly AileBireyiService _servis = new();
    private readonly int _policeId;
    private readonly DataGridView _grid;
    private readonly Label _lblSayi;

    public AileBireyleriPenceresi(int policeId, string policeNo, string musteriAdSoyad)
    {
        _policeId = policeId;

        Text = "Aile Bireyleri";
        ClientSize = new Size(860, 560);
        MinimumSize = new Size(760, 480);
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
            Text = $"{policeNo}  ·  {musteriAdSoyad}",
            Font = Tema.Normal,
            ForeColor = Tema.MetinSoluk,
            Dock = DockStyle.Top,
            Height = 28,
            AutoSize = false
        };
        var lblBaslik = new Label
        {
            Text = "Aile Bireyleri",
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
            Width = 340,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 6, 0, 0)
        };
        var btnEkle = new YuvarlakButon { Text = "+  Ekle", Width = 100, Margin = new Padding(10, 0, 0, 0) };
        var btnDuzenle = new YuvarlakButon { Text = "Düzenle", Width = 100, Stil = ButonStili.Ikincil, Margin = new Padding(10, 0, 0, 0) };
        var btnSil = new YuvarlakButon { Text = "Sil", Width = 80, Stil = ButonStili.Tehlike, Margin = new Padding(10, 0, 0, 0) };
        butonlar.Controls.Add(btnEkle);
        butonlar.Controls.Add(btnDuzenle);
        butonlar.Controls.Add(btnSil);

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
        _grid.Columns.Add(Kolon("YakinlikMetni", "YAKINLIK", 14));

        var adKolon = Kolon("AdSoyad", "AD SOYAD", 28);
        adKolon.DefaultCellStyle.Font = Tema.Kalin;
        _grid.Columns.Add(adKolon);

        _grid.Columns.Add(Kolon("TcKimlikNo", "T.C. KİMLİK NO", 20));
        _grid.Columns.Add(Kolon("DogumTarihi", "DOĞUM TARİHİ", 16, "dd.MM.yyyy"));
        _grid.Columns.Add(Kolon("Yas", "YAŞ", 8));
        kart.Controls.Add(_grid);

        Controls.Add(kart);
        Controls.Add(toolbar);
        Controls.Add(header);

        Tema.Uygula(this);

        btnEkle.Click += async (_, _) => await EkleAsync();
        btnDuzenle.Click += async (_, _) => await DuzenleAsync();
        btnSil.Click += async (_, _) => await SilAsync();
        _grid.CellDoubleClick += async (_, e) =>
        {
            if (e.RowIndex >= 0) await DuzenleAsync();
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

    private async Task YukleAsync()
    {
        var liste = await _servis.ListeleAsync(_policeId);
        _grid.DataSource = liste;
        _lblSayi.Text = liste.Count == 0
            ? "Bu poliçede henüz aile bireyi yok"
            : $"{liste.Count} aile bireyi";
    }

    private AileBireyi? Secili() => _grid.CurrentRow?.DataBoundItem as AileBireyi;

    private async Task EkleAsync()
    {
        using var f = new AileBireyiFormu(_policeId);
        if (f.ShowDialog(this) == DialogResult.OK) await YukleAsync();
    }

    private async Task DuzenleAsync()
    {
        var a = Secili();
        if (a == null) return;

        using var f = new AileBireyiFormu(_policeId, a);
        if (f.ShowDialog(this) == DialogResult.OK) await YukleAsync();
    }

    private async Task SilAsync()
    {
        var a = Secili();
        if (a == null) return;

        var onay = MessageBox.Show(
            $"{a.AdSoyad} ({a.YakinlikMetni}) poliçeden çıkarılacak. Emin misin?",
            "Silme onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (onay != DialogResult.Yes) return;

        var sonuc = await _servis.SilAsync(a.Id);
        if (!sonuc.Basarili) MessageBox.Show(sonuc.Mesaj);
        await YukleAsync();
    }
}