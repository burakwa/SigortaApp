using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class UrunTeminatiFormu : Form
{
    private readonly UrunService _servis = new();
    private readonly int _urunId;
    private readonly UrunTeminati? _mevcut;

    // Yeni kayıtta seçim kutusu, düzenlemede salt okunur metin kullanılır
    private readonly ComboBox _cmbTeminat = new()
    {
        DropDownStyle = ComboBoxStyle.DropDown,
        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
        AutoCompleteSource = AutoCompleteSource.ListItems,
        DisplayMember = "Ad",
        ValueMember = "Id"
    };
    private readonly TextBox _txtTeminat = new() { ReadOnly = true };
    private readonly CheckBox _chkLimitsiz = new() { Text = "Limitsiz", AutoSize = true, Margin = new Padding(3, 0, 3, 6) };
    private readonly NumericUpDown _numLimit = new()
    {
        DecimalPlaces = 2,
        Minimum = 0,
        Maximum = 100_000_000,
        Increment = 1000,
        ThousandsSeparator = true
    };
    private readonly NumericUpDown _numKatilim = new() { DecimalPlaces = 2, Minimum = 0, Maximum = 100, Increment = 5 };
    private readonly NumericUpDown _numBekleme = new() { Minimum = 0, Maximum = 3650 };
    private readonly TextBox _txtIstisna = new() { Multiline = true, Height = 80, MaxLength = 500 };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "Vazgeç", Width = 100, Height = 42, Stil = ButonStili.Ikincil };

    public UrunTeminatiFormu(int urunId, UrunTeminati? mevcut = null, string? teminatAdi = null)
    {
        _urunId = urunId;
        _mevcut = mevcut;

        Text = mevcut == null ? "Teminat Ekle" : "Teminat Koşullarını Düzenle";
        ClientSize = new Size(440, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Kart;
        Font = Tema.Normal;

        _chkLimitsiz.Font = Tema.Normal;
        _chkLimitsiz.ForeColor = Tema.Metin;

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
            Alan(akis, "Teminat", _cmbTeminat);
        }
        else
        {
            _txtTeminat.Text = teminatAdi ?? "";
            Alan(akis, "Teminat", _txtTeminat);
        }

        akis.Controls.Add(Etiket("Limit (₺)"));
        akis.Controls.Add(_chkLimitsiz);
        _numLimit.Width = 390;
        _numLimit.Margin = new Padding(3, 0, 3, 12);
        akis.Controls.Add(_numLimit);

        Alan(akis, "Katılım Payı (%)", _numKatilim);
        Alan(akis, "Bekleme Süresi (gün)", _numBekleme);
        Alan(akis, "İstisnalar (isteğe bağlı)", _txtIstisna);

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
            _chkLimitsiz.Checked = mevcut.Limit == null;
            _numLimit.Value = mevcut.Limit ?? 0;
            _numKatilim.Value = mevcut.KatilimPayiOrani;
            _numBekleme.Value = mevcut.BeklemeSuresiGun;
            _txtIstisna.Text = mevcut.Istisnalar;
        }
        _numLimit.Enabled = !_chkLimitsiz.Checked;
        _chkLimitsiz.CheckedChanged += (_, _) => _numLimit.Enabled = !_chkLimitsiz.Checked;

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

        var secenekler = await _servis.EklenebilirTeminatlarAsync(_urunId);
        _cmbTeminat.DataSource = secenekler;
        _cmbTeminat.SelectedIndex = -1;

        if (secenekler.Count == 0)
        {
            _lblHata.Text = "Eklenebilecek teminat kalmadı. Önce Teminatlar ekranından yeni teminat tanımla.";
            _btnKaydet.Enabled = false;
        }
    }

    private static Label Etiket(string metin) => new()
    {
        Text = metin,
        ForeColor = Tema.MetinSoluk,
        Font = Tema.Kucuk,
        AutoSize = false,
        Width = 390,
        Height = 20
    };

    private static void Alan(FlowLayoutPanel kap, string etiket, Control kontrol)
    {
        kap.Controls.Add(Etiket(etiket));
        kontrol.Width = 390;
        kontrol.Margin = new Padding(3, 0, 3, 12);
        kap.Controls.Add(kontrol);
    }

    private async void Kaydet_Click(object? sender, EventArgs e)
    {
        _lblHata.Text = "";

        int teminatId;
        if (_mevcut != null)
        {
            teminatId = _mevcut.TeminatId;
        }
        else if (_cmbTeminat.SelectedItem is Teminat secili)
        {
            teminatId = secili.Id;
        }
        else
        {
            _lblHata.Text = "Listeden bir teminat seç.";
            return;
        }

        _btnKaydet.Enabled = false;

        var ut = new UrunTeminati
        {
            Id = _mevcut?.Id ?? 0,
            UrunId = _urunId,
            TeminatId = teminatId,
            Limit = _chkLimitsiz.Checked ? null : _numLimit.Value,
            KatilimPayiOrani = _numKatilim.Value,
            BeklemeSuresiGun = (int)_numBekleme.Value,
            Istisnalar = _txtIstisna.Text
        };

        var sonuc = await _servis.TeminatKaydetAsync(ut);

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