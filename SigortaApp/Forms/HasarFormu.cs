using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class HasarFormu : Form
{
    private readonly HasarService _servis = new();
    private readonly PoliceService _policeServis = new();
    private readonly Hasar? _mevcut;

    // Yeni kayıtta seçim kutusu, düzenlemede salt okunur metin kullanılır
    private readonly ComboBox _cmbPolice = new()
    {
        DropDownStyle = ComboBoxStyle.DropDown,
        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
        AutoCompleteSource = AutoCompleteSource.ListItems,
        DisplayMember = "Gorunum",
        ValueMember = "Id"
    };
    private readonly TextBox _txtPolice = new() { ReadOnly = true };
    private readonly Label _lblDonem = new()
    {
        Font = Tema.Kucuk,
        ForeColor = Tema.MetinSoluk,
        AutoSize = false,
        Width = 390,
        Height = 20
    };
    private readonly DateTimePicker _dtHasar = new()
    {
        Format = DateTimePickerFormat.Short,
        MaxDate = DateTime.Today,
        Value = DateTime.Today
    };
    private readonly NumericUpDown _numTutar = new()
    {
        DecimalPlaces = 2,
        Maximum = 100_000_000,
        Increment = 100,
        ThousandsSeparator = true
    };
    private readonly TextBox _txtAciklama = new() { Multiline = true, Height = 80, MaxLength = 500 };
    private readonly ComboBox _cmbDurum = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "Vazgeç", Width = 100, Height = 42, Stil = ButonStili.Ikincil };

    public HasarFormu(Hasar? mevcut = null, string? policeGorunum = null)
    {
        _mevcut = mevcut;

        Text = mevcut == null ? "Yeni Hasar" : "Hasarı Düzenle";
        ClientSize = new Size(440, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Kart;
        Font = Tema.Normal;

        // Sıra, HasarDurumu enum sırasıyla aynı olmalı: Beklemede, Onaylandi, Reddedildi, Odendi
        _cmbDurum.Items.AddRange(new object[] { "Beklemede", "Onaylandı", "Reddedildi", "Ödendi" });
        _cmbDurum.SelectedIndex = 0;

        var akis = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(24, 20, 24, 0)
        };

        akis.Controls.Add(new Label
        {
            Text = Text,
            Font = Tema.Baslik,
            ForeColor = Tema.Metin,
            AutoSize = false,
            Width = 390,
            Height = 52
        });

        if (mevcut == null)
        {
            Alan(akis, "Poliçe", _cmbPolice);
            akis.Controls.Add(_lblDonem);
        }
        else
        {
            _txtPolice.Text = policeGorunum ?? "";
            Alan(akis, "Poliçe", _txtPolice);
        }

        Alan(akis, "Hasar Tarihi", _dtHasar);
        Alan(akis, "Tutar (₺)", _numTutar);
        Alan(akis, "Açıklama", _txtAciklama);

        if (mevcut != null)
            Alan(akis, "Durum", _cmbDurum);

        _lblHata.Width = 390;
        akis.Controls.Add(_lblHata);

        var alt = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 66,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(24, 10, 24, 0)
        };
        alt.Controls.Add(_btnKaydet);
        alt.Controls.Add(_btnIptal);

        Controls.Add(akis);
        Controls.Add(alt);

        if (mevcut != null)
        {
            _dtHasar.Value = mevcut.HasarTarihi;
            _numTutar.Value = mevcut.Tutar;
            _txtAciklama.Text = mevcut.Aciklama;
            _cmbDurum.SelectedIndex = (int)mevcut.Durum;
        }

        // Poliçe seçilince dönem bilgisini göster
        _cmbPolice.SelectedIndexChanged += (_, _) =>
        {
            _lblDonem.Text = _cmbPolice.SelectedItem is PoliceSecenegi s
                ? $"Poliçe dönemi: {s.Baslangic:dd.MM.yyyy} – {s.Bitis:dd.MM.yyyy}"
                : "";
        };

        _btnKaydet.Click += Kaydet_Click;
        _btnIptal.Click += (_, _) => DialogResult = DialogResult.Cancel;
        AcceptButton = _btnKaydet;
        CancelButton = _btnIptal;

        Tema.Uygula(this);
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_mevcut != null) return;

        var secenekler = await _policeServis.HasarIcinSecenekleriGetirAsync();
        _cmbPolice.DataSource = secenekler;
        _cmbPolice.SelectedIndex = -1;

        if (secenekler.Count == 0)
        {
            _lblHata.Text = "Hasar eklemek için önce bir poliçe oluşturmalısın.";
            _btnKaydet.Enabled = false;
        }
    }

    private static void Alan(FlowLayoutPanel kap, string etiket, Control kontrol)
    {
        kap.Controls.Add(new Label
        {
            Text = etiket,
            ForeColor = Tema.MetinSoluk,
            Font = Tema.Kucuk,
            AutoSize = false,
            Width = 390,
            Height = 20
        });
        kontrol.Width = 390;
        kontrol.Margin = new Padding(3, 0, 3, 12);
        kap.Controls.Add(kontrol);
    }

    private async void Kaydet_Click(object? sender, EventArgs e)
    {
        _lblHata.Text = "";

        int policeId;
        if (_mevcut != null)
        {
            policeId = _mevcut.PoliceId;
        }
        else if (_cmbPolice.SelectedItem is PoliceSecenegi secili)
        {
            policeId = secili.Id;
        }
        else
        {
            _lblHata.Text = "Listeden bir poliçe seç.";
            return;
        }

        _btnKaydet.Enabled = false;

        var h = new Hasar
        {
            Id = _mevcut?.Id ?? 0,
            PoliceId = policeId,
            HasarTarihi = _dtHasar.Value.Date,
            Tutar = _numTutar.Value,
            Aciklama = _txtAciklama.Text,
            Durum = (HasarDurumu)_cmbDurum.SelectedIndex
        };

        var sonuc = _mevcut == null
            ? await _servis.EkleAsync(h)
            : await _servis.GuncelleAsync(h);

        if (sonuc.Basarili)
        {
            DialogResult = DialogResult.OK;
        }
        else
        {
            _lblHata.Text = sonuc.Mesaj;
            _btnKaydet.Enabled = true;
        }
    }
}