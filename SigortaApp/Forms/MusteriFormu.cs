using SigortaApp.Helpers;
using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class MusteriFormu : Form
{
    private readonly MusteriService _servis = new();
    private readonly Musteri? _mevcut;

    private readonly TextBox _txtTc = new() { MaxLength = 11 };
    private readonly Label _lblTc = new() { Font = Tema.Kucuk, AutoSize = true, Height = 18 };
    private readonly TextBox _txtAd = new();
    private readonly TextBox _txtSoyad = new();
    private readonly DateTimePicker _dtDogum = new()
    {
        Format = DateTimePickerFormat.Short,
        MaxDate = DateTime.Today,
        Value = new DateTime(1990, 1, 1)
    };
    private readonly TextBox _txtTelefon = new() { MaxLength = 15, PlaceholderText = "5XX XXX XX XX" };
    private readonly TextBox _txtEmail = new();
    private readonly TextBox _txtAdres = new() { Multiline = true, Height = 70 };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "İptal", Width = 100, Height = 42, Stil = ButonStili.Ikincil };



    public MusteriFormu(Musteri? mevcut = null)
    {
        _mevcut = mevcut;

        Text = mevcut == null ? "Yeni Müşteri" : "Müşteriyi Düzenle";
        ClientSize = new Size(440, 640);
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
            Height = 48
        });

        Alan(akis, "T.C. Kimlik No", _txtTc);
        akis.Controls.Add(_lblTc);
        Alan(akis, "Ad", _txtAd);
        Alan(akis, "Soyad", _txtSoyad);
        Alan(akis, "Doğum Tarihi", _dtDogum);
        Alan(akis, "Telefon", _txtTelefon);
        Alan(akis, "E-posta (isteğe bağlı)", _txtEmail);
        Alan(akis, "Adres (isteğe bağlı)", _txtAdres);

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

        // Mevcut müşteriyi forma doldur
        if (mevcut != null)
        {
            _txtTc.Text = mevcut.TcKimlikNo;
            _txtAd.Text = mevcut.Ad;
            _txtSoyad.Text = mevcut.Soyad;
            _dtDogum.Value = mevcut.DogumTarihi;
            _txtTelefon.Text = mevcut.Telefon;
            _txtEmail.Text = mevcut.Email;
            _txtAdres.Text = mevcut.Adres;
        }

        // Yalnızca rakam girişi + canlı TC kontrolü
        _txtTc.KeyPress += (_, e) =>
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        };
        _txtTc.TextChanged += (_, _) => TcKontrol();
        TcKontrol();

        _btnKaydet.Click += Kaydet_Click;
        _btnIptal.Click += (_, _) => DialogResult = DialogResult.Cancel;
        AcceptButton = _btnKaydet;
        CancelButton = _btnIptal;

        Tema.Uygula(this);

        // İptal butonu ikincil stil (Uygula mavi yaptığı için sonradan ezeriz)
        _btnIptal.BackColor = Tema.Arkaplan;
        _btnIptal.ForeColor = Tema.Metin;
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
        kontrol.Margin = new Padding(3, 0, 3, 10);
        kap.Controls.Add(kontrol);
    }

    private void TcKontrol()
    {
        var t = _txtTc.Text;
        if (t.Length == 0) { _lblTc.Text = ""; return; }

        if (t.Length < 11)
        {
            _lblTc.ForeColor = Tema.MetinSoluk;
            _lblTc.Text = $"{t.Length}/11";
        }
        else if (TcKimlikDogrulayici.Gecerlimi(t))
        {
            _lblTc.ForeColor = Tema.Basari;
            _lblTc.Text = "✓ Geçerli";
        }
        else
        {
            _lblTc.ForeColor = Tema.Tehlike;
            _lblTc.Text = "✕ Geçersiz T.C. Kimlik No";
        }
    }

    private async void Kaydet_Click(object? sender, EventArgs e)
    {
        _lblHata.Text = "";
        _btnKaydet.Enabled = false;

        var m = new Musteri
        {
            Id = _mevcut?.Id ?? 0,
            TcKimlikNo = _txtTc.Text.Trim(),
            Ad = _txtAd.Text,
            Soyad = _txtSoyad.Text,
            DogumTarihi = _dtDogum.Value.Date,
            Telefon = _txtTelefon.Text,
            Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text,
            Adres = string.IsNullOrWhiteSpace(_txtAdres.Text) ? null : _txtAdres.Text
        };

        var sonuc = _mevcut == null
            ? await _servis.EkleAsync(m)
            : await _servis.GuncelleAsync(m);

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