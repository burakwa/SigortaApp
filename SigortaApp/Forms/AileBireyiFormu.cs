using SigortaApp.Helpers;
using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class AileBireyiFormu : Form
{
    private readonly AileBireyiService _servis = new();
    private readonly int _policeId;
    private readonly AileBireyi? _mevcut;

    private readonly ComboBox _cmbYakinlik = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtTc = new() { MaxLength = 11 };
    private readonly Label _lblTc = new() { Font = Tema.Kucuk, AutoSize = true, Height = 18 };
    private readonly TextBox _txtAd = new();
    private readonly TextBox _txtSoyad = new();
    private readonly DateTimePicker _dtDogum = new()
    {
        Format = DateTimePickerFormat.Short,
        MaxDate = DateTime.Today,
        Value = new DateTime(2000, 1, 1)
    };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "Vazgeç", Width = 100, Height = 42, Stil = ButonStili.Ikincil };

    public AileBireyiFormu(int policeId, AileBireyi? mevcut = null)
    {
        _policeId = policeId;
        _mevcut = mevcut;

        Text = mevcut == null ? "Aile Bireyi Ekle" : "Aile Bireyini Düzenle";
        ClientSize = new Size(440, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Kart;
        Font = Tema.Normal;

        // Sıra, Yakinlik enum sırasıyla aynı olmalı: Es, Cocuk, Anne, Baba, Diger
        _cmbYakinlik.Items.AddRange(new object[] { "Eş", "Çocuk", "Anne", "Baba", "Diğer" });
        _cmbYakinlik.SelectedIndex = 1;

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

        Alan(akis, "Yakınlık", _cmbYakinlik);
        Alan(akis, "T.C. Kimlik No", _txtTc);
        akis.Controls.Add(_lblTc);
        Alan(akis, "Ad", _txtAd);
        Alan(akis, "Soyad", _txtSoyad);
        Alan(akis, "Doğum Tarihi", _dtDogum);

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
            _cmbYakinlik.SelectedIndex = (int)mevcut.Yakinlik;
            _txtTc.Text = mevcut.TcKimlikNo;
            _txtAd.Text = mevcut.Ad;
            _txtSoyad.Text = mevcut.Soyad;
            _dtDogum.Value = mevcut.DogumTarihi;
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

        var a = new AileBireyi
        {
            Id = _mevcut?.Id ?? 0,
            PoliceId = _policeId,
            Yakinlik = (Yakinlik)_cmbYakinlik.SelectedIndex,
            TcKimlikNo = _txtTc.Text,
            Ad = _txtAd.Text,
            Soyad = _txtSoyad.Text,
            DogumTarihi = _dtDogum.Value.Date
        };

        var sonuc = _mevcut == null
            ? await _servis.EkleAsync(a)
            : await _servis.GuncelleAsync(a);

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