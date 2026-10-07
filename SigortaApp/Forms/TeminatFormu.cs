using SigortaApp.Models;
using SigortaApp.Services;
using SigortaApp.UI;

namespace SigortaApp.Forms;

public class TeminatFormu : Form
{
    private readonly TeminatService _servis = new();
    private readonly Teminat? _mevcut;

    private readonly TextBox _txtKod = new() { MaxLength = 20, PlaceholderText = "ör. YATARAK" };
    private readonly TextBox _txtAd = new() { MaxLength = 80 };
    private readonly TextBox _txtAciklama = new() { Multiline = true, Height = 90, MaxLength = 300 };
    private readonly Label _lblHata = new() { ForeColor = Tema.Tehlike, Font = Tema.Kucuk, AutoSize = false, Height = 22 };
    private readonly YuvarlakButon _btnKaydet = new() { Text = "Kaydet", Width = 120, Height = 42 };
    private readonly YuvarlakButon _btnIptal = new() { Text = "Vazgeç", Width = 100, Height = 42, Stil = ButonStili.Ikincil };

    public TeminatFormu(Teminat? mevcut = null)
    {
        _mevcut = mevcut;

        Text = mevcut == null ? "Yeni Teminat" : "Teminatı Düzenle";
        ClientSize = new Size(440, 500);
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

        Alan(akis, "Teminat Kodu", _txtKod);
        Alan(akis, "Teminat Adı", _txtAd);
        Alan(akis, "Açıklama (isteğe bağlı)", _txtAciklama);

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
            _txtKod.Text = mevcut.Kod;
            _txtAd.Text = mevcut.Ad;
            _txtAciklama.Text = mevcut.Aciklama;
        }

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

    private async void Kaydet_Click(object? sender, EventArgs e)
    {
        _lblHata.Text = "";
        _btnKaydet.Enabled = false;

        var t = new Teminat
        {
            Id = _mevcut?.Id ?? 0,
            Kod = _txtKod.Text,
            Ad = _txtAd.Text,
            Aciklama = _txtAciklama.Text
        };

        var sonuc = _mevcut == null
            ? await _servis.EkleAsync(t)
            : await _servis.GuncelleAsync(t);

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