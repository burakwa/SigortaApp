using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class PoliceFormu : Form
{
    private readonly PoliceService _servis = new();
    private readonly MusteriService _musteriServis = new();
    private readonly Police? _mevcut;

    private readonly ComboBox _cmbMusteri = new()
    {
        DropDownStyle = ComboBoxStyle.DropDown,
        AutoCompleteMode = AutoCompleteMode.SuggestAppend,
        AutoCompleteSource = AutoCompleteSource.ListItems,
        DisplayMember = "Gorunum",
        ValueMember = "Id"
    };
    private readonly TextBox _txtPoliceNo = new() { MaxLength = 30 };
    private readonly DateTimePicker _dtBaslangic = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
    private readonly DateTimePicker _dtBitis = new() { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(1) };
    private readonly NumericUpDown _numPrim = new()
    {
        DecimalPlaces = 2,
        Maximum = 10_000_000,
        Increment = 100,
        ThousandsSeparator = true
    };
    private readonly ComboBox _cmbDurum = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "Vazgeç", Width = 100, Height = 42, Stil = ButonStili.Ikincil };

    public PoliceFormu(Police? mevcut = null)
    {
        _mevcut = mevcut;

        Text = mevcut == null ? "Yeni Poliçe" : "Poliçeyi Düzenle";
        ClientSize = new Size(440, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Kart;
        Font = Tema.Normal;

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

        Alan(akis, "Müşteri", _cmbMusteri);
        Alan(akis, "Poliçe No", _txtPoliceNo);
        Alan(akis, "Başlangıç Tarihi", _dtBaslangic);
        Alan(akis, "Bitiş Tarihi", _dtBitis);
        Alan(akis, "Prim (₺)", _numPrim);

        if (mevcut != null)
        {
            _cmbDurum.Items.AddRange(new object[] { "Aktif", "İptal" });
            _cmbDurum.SelectedIndex = mevcut.Durum == PoliceDurumu.Iptal ? 1 : 0;
            Alan(akis, "Durum", _cmbDurum);
        }

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

        // Mevcut poliçeyi forma doldur
        if (mevcut != null)
        {
            _txtPoliceNo.Text = mevcut.PoliceNo;
            _dtBaslangic.Value = mevcut.BaslangicTarihi;
            _dtBitis.Value = mevcut.BitisTarihi;
            _numPrim.Value = mevcut.Prim;
        }

        // Yeni poliçede başlangıç değişince bitiş otomatik +1 yıl olsun
        _dtBaslangic.ValueChanged += (_, _) =>
        {
            if (_mevcut == null) _dtBitis.Value = _dtBaslangic.Value.AddYears(1);
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

        _cmbMusteri.DataSource = await _musteriServis.ListeleAsync();
        _cmbMusteri.SelectedIndex = -1;

        if (_mevcut != null)
        {
            _cmbMusteri.SelectedValue = _mevcut.MusteriId;
            _cmbMusteri.Enabled = false;   // poliçenin müşterisi değişmez
        }
        else
        {
            _txtPoliceNo.Text = await _servis.YeniPoliceNoAsync();
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

        if (_cmbMusteri.SelectedItem is not Musteri secili)
        {
            _lblHata.Text = "Listeden bir müşteri seç.";
            return;
        }

        _btnKaydet.Enabled = false;

        var p = new Police
        {
            Id = _mevcut?.Id ?? 0,
            MusteriId = secili.Id,
            PoliceNo = _txtPoliceNo.Text,
            BaslangicTarihi = _dtBaslangic.Value.Date,
            BitisTarihi = _dtBitis.Value.Date,
            Prim = _numPrim.Value,
            Durum = _cmbDurum.SelectedIndex == 1 ? PoliceDurumu.Iptal : PoliceDurumu.Bagli
        };

        var sonuc = _mevcut == null
            ? await _servis.EkleAsync(p)
            : await _servis.GuncelleAsync(p);

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